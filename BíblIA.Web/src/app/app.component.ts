import { Component, inject, signal, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { filter } from 'rxjs';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatMenuModule,
    MatIconModule,
    MatSidenavModule,
    MatListModule,
    MatDividerModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  protected auth = inject(AuthService);
  private router  = inject(Router);
  private destroyRef = inject(DestroyRef);

  // Esconde toolbar e nav na rota de autenticação para uma página de login limpa
  readonly isAuthRoute = signal(this.router.url.startsWith('/auth'));

  title = 'BíblIA';

  constructor() {
    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(e => this.isAuthRoute.set(e.urlAfterRedirects.startsWith('/auth')));
  }

  menuItems = [
    { label: 'Início',     route: '/',           icon: 'home' },
    { label: 'Chat',       route: '/chat',        icon: 'chat' },
    { label: 'Bíblia',     route: '/bible',       icon: 'book' },
    { label: 'Teologia',   route: '/theology',    icon: 'school' },
    { label: 'Escatologia',route: '/eschatology', icon: 'public' },
    { label: 'História',   route: '/history',     icon: 'history' }
  ];
}
