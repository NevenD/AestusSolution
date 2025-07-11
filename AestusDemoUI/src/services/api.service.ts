import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { ErrorService } from './error.service';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  constructor(private _http: HttpClient, private _errorService: ErrorService) {}

  httpOptions = {
    headers: new HttpHeaders({
      Accept: 'application/json',
      'Content-Type': 'application/json',
    }),
  };

  _getData<T>(urlPath: string, params = {}): Observable<T> {
    return this._http
      .get<T>(`${environment.url}${urlPath}`, {
        headers: this.httpOptions.headers,
        params,
      })
      .pipe(
        catchError((error) =>
          this._errorService.handleHttpResponseError(error)
        ),
        map((response: T) => {
          return response;
        })
      );
  }
}
