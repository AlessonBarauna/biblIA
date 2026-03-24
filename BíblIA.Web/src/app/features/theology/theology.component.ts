import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService, TheologyCourse, TheologyModule, TheologyQuiz } from '../../services/api.service';
import { AiPanelComponent } from '../../shared/ai-panel/ai-panel.component';

// Nível de detalhe da view — padrão de máquina de estados finitos.
// Mantemos tudo em signals para que o template reaja automaticamente.
type View = 'courses' | 'modules';

// Estado por questão — cada quiz tem seu próprio selectedAnswer e showResult.
// Antes era per-módulo, o que causava: responder Q1 → showResult=true para o
// módulo inteiro → Q2 já aparecia como "respondida" automaticamente.
interface QuizState {
  selectedAnswer: string | null;
  showResult: boolean;
}

interface ModuleWithQuizzes {
  module: TheologyModule;
  quizzes: TheologyQuiz[];
  quizzesLoaded: boolean;
  quizStates: Record<number, QuizState>; // chave = quiz.id
}

@Component({
  selector: 'app-theology',
  standalone: true,
  imports: [
    MatCardModule,
    MatExpansionModule,
    MatButtonModule,
    MatChipsModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    AiPanelComponent,
  ],
  templateUrl: './theology.component.html',
  styleUrls: ['./theology.component.css']
})
export class TheologyComponent implements OnInit {
  private api = inject(ApiService);

  // ── Estado ────────────────────────────────────────────────────────────────
  view = signal<View>('courses');
  loading = signal(false);
  error = signal<string | null>(null);

  courses = signal<TheologyCourse[]>([]);
  selectedCourse = signal<TheologyCourse | null>(null);
  modulesWithQuizzes = signal<ModuleWithQuizzes[]>([]);

  // Separa cursos por nível para exibição agrupada
  basicCourses    = computed(() => this.courses().filter(c => c.level === 'Básico'));
  intermCourses   = computed(() => this.courses().filter(c => c.level === 'Intermediário'));
  advancedCourses = computed(() => this.courses().filter(c => c.level === 'Avançado'));

  levelOrder = ['Básico', 'Intermediário', 'Avançado'];

  ngOnInit() {
    this.loadCourses();
  }

  private loadCourses() {
    this.loading.set(true);
    this.error.set(null);
    this.api.getTheologyCourses().subscribe({
      next: (courses) => {
        this.courses.set(courses);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Não foi possível carregar os cursos. Verifique a conexão com o servidor.');
        this.loading.set(false);
      }
    });
  }

  selectCourse(course: TheologyCourse) {
    this.selectedCourse.set(course);
    this.view.set('modules');
    this.modulesWithQuizzes.set([]);
    this.loading.set(true);

    this.api.getTheologyModules(course.id).subscribe({
      next: (modules) => {
        this.modulesWithQuizzes.set(
          modules.map(m => ({ module: m, quizzes: [], quizzesLoaded: false, quizStates: {} }))
        );
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Não foi possível carregar os módulos.');
        this.loading.set(false);
      }
    });
  }

  backToCourses() {
    this.view.set('courses');
    this.selectedCourse.set(null);
    this.modulesWithQuizzes.set([]);
  }

  // Lazy load de quizzes — só carrega quando o painel do módulo é aberto
  loadQuizzes(item: ModuleWithQuizzes) {
    if (item.quizzesLoaded) return;

    this.api.getTheologyQuizzes(item.module.id).subscribe({
      next: (quizzes) => {
        // Atualiza o item específico de forma imutável
        this.modulesWithQuizzes.update(list =>
          list.map(i => i.module.id === item.module.id
            ? { ...i, quizzes, quizzesLoaded: true }
            : i
          )
        );
      }
    });
  }

  answerQuiz(moduleId: number, quizId: number, answer: string) {
    this.modulesWithQuizzes.update(list =>
      list.map(i => i.module.id === moduleId
        ? { ...i, quizStates: { ...i.quizStates, [quizId]: { selectedAnswer: answer, showResult: true } } }
        : i
      )
    );
  }

  resetQuiz(moduleId: number, quizId: number) {
    this.modulesWithQuizzes.update(list =>
      list.map(i => i.module.id === moduleId
        ? { ...i, quizStates: { ...i.quizStates, [quizId]: { selectedAnswer: null, showResult: false } } }
        : i
      )
    );
  }

  getQuizState(item: ModuleWithQuizzes, quizId: number): QuizState {
    return item.quizStates[quizId] ?? { selectedAnswer: null, showResult: false };
  }

  getLevelIcon(level: string): string {
    const icons: Record<string, string> = { 'Básico': 'school', 'Intermediário': 'menu_book', 'Avançado': 'psychology' };
    return icons[level] ?? 'school';
  }

  getLevelColor(level: string): string {
    const colors: Record<string, string> = { 'Básico': 'primary', 'Intermediário': 'accent', 'Avançado': 'warn' };
    return colors[level] ?? 'primary';
  }
}
