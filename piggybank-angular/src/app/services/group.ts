import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {
  Group, CreateGroupRequest, GroupExpense, CreateGroupExpenseRequest,
  GroupSettlement, CreateSettlementRequest
} from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class GroupService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<Group[]>(`${API}/Groups`);
  }

  getById(id: number) {
    return this.http.get<Group>(`${API}/Groups/${id}`);
  }

  create(body: CreateGroupRequest) {
    return this.http.post<Group>(`${API}/Groups`, body);
  }

  delete(id: number) {
    return this.http.delete(`${API}/Groups/${id}`);
  }

  joinByCode(inviteCode: string) {
    return this.http.post<Group>(`${API}/Groups/join/${inviteCode}`, {});
  }

  getExpenses(groupId: number) {
    return this.http.get<GroupExpense[]>(`${API}/GroupExpenses/group/${groupId}`);
  }

  createExpense(body: CreateGroupExpenseRequest) {
    return this.http.post<GroupExpense>(`${API}/GroupExpenses`, body);
  }

  createExpenseSplit(body: CreateGroupExpenseRequest) {
    return this.http.post<GroupExpense>(`${API}/GroupExpenses/split`, body);
  }

  deleteExpense(id: number) {
    return this.http.delete(`${API}/GroupExpenses/${id}`);
  }

  getSettlements(groupId: number) {
    return this.http.get<GroupSettlement[]>(`${API}/Groups/${groupId}/settlement-history`);
  }

  createSettlement(body: CreateSettlementRequest) {
    return this.http.post<GroupSettlement>(`${API}/Groups/${body.groupId}/settle`, body);
  }

  markShareAsPaid(shareId: number) {
    return this.http.post(`${API}/GroupExpenses/shares/${shareId}/mark-paid`, {});
  }
}
