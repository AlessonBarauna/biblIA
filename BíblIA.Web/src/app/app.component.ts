import { Component, inject, signal, DestroyRef, HostListener, ElementRef, ViewChild } from '@angular/core';
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
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { AuthService } from './services/auth.service';
import { ThemeService } from './services/theme.service';
import { NotificationService } from './services/notification.service';
import { GlobalSearchService } from './services/global-search.service';

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
    MatDividerModule,
    MatTooltipModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  protected auth         = inject(AuthService);
  protected theme        = inject(ThemeService);
  protected globalSearch = inject(GlobalSearchService);
  private notifications  = inject(NotificationService);
  private router         = inject(Router);
  private destroyRef     = inject(DestroyRef);

  @ViewChild('searchInput') searchInputRef!: ElementRef<HTMLInputElement>;

  @HostListener('document:keydown', ['$event'])
  onKey(e: KeyboardEvent): void {
    // Ctrl+K ou Cmd+K foca a busca global
    if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
      e.preventDefault();
      this.searchInputRef?.nativeElement.focus();
    }
    if (e.key === 'Escape') this.globalSearch.clear();
  }

  @HostListener('document:click', ['$event'])
  onDocClick(e: MouseEvent): void {
    const target = e.target as HTMLElement;
    if (!target.closest('.global-search-wrapper')) {
      this.globalSearch.open.set(false);
    }
  }

  // Esconde toolbar e nav na rota de autenticação para uma página de login limpa
  readonly isAuthRoute = signal(this.router.url.startsWith('/auth'));

  title = 'BíblIA';

  constructor() {
    this.notifications.checkAndNotify();

    this.router.events.pipe(
      filter((e): e is NavigationEnd => e instanceof NavigationEnd),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(e => this.isAuthRoute.set(e.urlAfterRedirects.startsWith('/auth')));
  }

  menuItems = [
    { label: 'Início',     route: '/',            icon: 'home' },
    { label: 'Chat',       route: '/chat',         icon: 'chat' },
    { label: 'Bíblia',     route: '/bible',        icon: 'book' },
    { label: 'Favoritos',  route: '/bookmarks',    icon: 'bookmark' },
    { label: 'Leitura',    route: '/reading',      icon: 'auto_stories' },
    { label: 'Teologia',   route: '/theology',     icon: 'school' },
    { label: 'Escatologia',route: '/eschatology',  icon: 'public' },
    { label: 'História',   route: '/history',      icon: 'history' }
  ];
}
