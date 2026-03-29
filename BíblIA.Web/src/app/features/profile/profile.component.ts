import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ApiService, UserProfile, VerseNote } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatSlideToggleModule
  ],
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  private api   = inject(ApiService);
  private auth  = inject(AuthService);
  readonly notifications = inject(NotificationService);

  loading = signal(true);
  profile = signal<UserProfile | null>(null);

  // ── Edição de nome ────────────────────────────────────────────────────────
  editingName  = signal(false);
  editNameValue = signal('');
  savingName   = signal(false);
  nameError    = signal('');

  // ── Troca de senha ────────────────────────────────────────────────────────
  showPasswordForm = signal(false);
  currentPassword  = signal('');
  newPassword      = signal('');
  confirmPassword  = signal('');
  savingPassword   = signal(false);
  passwordError    = signal('');
  passwordSuccess  = signal('');

  ngOnInit(): void {
    this.api.getProfile().subscribe({
      next:  p  => { this.profile.set(p); this.loading.set(false); },
      error: () => this.loading.set(false)
    });
  }

  // ── Ações de nome ─────────────────────────────────────────────────────────

  startEditName(): void {
    this.editNameValue.set(this.profile()?.name ?? '');
    this.nameError.set('');
    this.editingName.set(true);
  }

  cancelEditName(): void {
    this.editingName.set(false);
  }

  saveName(): void {
    const name = this.editNameValue().trim();
    if (!name) { this.nameError.set('O nome não pode estar vazio.'); return; }

    this.savingName.set(true);
    this.api.updateProfile({ name }).subscribe({
      next: res => {
        this.profile.update(p => p ? { ...p, name: res.name } : p);
        this.auth.updateUserName(res.name); // sincronia com toolbar
        this.editingName.set(false);
        this.savingName.set(false);
      },
      error: () => {
        this.nameError.set('Erro ao salvar. Tente novamente.');
        this.savingName.set(false);
      }
    });
  }

  // ── Ações de senha ────────────────────────────────────────────────────────

  submitPassword(): void {
    this.passwordError.set('');
    this.passwordSuccess.set('');

    if (this.newPassword() !== this.confirmPassword()) {
      this.passwordError.set('As senhas não conferem.');
      return;
    }
    if (this.newPassword().length < 6) {
      this.passwordError.set('A nova senha deve ter pelo menos 6 caracteres.');
      return;
    }

    this.savingPassword.set(true);
    this.api.changePassword({
      currentPassword:  this.currentPassword(),
      newPassword:      this.newPassword(),
      confirmPassword:  this.confirmPassword()
    }).subscribe({
      next: res => {
        this.passwordSuccess.set(res.message);
        this.currentPassword.set('');
        this.newPassword.set('');
        this.confirmPassword.set('');
        this.savingPassword.set(false);
        this.showPasswordForm.set(false);
      },
      error: err => {
        this.passwordError.set(err.error?.message ?? 'Erro ao alterar senha.');
        this.savingPassword.set(false);
      }
    });
  }

  // ── Exportar anotações ────────────────────────────────────────────────────
  //
  // Busca todas as anotações do usuário, gera um arquivo Markdown agrupado
  // por livro e capítulo, e dispara o download no browser — sem backend extra.

  exportingNotes = signal(false);

  exportNotes(): void {
    this.exportingNotes.set(true);
    this.api.getAllVerseNotes().subscribe({
      next: notes => {
        this.exportingNotes.set(false);
        if (notes.length === 0) return;
        const md = this.buildMarkdown(notes);
        this.downloadFile(md, 'anotacoes-biblicas.md', 'text/markdown;charset=utf-8');
      },
      error: () => this.exportingNotes.set(false)
    });
  }

  private buildMarkdown(notes: VerseNote[]): string {
    const date = new Date().toLocaleDateString('pt-BR', { day: 'numeric', month: 'long', year: 'numeric' });
    const lines: string[] = [`# Minhas Anotações Bíblicas`, `*Exportado em ${date}*`, ``];

    // Agrupa por livro
    const byBook = new Map<string, VerseNote[]>();
    for (const n of notes) {
      const key = n.bookName || `Livro ${n.bookId}`;
      if (!byBook.has(key)) byBook.set(key, []);
      byBook.get(key)!.push(n);
    }

    for (const [book, bookNotes] of byBook) {
      lines.push(`---`, ``, `## ${book}`, ``);

      // Agrupa por capítulo dentro do livro
      const byChapter = new Map<number, VerseNote[]>();
      for (const n of bookNotes) {
        if (!byChapter.has(n.chapter)) byChapter.set(n.chapter, []);
        byChapter.get(n.chapter)!.push(n);
      }

      for (const [ch, chNotes] of byChapter) {
        lines.push(`### Capítulo ${ch}`, ``);
        for (const n of chNotes) {
          lines.push(`**Versículo ${n.verse}**`, `> ${n.note}`, ``);
        }
      }
    }

    return lines.join('\n');
  }

  private downloadFile(content: string, filename: string, mimeType: string): void {
    const blob = new Blob([content], { type: mimeType });
    const url  = URL.createObjectURL(blob);
    const a    = document.createElement('a');
    a.href     = url;
    a.download = filename;
    a.click();
    URL.revokeObjectURL(url);
  }

  // ── Lembretes de leitura ──────────────────────────────────────────────────

  async enableNotifications(): Promise<void> {
    await this.notifications.requestPermission();
    if (this.notifications.permission() === 'granted') {
      this.notifications.saveSettings(true, this.notifications.reminderTime());
    }
  }

  onReminderToggle(checked: boolean): void {
    if (checked && this.notifications.permission() !== 'granted') {
      this.enableNotifications();
    } else {
      this.notifications.saveSettings(checked, this.notifications.reminderTime());
    }
  }

  onReminderTimeChange(time: string): void {
    this.notifications.saveSettings(this.notifications.reminderEnabled(), time);
  }

  // ── Helpers ───────────────────────────────────────────────────────────────

  get initials(): string {
    const name = this.profile()?.name ?? '';
    return name.split(' ').slice(0, 2).map(w => w[0]).join('').toUpperCase();
  }

  formatDate(iso: string): string {
    return new Date(iso).toLocaleDateString('pt-BR', { day: 'numeric', month: 'long', year: 'numeric' });
  }
}
