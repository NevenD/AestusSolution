import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Controller } from '../entities/enums';
import { DashboardDto } from '../entities/models';

@Injectable({
  providedIn: 'root',
})
export class DashboardService {
  constructor(private _api: ApiService) {}

  getDashboardData(): Observable<DashboardDto> {
    return this._api._getData<DashboardDto>(
      `${Controller.DashboardController}`
    );
  }
}
