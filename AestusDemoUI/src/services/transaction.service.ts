import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Controller } from '../entities/enums';
import { TransactionDto } from '../entities/models';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  constructor(private _api: ApiService) {}

  getTransactions(): Observable<TransactionDto[]> {
    return this._api._getData<TransactionDto[]>(
      `${Controller.TransactionsController}`
    );
  }
}
