import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateSharedBudgetRequest, SharedBudget } from '../models';
import { catchError, of } from 'rxjs';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class SharedBudgetService {
  private http = inject(HttpClient);

  getByMonth(sharedAccountId: number, year: number, month: number) {
    return this.http
      .get<SharedBudget>(`${API}/SharedAccounts/${sharedAccountId}/SharedBudgets/${year}/${month}`)
      .pipe(catchError(() => of(null)));
  }

  save(sharedAccountId: number, body: CreateSharedBudgetRequest) {
    return this.http.post<SharedBudget>(
      `${API}/SharedAccounts/${sharedAccountId}/SharedBudgets`, body
    );
  }

  delete(sharedAccountId: number, budgetId: number) {
    return this.http.delete(
      `${API}/SharedAccounts/${sharedAccountId}/SharedBudgets/${budgetId}`
    );
  }
}
