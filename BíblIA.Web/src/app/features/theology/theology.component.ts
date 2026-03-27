import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService, TheologyCourse, TheologyModule, TheologyQuiz } from '../../services/api.service';
import { AuthService } from '../../services/auth.service';
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
    MatProgressBarModule,
    MatDividerModule,
    MatTooltipModule,
    AiPanelComponent,
  ],
  templateUrl: './theology.component.html',
  styleUrls: ['./theology.component.css']
})
export class TheologyComponent implements OnInit {
  private api  = inject(ApiService);
  private auth = inject(AuthService);

  // ── Estado ────────────────────────────────────────────────────────────────
  view = signal<View>('courses');
  loading = signal(false);
  error = signal<string | null>(null);

  courses = signal<TheologyCourse[]>([]);
  selectedCourse = signal<TheologyCourse | null>(null);
  modulesWithQuizzes = signal<ModuleWithQuizzes[]>([]);

  // progressSet: conjunto de moduleIds concluídos — lookup O(1)
  progressSet = signal<Set<number>>(new Set());
  // courseProgressMap: courseId → quantidade de módulos concluídos
  courseProgressMap = signal<Map<number, number>>(new Map());

  readonly isLoggedIn = this.auth.isLoggedIn;

  // Separa cursos por nível para exibição agrupada
  basicCourses    = computed(() => this.courses().filter(c => c.level === 'Básico'));
  intermCourses   = computed(() => this.courses().filter(c => c.level === 'Intermediário'));
  advancedCourses = computed(() => this.courses().filter(c => c.level === 'Avançado'));

  levelOrder = ['Básico', 'Intermediário', 'Avançado'];

  ngOnInit() {
    this.loadCourses();
    if (this.auth.isLoggedIn()) {
      this.loadProgress();
    }
  }

  private loadProgress() {
    this.api.getProgress().subscribe({
      next: list => {
        const modules = new Set<number>(list.map(p => p.moduleId));
        const courses = new Map<number, number>();
        list.forEach(p => courses.set(p.courseId, (courses.get(p.courseId) ?? 0) + 1));
        this.progressSet.set(modules);
        this.courseProgressMap.set(courses);
      },
      error: () => {}
    });
  }

  isModuleCompleted(moduleId: number): boolean {
    return this.progressSet().has(moduleId);
  }

  courseCompletedCount(courseId: number): number {
    return this.courseProgressMap().get(courseId) ?? 0;
  }

  toggleModuleComplete(item: ModuleWithQuizzes) {
    const course = this.selectedCourse()!;
    const moduleId = item.module.id;

    if (this.isModuleCompleted(moduleId)) {
      this.api.undoModule(moduleId).subscribe({
        next: () => {
          const modules = new Set(this.progressSet());
          modules.delete(moduleId);
          this.progressSet.set(modules);

          const courses = new Map(this.courseProgressMap());
          courses.set(course.id, Math.max(0, (courses.get(course.id) ?? 1) - 1));
          this.courseProgressMap.set(courses);
        }
      });
    } else {
      const score = this.calcModuleScore(item);
      this.api.completeModule({ courseId: course.id, moduleId, score }).subscribe({
        next: () => {
          const modules = new Set(this.progressSet());
          modules.add(moduleId);
          this.progressSet.set(modules);

          const courses = new Map(this.courseProgressMap());
          courses.set(course.id, (courses.get(course.id) ?? 0) + 1);
          this.courseProgressMap.set(courses);
        }
      });
    }
  }

  // Calcula pontuação dos quizzes do módulo (0–100). 0 se sem quizzes respondidos.
  private calcModuleScore(item: ModuleWithQuizzes): number {
    const answered = item.quizzes.filter(q => item.quizStates[q.id]?.showResult);
    if (answered.length === 0) return 0;
    const correct = answered.filter(q => item.quizStates[q.id].selectedAnswer === q.correctAnswer).length;
    return Math.round((correct / answered.length) * 100);
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
