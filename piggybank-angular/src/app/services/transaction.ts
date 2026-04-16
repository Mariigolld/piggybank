import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Category, CreateCategoryRequest, CreateTransactionRequest, Transaction } from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class TransactionService {
  private http = inject(HttpClient);

  getRecent(limit = 20) {
    return this.http.get<Transaction[]>(`${API}/Transactions?pageSize=${limit}`);
  }

  getByAccount(accountId: number) {
    return this.http.get<Transaction[]>(`${API}/Transactions?bankAccountId=${accountId}`);
  }

  create(body: CreateTransactionRequest) {
    return this.http.post<Transaction>(`${API}/Transactions`, body);
  }

  delete(id: number) {
    return this.http.delete(`${API}/Transactions/${id}`);
  }

  getCategories() {
    return this.http.get<Category[]>(`${API}/Categories`);
  }

  createCategory(body: CreateCategoryRequest) {
    return this.http.post<Category>(`${API}/Categories`, body);
  }

  deleteCategory(id: number) {
    return this.http.delete(`${API}/Categories/${id}`);
  }
}
