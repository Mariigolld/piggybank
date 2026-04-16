import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CreateRecurringBillRequest, RecurringBill } from '../models';

const API = 'http://localhost:5235/api';

@Injectable({ providedIn: 'root' })
export class RecurringBillService {
  private http = inject(HttpClient);

  getAll(sharedAccountId: number) {
    return this.http.get<RecurringBill[]>(`${API}/SharedAccounts/${sharedAccountId}/RecurringBills`);
  }

  create(sharedAccountId: number, body: CreateRecurringBillRequest) {
    return this.http.post<RecurringBill>(`${API}/SharedAccounts/${sharedAccountId}/RecurringBills`, body);
  }

  markPaid(sharedAccountId: number, billId: number, year: number, month: number) {
    return this.http.post(
      `${API}/SharedAccounts/${sharedAccountId}/RecurringBills/${billId}/payments/${year}/${month}/paid`,
      { note: null }
    );
  }

  markUnpaid(sharedAccountId: number, billId: number, year: number, month: number) {
    return this.http.post(
      `${API}/SharedAccounts/${sharedAccountId}/RecurringBills/${billId}/payments/${year}/${month}/unpaid`,
      {}
    );
  }

  deactivate(sharedAccountId: number, billId: number) {
    return this.http.patch(
      `${API}/SharedAccounts/${sharedAccountId}/RecurringBills/${billId}/deactivate`, {}
    );
  }

  reactivate(sharedAccountId: number, billId: number) {
    return this.http.patch(
      `${API}/SharedAccounts/${sharedAccountId}/RecurringBills/${billId}/reactivate`, {}
    );
  }

  delete(sharedAccountId: number, billId: number) {
    return this.http.delete(`${API}/SharedAccounts/${sharedAccountId}/RecurringBills/${billId}`);
  }
}
