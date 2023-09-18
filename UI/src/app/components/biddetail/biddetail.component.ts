import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Auction } from '../auction/auction';
import { BidDetail } from './biddetail';
import { ActivatedRoute } from '@angular/router';
import { AlertComponent } from '../alert/alert.component';
import { AlertService } from '../alert/alert.service';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';

@Component({
  selector: 'biddetail',
  templateUrl: './biddetail.component.html',
  providers: [AlertService, AlertComponent],
})
export class BidDetailComponent implements OnInit {
  imageFile: any;
  bids: any;
  auctions: any;
  auctionId!: number;
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
  bidDetail: BidDetail = new BidDetail(1, 1, 0, '', new Date(), '');
  loadingPrevBids!: boolean;
  loadMakeOffer!: boolean;
  elapsedTime: number = -1;
  constructor(
    private authorizationService: AuthorizationService,
    private httpClient: HttpClient,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private _sanitizer: DomSanitizer,
    private appInsights: AppInsightsService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      this.auctionId = params.auction;
      this.loadBidByAuctionId();
      this.loadPreviousBids();
      this.loadMakeOffer = false;
    });
  }

  public refresh() {
    if (this.elapsedTime < 0 && this.elapsedTime != -1) {
      this.loadMakeOffer = true;
      this.alertService.add({
        type: 'warning',
        message: 'Auction closed now!',
      });
    }
  }
  public loadPreviousBids() {
    this.loadingPrevBids = true;
    this.httpClient
      .get(environment.bidAPI + '/bid?auctionID=' + this.auctionId)
      .subscribe(
        (res) => {
          console.log('Response is ' + res);
          this.bids = res;
          this.loadingPrevBids = false;
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occured, please contact Administrator',
          });
          this.loadingPrevBids = false;
          this.appInsights.instance.trackException(err);
        }
      );
    this.loadingPrevBids = false;
    this.appInsights.instance.trackEvent({ name: 'LoadedPreviousBids' });
  }

  public blobToFile = (theBlob: Blob, fileName: string): File => {
    var b: any = theBlob;
    console.log('Blob to file' + theBlob);
    //A Blob() is almost a File() - it's just missing the two properties below which we will add
    b.lastModifiedDate = new Date();
    b.name = fileName;
    //Cast to a File() type
    return <File>theBlob;
  };

  public loadBidByAuctionId() {
    this.httpClient
      .get(environment.auctionAPI + '/auctionById/' + this.auctionId)
      .subscribe(
        (res) => {
          var resObj: { [index: string]: any } = res;

          console.log('Response is ' + res);
          this.auction = resObj[0];
          var currentDateTime = new Date();
          var dt2 = new Date(this.auction.auctionDate);
          console.log('Today date is ' + currentDateTime);
          console.log('Auction date is' + dt2);
          var expiryDate = dt2;
          console.log('hours are ' + this.auction.activeInHours);
          expiryDate.setHours(
            expiryDate.getHours() + this.auction.activeInHours
          );
          console.log('Expiry time is' + expiryDate);
          var diffMs = expiryDate.getTime() - currentDateTime.getTime();
          diffMs = diffMs / 60000;
          console.log('Total minutes left is' + diffMs);
          if (diffMs < 0) {
            this.loadMakeOffer = true;
            this.alertService.add({
              type: 'warning',
              message: 'Auction closed now!',
            });
          } else {
            this.elapsedTime = diffMs;
            this.loadMakeOffer = false;
            console.log('Total elapsed time left is' + this.elapsedTime);
          }
          console.log('Total minutes left is ' + diffMs);
          this.imageFile = this._sanitizer.bypassSecurityTrustResourceUrl(
            'data:image/jpg;base64,' + resObj[0].image
          );
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
    this.appInsights.instance.trackEvent({ name: 'LoadedBidbyAuctionID' });
  }

  public getActiveBids() {
    this.httpClient.get(environment.auctionAPI + '/auctions').subscribe(
      (res) => {
        this.auctions = res;
        console.log('Active auctions are loaded');
        console.log(res);
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
    this.appInsights.instance.trackEvent({ name: 'LoadedActiveBids' });
  }

  public submitBid() {
    this.loadMakeOffer = true;
    console.log('submitting bid');
    this.bidDetail.userId = this.authorizationService.getLoggedUser().UserId;
    this.bidDetail.userName =
      this.authorizationService.getLoggedUser().UserName;
    console.log(this.auction);
    this.httpClient
      .post(
        environment.bidAPI +
          '/bid?bidAmount=' +
          this.bidDetail.bidAmount +
          '&userID=' +
          this.bidDetail.userId +
          '&auctionID=' +
          this.auction.idAuction +
          '&userName=' +
          this.bidDetail.userName,
        this.auction
      )
      .subscribe(
        (res) => {
          this.alertService.add({
            type: 'success',
            message: 'Bid is made successfully',
          });
          this.loadPreviousBids();
          this.loadMakeOffer = false;
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occured, please contact Administrator',
          });
          this.loadMakeOffer = false;
          this.appInsights.instance.trackException(err);
        }
      );

    console.log('Bid is made');
    this.appInsights.instance.trackEvent({ name: 'SubmittedBid' });
  }
}
