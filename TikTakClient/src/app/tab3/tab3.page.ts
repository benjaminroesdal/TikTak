import { Component } from '@angular/core';
import {AuthService} from 'src/app/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import * as jwt_decode from 'jwt-decode';
import { StorageService } from '../services/storage.service';

@Component({
  selector: 'app-tab3',
  templateUrl: 'tab3.page.html',
  styleUrls: ['tab3.page.scss']
})
export class Tab3Page {
  public imgUrl: string = "";
  public name: string = "Username";
  constructor(private authService: AuthService, private router: Router, private storage: StorageService) {}

  async ngOnInit() {
    this.storage.get('AccessToken').then(e => {
      const jwtDec = jwt_decode.jwtDecode<User>(e);
      this.imgUrl = jwtDec.profile_img;
      this.name = jwtDec.user_email;
    })
  }

  async logout() {
    await this.authService.Logout().then(() => {
      this.router.navigate(['tabs/tab1']);
    });
  }
}

export interface User {
  profile_img: string;
  user_email: string;
}
