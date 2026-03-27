import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

// ── Interfaces de domínio ────────────────────────────────────────────────────
export interface AuthResponse {
  success: boolean;
  message: string;
  token?: string;
  user?: { id: number; name: string; email: string };
}

export interface BibleBook {
  id: number;
  name: string;
  abbreviation: string;
  testament: string; // 'OT' | 'NT'
  chapterCount: number;
  description: string;
  orderIndex: number;
}

export interface BibleVerse {
  id: number;
  bookId: number;
  bookName: string;
  chapter: number;
  verse: number;
  textKJV: string;
  textAA:  string;
  textACF: string;
  textNVI: string;
}

export interface ChapterSummary {
  chapter: number;
  verseCount: number;
}

export interface TheologyCourse {
  id: number;
  title: string;
  description: string;
  category: string;
  durationHours: number;
  level: string;
  imageIcon: string;
  moduleCount: number;
  externalUrl?: string;
  provider?: string;
}

export interface BibleStudyNote {
  id: number;
  bookId: number;
  chapter: number;
  title: string;
  context: string;
  theologicalSignificance: string;
  keyThemes: string;
  crossReferences: string;
  commentary: string;
  authorNote: string;
}

export interface TheologyModule {
  id: number;
  courseId: number;
  title: string;
  content: string;
  references: string;
  orderIndex: number;
}

export interface TheologyQuiz {
  id: number;
  moduleId: number;
  question: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctAnswer: string;
  explanation: string;
}

export interface ChurchHero {
  id: number;
  name: string;
  period: string;
  birthYear: number | null;
  deathYear: number | null;
  nationality: string;
  biography: string;
  keyContributions: string;
  favoriteVerse: string;
  category: string;
  imageUrl: string;
}

export interface Revival {
  id: number;
  name: string;
  year: number;
  endYear: number | null;
  location: string;
  leaderNames: string;
  description: string;
  keyEvents: string;
  impact: string;
  estimatedConversions: string;
}

export interface EschatologyView {
  id: number;
  name: string;
  summary: string;
  detailedExplanation: string;
  mainTheologians: string;
  keyScriptures: string;
  strengths: string;
  weaknesses: string;
  millenniumView: string;
  raptureView: string;
  tribulationView: string;
}

export interface UserProgress {
  id: number;
  courseId: number;
  moduleId: number;
  completed: boolean;
  completedAt: string | null;
  score: number;
}

export interface Bookmark {
  id: number;
  userId: number;
  bookId: number;
  bookName: string;
  chapter: number;
  verse: number;
  verseText: string;
  note: string;
  createdAt: string;
}

export interface Chat {
  id: number;
  userId: number;
  title: string;
  createdAt: string;
  updatedAt: string;
  messages: ChatMessage[];
}

export interface ChatMessage {
  id: number;
  chatId: number;
  role: 'user' | 'assistant';
  content: string;
  createdAt: string;
}

// ── Service ──────────────────────────────────────────────────────────────────
@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private readonly apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // ── Bible: navegação hierárquica ─────────────────────────────────────────

  getBooks(testament?: 'OT' | 'NT'): Observable<BibleBook[]> {
    const params = testament ? `?testament=${testament}` : '';
    return this.http.get<BibleBook[]>(`${this.apiUrl}/bible/books${params}`);
  }

  getBook(bookId: number): Observable<BibleBook> {
    return this.http.get<BibleBook>(`${this.apiUrl}/bible/books/${bookId}`);
  }

  getChapters(bookId: number): Observable<ChapterSummary[]> {
    return this.http.get<ChapterSummary[]>(`${this.apiUrl}/bible/books/${bookId}/chapters`);
  }

  getChapter(bookId: number, chapter: number): Observable<BibleVerse[]> {
    return this.http.get<BibleVerse[]>(`${this.apiUrl}/bible/books/${bookId}/chapters/${chapter}`);
  }

  getBibleVerse(bookId: number, chapter: number, verse: number): Observable<BibleVerse> {
    return this.http.get<BibleVerse>(`${this.apiUrl}/bible/books/${bookId}/chapters/${chapter}/verses/${verse}`);
  }

  // ── Auth ─────────────────────────────────────────────────────────────────

  register(data: { name: string; email: string; password: string; passwordConfirm: string }): Observable<AuthResponse> {
    console.log('[ApiService] POST /auth/register:', data);
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, data);
  }

  login(data: { email: string; password: string }): Observable<AuthResponse> {
    console.log('[ApiService] POST /auth/login:', data);
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, data);
  }

  // ── Chats ────────────────────────────────────────────────────────────────

  getUserChats(userId: number): Observable<Chat[]> {
    return this.http.get<Chat[]>(`${this.apiUrl}/chats/${userId}`);
  }

  getChat(id: number): Observable<Chat> {
    return this.http.get<Chat>(`${this.apiUrl}/chats/detail/${id}`);
  }

  createChat(data: { userId: number; title: string }): Observable<Chat> {
    return this.http.post<Chat>(`${this.apiUrl}/chats`, data);
  }

  sendChatMessage(chatId: number, content: string): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.apiUrl}/chats/${chatId}/messages`, { chatId, content });
  }

  updateChat(id: number, data: { title: string }): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/chats/${id}`, data);
  }

  deleteChat(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/chats/${id}`);
  }

  // ── Theology: Cursos e módulos ────────────────────────────────────────────

  getTheologyCourses(): Observable<TheologyCourse[]> {
    return this.http.get<TheologyCourse[]>(`${this.apiUrl}/theology/courses`);
  }

  getTheologyCourse(courseId: number): Observable<TheologyCourse> {
    return this.http.get<TheologyCourse>(`${this.apiUrl}/theology/courses/${courseId}`);
  }

  getTheologyModules(courseId: number): Observable<TheologyModule[]> {
    return this.http.get<TheologyModule[]>(`${this.apiUrl}/theology/courses/${courseId}/modules`);
  }

  getTheologyQuizzes(moduleId: number): Observable<TheologyQuiz[]> {
    return this.http.get<TheologyQuiz[]>(`${this.apiUrl}/theology/modules/${moduleId}/quizzes`);
  }

  // ── Eschatology: Visões escatológicas ───────────────────────────────────────

  getEschatologyViews(): Observable<EschatologyView[]> {
    return this.http.get<EschatologyView[]>(`${this.apiUrl}/eschatology/views`);
  }

  compareEschatologyViews(view1: number, view2: number): Observable<{ view1: EschatologyView; view2: EschatologyView; comparison: any }> {
    return this.http.get<any>(`${this.apiUrl}/eschatology/compare`, { params: { view1, view2 } });
  }

  // ── History: Heróis e avivamentos ───────────────────────────────────────────

  getChurchHeroes(category?: string): Observable<ChurchHero[]> {
    let params = new HttpParams();
    if (category) params = params.set('category', category);
    return this.http.get<ChurchHero[]>(`${this.apiUrl}/history/heroes`, { params });
  }

  getRevivals(): Observable<Revival[]> {
    return this.http.get<Revival[]>(`${this.apiUrl}/history/revivals`);
  }

  getChapterNote(bookId: number, chapter: number): Observable<BibleStudyNote> {
    return this.http.get<BibleStudyNote>(`${this.apiUrl}/bible/books/${bookId}/chapters/${chapter}/note`);
  }

  // ── Progress ─────────────────────────────────────────────────────────────

  getProgress(): Observable<UserProgress[]> {
    return this.http.get<UserProgress[]>(`${this.apiUrl}/progress`);
  }

  completeModule(data: { courseId: number; moduleId: number; score?: number }): Observable<UserProgress> {
    return this.http.post<UserProgress>(`${this.apiUrl}/progress`, data);
  }

  undoModule(moduleId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/progress/modules/${moduleId}`);
  }

  // ── Bookmarks ────────────────────────────────────────────────────────────

  getBookmarks(): Observable<Bookmark[]> {
    return this.http.get<Bookmark[]>(`${this.apiUrl}/bookmarks`);
  }

  addBookmark(data: { bookId: number; chapter: number; verse: number; verseText: string; note?: string }): Observable<Bookmark> {
    return this.http.post<Bookmark>(`${this.apiUrl}/bookmarks`, data);
  }

  removeBookmark(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/bookmarks/${id}`);
  }

  // ── Bible: Busca de versículos ─────────────────────────────────────────────

  searchBibleVerses(query: string, limit = 20): Observable<BibleVerse[]> {
    return this.http.get<BibleVerse[]>(`${this.apiUrl}/bible/search`, { params: { query, limit } });
  }

  // ── AI: Perguntas pontuais por domínio (stateless) ──────────────────────────

  askAi(question: string, domain: 'general' | 'bible' | 'theology' | 'history' | 'eschatology', context?: string): Observable<{ answer: string }> {
    return this.http.post<{ answer: string }>(`${this.apiUrl}/ai/ask`, { question, domain, context });
  }
}
