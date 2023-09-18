import { Component, Inject } from '@angular/core';
import { AuthorizationService } from '../app/components/login/authorization.service';
import { AppInsightsService } from '../app/app-insights.service';
import { Router } from '@angular/router';
import { faHouseUser, faEnvelope, faPhone, faPrint } from '@fortawesome/free-solid-svg-icons';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'Resume Raiders';

  loggedIn: boolean = false;
  public userInfo: any = null;
  public isIframe: boolean = false;
  public useremail: any;
  faHouseUser = faHouseUser;
  faEnvelop = faEnvelope;
  faPhone = faPhone;
  faPrint = faPrint;

  constructor(
    private router: Router,
    public authorizationService: AuthorizationService,
    private appInsights: AppInsightsService
  ) {}

  logOut() {
    this.authorizationService.logout();
    this.router.navigate(['/login']);
  }
}