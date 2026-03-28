import { Routes } from '@angular/router';
import { HomeComponent } from './features/home/home.component';
import { BibleComponent } from './features/bible/bible.component';
import { ChatComponent } from './features/chat/chat.component';
import { TheologyComponent } from './features/theology/theology.component';
import { EschatologyComponent } from './features/eschatology/eschatology.component';
import { HistoryComponent } from './features/history/history.component';
import { AuthComponent } from './features/auth/auth.component';
import { BookmarksComponent } from './features/bookmarks/bookmarks.component';
import { ReadingComponent } from './features/reading/reading.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'auth',        component: AuthComponent },
  { path: '',            component: HomeComponent,        canActivate: [authGuard] },
  { path: 'bible',       component: BibleComponent,       canActivate: [authGuard] },
  { path: 'chat',        component: ChatComponent,        canActivate: [authGuard] },
  { path: 'theology',    component: TheologyComponent,    canActivate: [authGuard] },
  { path: 'eschatology', component: EschatologyComponent, canActivate: [authGuard] },
  { path: 'history',     component: HistoryComponent,     canActivate: [authGuard] },
  { path: 'bookmarks',   component: BookmarksComponent,   canActivate: [authGuard] },
  { path: 'reading',     component: ReadingComponent,     canActivate: [authGuard] },
  { path: '**',          redirectTo: '' }
];
