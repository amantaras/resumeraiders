import { Component, Injectable } from '@angular/core';
import { ImageService } from '../image/imageservice.component';
import { HttpClient } from '@angular/common/http';
import { AlertComponent } from '../alert/alert.component';
import { ActivatedRoute } from '@angular/router';
import { AlertService } from '../alert/alert.service';
import { environment } from '../../environments/environment';
import { Auction } from '../auction/auction';
import { AuctionPayment } from './auctionpayment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';

@Component({
  selector: 'payment',
  templateUrl: './payment.component.html',
  providers: [ImageService, AlertService, AlertComponent],
})
@Injectable({
  providedIn: 'root',
})
export class PaymentComponent {
  loadingPayment: boolean = false;
  auctionPayment: AuctionPayment = new AuctionPayment(0, '', '', '', 0, 0);
  auction: Auction = new Auction(
    0,
    '',
    '',
    0,
    0,
    new Date(),
    '',
    0,
    '',
    false,
    '',
    '',
    '',
    false
  );

  auctionId: number = 0;
  constructor(
    private authorizationService: AuthorizationService,
    private imageService: ImageService,
    private httpClient: HttpClient,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private appInsights: AppInsightsService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      console.log(params);
      this.auctionId = params.auction;
      this.auctionPayment.creditCardNo = '4111 1111 1111 1111';
      this.auctionPayment.month = 11;
      var d = new Date();
      var year = d.getFullYear();
      var month = d.getMonth();
      var day = d.getDate();
      var newDate = new Date(year + 1, month, day);
      this.auctionPayment.year = newDate.getFullYear();
      this.loadAuctionDetailbyId();
    });
  }

  public loadAuctionDetailbyId() {
    this.loadingPayment = true;
    console.log('Loading payment animation:' + this.loadingPayment);
    this.httpClient
      .get(environment.auctionAPI + '/auctionById/' + this.auctionId)
      .subscribe(
        (res) => {
          var resObj: { [index: string]: any } = res;
          console.log('Response is ' + res);
          this.auction = resObj[0];
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occured, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );
    this.loadingPayment = false;
    this.appInsights.instance.trackEvent({
      name: 'LoadedAuctionDetailByIDForPayment',
    });
  }

  public makePayment() {
    this.loadingPayment = true;
    console.log('Loading payment animation:' + this.loadingPayment);
    this.auction.userId = this.authorizationService.getLoggedUser().UserId;
    this.auction.userName = this.authorizationService.getLoggedUser().UserName;

    this.auctionPayment.idAuction = this.auctionId;
    this.auctionPayment.month = Number(this.auctionPayment.month);
    this.auctionPayment.year = Number(this.auctionPayment.year);
    this.auctionPayment.bidUser =
      this.authorizationService.getLoggedUser().UserId;

    this.httpClient
      .post(environment.paymentAPI + '/auctionpayments', this.auctionPayment)
      .subscribe(
        (res) => {
          var id = res;
          console.log(id);
          this.alertService.add({
            type: 'success',
            message: 'Payment is done successfully',
          });
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occured, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );

    this.loadingPayment = false;
    console.log('Auction Payment is completed');
    this.appInsights.instance.trackEvent({ name: 'MadePayment' });
  }
}
