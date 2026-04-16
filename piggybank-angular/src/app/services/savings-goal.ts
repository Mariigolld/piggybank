import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AddContributionRequest, CreateSavingsGoalRequest, SavingsGoal } from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class SavingsGoalService {
  private http = inject(HttpClient);

  getAll(sharedAccountId: number, includeArchived = false) {
    return this.http.get<SavingsGoal[]>(
      `${API}/SharedAccounts/${sharedAccountId}/SavingsGoals?includeArchived=${includeArchived}`
    );
  }

  create(sharedAccountId: number, body: CreateSavingsGoalRequest) {
    return this.http.post<SavingsGoal>(`${API}/SharedAccounts/${sharedAccountId}/SavingsGoals`, body);
  }

  addContribution(sharedAccountId: number, goalId: number, body: AddContributionRequest) {
    return this.http.post(
      `${API}/SharedAccounts/${sharedAccountId}/SavingsGoals/${goalId}/contributions`, body
    );
  }

  archive(sharedAccountId: number, goalId: number) {
    return this.http.patch(`${API}/SharedAccounts/${sharedAccountId}/SavingsGoals/${goalId}/archive`, {});
  }

  delete(sharedAccountId: number, goalId: number) {
    return this.http.delete(`${API}/SharedAccounts/${sharedAccountId}/SavingsGoals/${goalId}`);
  }
}
