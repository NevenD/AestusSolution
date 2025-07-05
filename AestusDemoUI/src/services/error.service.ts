import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { HttpResponseStatusEnum } from '../entities/enums';
import { ToastrService } from 'ngx-toastr';
@Injectable({
  providedIn: 'root',
})
export class ErrorService {
  constructor(private _toastr: ToastrService) {}

  public handleHttpResponseError(error: HttpErrorResponse): Observable<never> {
    let message: string;

    switch (error.status) {
      case HttpResponseStatusEnum.NotFound:
        message = `Greška: ${error.status}. Resurs nije pronađen.`;
        break;
      case HttpResponseStatusEnum.BadRequest:
        message = `Greška: ${error.status}. Pogrešan zahtjev.`;
        break;
      case HttpResponseStatusEnum.InternalServerError:
        message = `Greška: ${error.status}. Interna pogreška poslužitelja.`;
        break;
      case HttpResponseStatusEnum.UnknownError:
      default:
        message = 'Došlo je do greške prilikom komunikacije sa serverom!';
        break;
    }

    this._toastr.error(message);
    return throwError(() => error);
  }

  public handleValidationError(errors: string[]): void {
    if (errors.length > 1) {
      for (const error of errors) {
        this._toastr.error(`${error}`, 'Došlo je do greške!');
      }
    } else {
      this._toastr.error(`${errors[0]}`, 'Došlo je do greške!');
    }
  }
}
