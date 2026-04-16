import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BankAccount, CreateBankAccountRequest, UpdateBankAccountRequest } from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class BankAccountService {
  private http = inject(HttpClient);

  getAll() {
    return this.http.get<BankAccount[]>(`${API}/BankAccounts`);
  }

  create(body: CreateBankAccountRequest) {
    return this.http.post<BankAccount>(`${API}/BankAccounts`, body);
  }

  update(id: number, body: UpdateBankAccountRequest) {
    return this.http.put<BankAccount>(`${API}/BankAccounts/${id}`, body);
  }

  delete(id: number) {
    return this.http.delete(`${API}/BankAccounts/${id}`);
  }

  toggleSharedBudget(id: number) {
    return this.http.patch<BankAccount>(`${API}/BankAccounts/${id}/toggle-shared`, {});
  }
}
