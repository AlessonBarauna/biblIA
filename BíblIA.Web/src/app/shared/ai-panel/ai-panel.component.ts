import { Component, Input, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';
import { ApiService } from '../../services/api.service';

interface AiMessage {
  role: 'user' | 'assistant';
  content: string;
}

@Component({
  selector: 'app-ai-panel',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatIconModule,
    MatInputModule,
    MatFormFieldModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './ai-panel.component.html',
  styleUrls: ['./ai-panel.component.css']
})
export class AiPanelComponent {
  @Input() domain: 'general' | 'bible' | 'theology' | 'history' | 'eschatology' = 'general';
  @Input() placeholder = 'Faça uma pergunta...';
  @Input() title = 'Assistente IA';

  private api = inject(ApiService);
  private sanitizer = inject(DomSanitizer);

  messages = signal<AiMessage[]>([]);
  input = signal('');
  loading = signal(false);

  send(): void {
    const question = this.input().trim();
    if (!question || this.loading()) return;

    this.messages.update(m => [...m, { role: 'user', content: question }]);
    this.input.set('');
    this.loading.set(true);

    this.api.askAi(question, this.domain).subscribe({
      next: res => {
        this.messages.update(m => [...m, { role: 'assistant', content: res.answer }]);
        this.loading.set(false);
      },
      error: () => {
        this.messages.update(m => [...m, { role: 'assistant', content: 'Erro ao conectar com a IA. Verifique sua chave Gemini.' }]);
        this.loading.set(false);
      }
    });
  }

  onKeydown(event: KeyboardEvent): void {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.send();
    }
  }

  renderMarkdown(content: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(marked.parse(content) as string);
  }

  clearMessages(): void {
    this.messages.set([]);
  }
}
