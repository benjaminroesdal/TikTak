import { Injectable } from '@angular/core';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { BehaviorSubject, Observable, from, throwError } from 'rxjs';
import { catchError, delay, filter, switchMap, take, tap } from 'rxjs/operators';
import { Storage } from '@ionic/storage-angular';
import {AuthService} from 'src/app/services/auth.service';
import { ToastService } from '../services/toast.service';
import { environment } from 'src/environments/environment';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private storage: Storage, private authService: AuthService, private toastService: ToastService) {}
  private apiKey = environment.backend.apiKey;
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.url.includes('/RefreshAccessToken') || req.url.includes('/Login')) {
      const apiReq = this.addApiKey(req);
      return next.handle(apiReq);
    }

    return from(this.authService.isTokenExpired()).pipe(
      switchMap(isExpired => {
        if (isExpired) {
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
        delay(300),
        switchMap(() => from(this.storage.get('AccessToken'))),
        switchMap((token: string) => {
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

  private addApiKey(request: HttpRequest<any>): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        "X-Api-Key": this.apiKey,
        "ngrok-skip-browser-warning": "69420"
      }
    });
  }

  private addToken(request: HttpRequest<any>, token: string): HttpRequest<any> {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
        "X-Api-Key": this.apiKey,
        "ngrok-skip-browser-warning": "69420"
      }
    });
  }
}