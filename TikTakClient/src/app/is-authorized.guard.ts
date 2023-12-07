import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import {AuthService} from 'src/app/services/auth.service';
import {StorageService} from 'src/app/services/storage.service';

@Injectable({
  providedIn: 'root'
})
export class IsAuthorizedGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router, private storageService: StorageService){
    
  }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {
    return this.storageService.init().then(() => {
      return this.authService.isTokenExpired().then(isExpired => {
        if (!isExpired) {
          return true;
        } else {
          return this.storageService.get('RefreshToken').then(refreshToken => {
            if (!refreshToken) {
              this.router.navigate(['tabs/tab1']);
              return false;
            } else {
              // Here, we wait for the refresh token process to complete
              return this.authService.RefreshAccessToken().then(() => {
                return true;
              }).catch(() => {
                this.router.navigate(['tabs/tab1']);
                return false;
              });
            }
          });
        }
      });
    });
  }
  
}
