import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatCardModule, RouterModule],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  features = [
    {
      title: 'Chat com IA',
      description: 'Converse com uma IA especializada em teologia bíblica',
      route: '/chat',
      icon: '💬'
    },
    {
      title: 'Bíblia',
      description: 'Acesse versículos bíblicos de diferentes versões',
      route: '/bible',
      icon: '📖'
    },
    {
      title: 'Teologia',
      description: 'Explore conceitos teológicos profundos',
      route: '/theology',
      icon: '⛪'
    },
    {
      title: 'Escatologia',
      description: 'Entenda os últimos tempos e profecias',
      route: '/eschatology',
      icon: '🔮'
    },
    {
      title: 'História',
      description: 'Contexto histórico dos livros bíblicos',
      route: '/history',
      icon: '📚'
    }
  ];
}