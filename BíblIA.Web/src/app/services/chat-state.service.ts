import { Injectable, signal } from '@angular/core';

// Serviço singleton que persiste o chatId ativo entre navegações.
// Sem isso, toda visita ao /chat recria o componente e perde o estado.
@Injectable({ providedIn: 'root' })
export class ChatStateService {
  readonly activeChatId = signal<number | null>(null);
}
