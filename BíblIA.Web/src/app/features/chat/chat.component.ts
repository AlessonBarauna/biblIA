import { Component, OnInit, signal, computed, ViewChild, ElementRef, AfterViewChecked, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatListModule } from '@angular/material/list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatMenuModule } from '@angular/material/menu';
import { DatePipe } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import { marked } from 'marked';
import { ApiService, Chat, ChatMessage } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
import { ChatStateService } from '../../services/chat-state.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    DatePipe,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatDividerModule,
    MatTooltipModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatChipsModule,
    MatMenuModule
  ],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, AfterViewChecked {
  // Injections
  private apiService = inject(ApiService);
  private authService = inject(AuthService);
  private sanitizer = inject(DomSanitizer);
  private chatState = inject(ChatStateService);

  // Signals (Angular 21 - reactivity)
  readonly chatId = signal<number | null>(null);
  readonly messages = signal<ChatMessage[]>([]);
  readonly chats = signal<Chat[]>([]);
  readonly messageContent = signal<string>('');
  readonly loading = signal<boolean>(false);
  readonly showSidebar = signal<boolean>(true);
  readonly copiedMessageId = signal<number | null>(null);
  
  // Contexto bíblico
  readonly biblicalContext = signal<{ book: string; chapter?: number } | null>(null);
  readonly selectedBook = signal<string>('Mateus');
  readonly selectedChapter = signal<number | null>(null);
  readonly bibleBooks = signal<string[]>([
    'Gênesis', 'Êxodo', 'Levítico', 'Números', 'Deuteronômio',
    'Josué', 'Juízes', 'Rute', '1 Samuel', '2 Samuel',
    '1 Reis', '2 Reis', '1 Crônicas', '2 Crônicas', 'Esdras',
    'Neemias', 'Ester', 'Jó', 'Salmos', 'Provérbios',
    'Eclesiastes', 'Cântico dos Cânticos', 'Isaías', 'Jeremias', 'Lamentações',
    'Ezequiel', 'Daniel', 'Oséias', 'Joel', 'Amós',
    'Obadias', 'Jonas', 'Miqueias', 'Naum', 'Habacuque',
    'Sofonias', 'Ageu', 'Zacarias', 'Malaquias', 'Mateus',
    'Marcos', 'Lucas', 'João', 'Atos', 'Romanos',
    '1 Coríntios', '2 Coríntios', 'Gálatas', 'Efésios', 'Filipenses',
    'Colossenses', '1 Tessalonicenses', '2 Tessalonicenses', '1 Timóteo', '2 Timóteo',
    'Tito', 'Filemon', 'Hebreus', 'Tiago', '1 Pedro',
    '2 Pedro', '1 João', '2 João', '3 João', 'Judas', 'Apocalipse'
  ]);
  
  // Referências cruzadas
  readonly suggestedVerses = signal<any[]>([]);
  readonly showVerseReference = signal<number | null>(null);
  
  // Histórico e busca
  readonly searchQuery = signal<string>('');
  readonly filteredChats = computed(() => {
    const query = this.searchQuery().toLowerCase();
    if (!query) return this.chats();
    return this.chats().filter(chat =>
      chat.title.toLowerCase().includes(query)
    );
  });

  // Computed signals
  readonly hasMessages = computed(() => this.messages().length > 0);
  readonly isInputEmpty = computed(() => !this.messageContent().trim());
  readonly canSend = computed(() => !this.isInputEmpty() && this.chatId() !== null && !this.loading());
  readonly contextText = computed(() => {
    const ctx = this.biblicalContext();
    if (!ctx) return null;
    return ctx.chapter ? `${ctx.book} ${ctx.chapter}` : ctx.book;
  });

  @ViewChild('messagesContainer') messagesContainer!: ElementRef;
  private shouldScroll = false;

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (!user) return;

    this.apiService.getUserChats(user.id).subscribe({
      next: (chats) => {
        const sorted = [...chats].sort((a, b) =>
          new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
        );
        this.chats.set(sorted);

        if (sorted.length === 0) {
          // Sem histórico: mostra estado vazio, usuário clica "Nova conversa"
          return;
        }

        // Restaura o chat que estava ativo antes da navegação,
        // ou seleciona o mais recente se for a primeira visita.
        const savedId = this.chatState.activeChatId();
        const toSelect = savedId
          ? (sorted.find(c => c.id === savedId) ?? sorted[0])
          : sorted[0];
        this.selectChat(toSelect);
      },
      error: (err) => console.error('Erro ao carregar chats:', err)
    });
  }

  ngAfterViewChecked(): void {
    if (this.shouldScroll) {
      this.scrollToBottom();
      this.shouldScroll = false;
    }
  }

  scrollToBottom(): void {
    try {
      this.messagesContainer.nativeElement.scrollTop =
        this.messagesContainer.nativeElement.scrollHeight;
    } catch { }
  }

  loadUserChats(userId: number): void {
    this.apiService.getUserChats(userId).subscribe({
      next: (chats) => this.chats.set(chats),
      error: (error) => console.error('Erro ao carregar chats:', error)
    });
  }

  createNewChat(): void {
    const user = this.authService.getCurrentUser();
    if (!user) return;

    const timestamp = new Date().toLocaleTimeString('pt-BR', {
      hour: '2-digit',
      minute: '2-digit'
    });

    this.apiService.createChat({
      userId: user.id,
      title: `Conversa - ${timestamp}`
    }).subscribe({
      next: (chat) => {
        this.chatId.set(chat.id);
        this.messages.set([]);
        this.loadUserChats(user.id);
      },
      error: (error) => console.error('Erro ao criar chat:', error)
    });
  }

  selectChat(chat: Chat): void {
    this.chatState.activeChatId.set(chat.id);
    this.chatId.set(chat.id);
    this.loadChat();
  }

  deleteChat(chatId: number, event: Event): void {
    event.stopPropagation();
    if (!confirm('Tem certeza que deseja deletar esta conversa?')) return;

    this.apiService.deleteChat(chatId).subscribe({
      next: () => {
        const user = this.authService.getCurrentUser();
        if (user) {
          this.loadUserChats(user.id);
          if (this.chatId() === chatId) {
            this.createNewChat();
          }
        }
      },
      error: (error) => console.error('Erro ao deletar chat:', error)
    });
  }

  sendMessage(): void {
    if (!this.canSend() || !this.chatId()) return;

    const content = this.messageContent().trim();
    this.messageContent.set('');
    this.loading.set(true);
    this.shouldScroll = true;

    // Adicionar mensagem do usuário imediatamente (optimistic update)
    const userMessage: ChatMessage = {
      id: Math.random(),
      chatId: this.chatId()!,
      role: 'user',
      content: content,
      createdAt: new Date().toISOString()
    };
    this.messages.update(msg => [...msg, userMessage]);

    // Enviar com contexto bíblico se disponível
    const messageWithContext = this.biblicalContext() 
      ? `[Contexto: ${this.contextText()}]\n\n${content}`
      : content;

    this.apiService.sendChatMessage(this.chatId()!, messageWithContext).subscribe({
      next: () => {
        this.loadChat();
        // Atualizar título do chat se for a primeira mensagem
        if (this.messages().length === 2) {
          this.updateChatTitle(content);
        }
        // Buscar referências cruzadas se mensagem mencionar escritura
        this.suggestCrossReferences(content);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao enviar mensagem:', error);
        this.loading.set(false);
        // Remove a mensagem do usuário em caso de erro
        this.messages.update(msg => msg.filter(m => m.id !== userMessage.id));
      }
    });
  }

  // Atualizar título do chat com resumo automático
  private updateChatTitle(firstMessage: string): void {
    if (!this.chatId()) return;
    
    const title = firstMessage.length > 50 
      ? firstMessage.substring(0, 50) + '...'
      : firstMessage;
    
    this.apiService.updateChat(this.chatId()!, { title }).subscribe({
      next: () => this.loadUserChats(this.authService.getCurrentUser()?.id || 0),
      error: (error) => console.error('Erro ao atualizar título:', error)
    });
  }

  // Buscar referências cruzadas baseado em palavras-chave
  private suggestCrossReferences(message: string): void {
    // Search common biblical terms
    const biblicalTerms = ['graça', 'perdão', 'salvação', 'fé', 'redenção', 'ressurreição'];
    const hasTerms = biblicalTerms.some(term => 
      message.toLowerCase().includes(term)
    );

    if (hasTerms) {
      // Buscar versículos relacionados
      this.apiService.searchBibleVerses(message.substring(0, 30)).subscribe({
        next: (verses) => {
          this.suggestedVerses.set(verses.slice(0, 3)); // Top 3 verses
        },
        error: (error) => console.error('Erro ao buscar versículos:', error)
      });
    }
  }

  // Selecionar contexto bíblico
  setContext(book: string, chapter?: number): void {
    this.selectedBook.set(book);
    if (chapter) {
      this.selectedChapter.set(chapter);
      this.biblicalContext.set({ book, chapter });
    } else {
      this.selectedChapter.set(null);
      this.biblicalContext.set({ book });
    }
  }

  // Limpar contexto
  clearContext(): void {
    this.biblicalContext.set(null);
    this.selectedBook.set('Mateus');
    this.selectedChapter.set(null);
  }

  // Atualizar busca de histórico
  updateSearchQuery(query: string): void {
    this.searchQuery.set(query);
  }

  loadChat(): void {
    if (!this.chatId()) return;

    this.apiService.getChat(this.chatId()!).subscribe({
      next: (chat) => {
        this.messages.set(chat.messages);
        this.shouldScroll = true;
      },
      error: (error) => console.error('Erro ao carregar chat:', error)
    });
  }

  copyMessage(content: string, messageId: number): void {
    navigator.clipboard.writeText(content).then(() => {
      this.copiedMessageId.set(messageId);
      setTimeout(() => this.copiedMessageId.set(null), 2000);
    });
  }

  toggleSidebar(): void {
    this.showSidebar.update(show => !show);
  }

  getAvatar(role: 'user' | 'assistant'): string {
    return role === 'user' ? 'person' : 'smart_toy';
  }

  // Converte markdown para HTML seguro.
  // Usado apenas para mensagens do assistente — respostas do Claude usam
  // headers, bold, listas etc. que ficam ilegíveis como texto puro.
  renderMarkdown(content: string): SafeHtml {
    const html = marked.parse(content) as string;
    return this.sanitizer.bypassSecurityTrustHtml(html);
  }
}