import { Component, OnInit, Inject, Injectable } from '@angular/core';
import { AlertService } from '../alert/alert.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';

@Component({
  selector: 'updateprofile',
  templateUrl: './updateprofile.component.html',
})
@Injectable({
  providedIn: 'root',
})
export class UpdateProfileComponent {
  auctions: any;
  userId: string;
  loading: boolean = false;

  constructor(
    private httpClient: HttpClient,
    private alertService: AlertService,
    private router: Router,
    private authorizationService: AuthorizationService,
    private appInsights: AppInsightsService
  ) {
    this.userId = this.authorizationService.getLoggedUser().UserId;
  }

  ngOnInit() {
    this.getMyWinningBids();
  }

  public getMyWinningBids() {
    this.loading = true;
    console.log(this.userId);
    this.httpClient
      .get(
        environment.auctionAPI + '/auctionsForPaymentsByUserId/' + this.userId
      )
      .subscribe(
        (res) => {
          this.auctions = res;
          console.log('Winning Bids are loaded');
          console.log(res);
        },
        (err) => {
          console.log(err);
          this.alertService.add({ type: 'danger', message: err });
          this.appInsights.instance.trackException(err);
        }
      );
    this.loading = false;
    this.appInsights.instance.trackEvent({ name: 'LoadedWinningBids' });
  }

  public makePayment(auction: { idauction: any }) {
    console.log(auction);
    this.router.navigate(['/makePayment'], {
      queryParams: { auction: auction.idauction },
    });
  }
}
