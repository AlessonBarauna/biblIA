import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { BibleComponent } from './features/bible/bible.component';
import { ChatComponent } from './features/chat/chat.component';
import { TheologyComponent } from './features/theology/theology.component';
import { EschatologyComponent } from './features/eschatology/eschatology.component';
import { HistoryComponent } from './features/history/history.component';
import { AuthComponent } from './features/auth/auth.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '',          component: HomeComponent },
  { path: 'auth',      component: AuthComponent },
  { path: 'bible',     component: BibleComponent },
  // Chat requer login — o authGuard redireciona para /auth se não autenticado
  { path: 'chat',      component: ChatComponent, canActivate: [authGuard] },
  { path: 'theology',  component: TheologyComponent },
  { path: 'eschatology', component: EschatologyComponent },
  { path: 'history',   component: HistoryComponent },
  { path: '**',        redirectTo: '' }
];
