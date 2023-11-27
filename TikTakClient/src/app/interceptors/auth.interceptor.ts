import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { catchError, filter, switchMap, take, tap } from 'rxjs/operators';
import { Storage } from '@ionic/storage-angular';
import {AuthService} from 'src/app/services/auth.service';
import { ToastService } from '../services/toast.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private storage: Storage, private authService: AuthService, private toastService: ToastService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.includes('/RefreshAccessToken')) {
      // If it's the refresh token call, bypass the interceptor logic and forward the request
      return next.handle(req);
    }
    return this.authService.tokenRefreshInProgress.pipe(
      filter(inProgress => !inProgress),
      take(1),
      switchMap(() => this.addAuthenticationToken(req)),
      switchMap(authReq => next.handle(authReq)),
      catchError((error) => {
        // handle the error
        this.toastService.showToast("Error occurred when refreshing session.")
        return throwError(error);
      })
    );
  }

  private addAuthenticationToken(request: HttpRequest<any>): Observable<HttpRequest<any>> {
    // Convert the Promise returned by this.storage.get into an Observable
    return from(this.storage.get('AccessToken')).pipe(
      switchMap(token => {
        // If there's a token, clone the request and add the token to the headers
        if (token) {
          return [request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`,
              "ngrok-skip-browser-warning": "69420"

            }
          })];
        }
        // If there's no token, return the original request
        return [request];
      })
    );
  }
}