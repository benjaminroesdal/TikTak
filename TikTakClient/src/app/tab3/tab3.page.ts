import { Component } from '@angular/core';
import {AuthService} from 'src/app/services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-tab3',
  templateUrl: 'tab3.page.html',
  styleUrls: ['tab3.page.scss']
})
export class Tab3Page {

  constructor(private authService: AuthService, private router: Router) {}

  async logout() {
    await this.authService.Logout().then(() => {
      this.router.navigate(['tabs/tab1']);
    });
  }
}
