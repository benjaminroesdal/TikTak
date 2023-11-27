import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { BehaviorSubject, Observable, from, throwError } from 'rxjs';
import { catchError, delay, filter, switchMap, take, tap } from 'rxjs/operators';
import { Storage } from '@ionic/storage-angular';
import {AuthService} from 'src/app/services/auth.service';
import { ToastService } from '../services/toast.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private storage: Storage, private authService: AuthService, private toastService: ToastService) {}

  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.includes('/RefreshAccessToken') || req.url.includes('/Login')) {
      return next.handle(req);
    }

    return from(this.authService.isTokenExpired()).pipe(
      switchMap(isExpired => {
        if (isExpired) {
          console.log(req)
          return this.handleTokenRefresh(req, next);
        } else {
          return this.addTokenToRequest(req, next);
        }
      }),
      catchError((error) => {
        this.toastService.showToast("Error occurred when refreshing session.");
        return throwError(error);
      })
    );
  }

  private handleTokenRefresh(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      return from(this.authService.RefreshAccessToken()).pipe(
        delay(100),
        switchMap(() => from(this.storage.get('AccessToken'))),
        switchMap((token: string) => {
          console.log(token)
          this.isRefreshing = false;
          this.refreshTokenSubject.next(token);
          return next.handle(this.addToken(req, token));
        })
      );
    } else {
      return this.refreshTokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => {
          console.log(token)
          return next.handle(this.addToken(req, token));
        })
      );
    }
  }

  private addTokenToRequest(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return from(this.storage.get('AccessToken')).pipe(
      switchMap((token: string) => {
        return next.handle(this.addToken(req, token));
      })
    );
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        "ngrok-skip-browser-warning": "69420"
      }
    });
  }

  private addAuthenticationToken(request: HttpRequest<any>): Observable<HttpRequest<any>> {
    return from(this.authService.isTokenExpired()).pipe(
      switchMap(isExpired => {
        if (isExpired) {
          // If the token is expired, refresh it
          return from(this.authService.RefreshAccessToken()).pipe(
            switchMap(() => this.storage.get('AccessToken')) // Get the new access token
          );
        } else {
          // If the token is not expired, get the current token
          return this.storage.get('AccessToken');
        }
      }),
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
      }),
      catchError((error) => {
        // handle error
        this.toastService.showToast("Error occurred when handling the token.");
        return throwError(error);
      })
    );
  }
}