import { Component, Injectable, Inject } from '@angular/core';
import { Auction } from './auction';
import { ImageService } from '../image/imageservice.component';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AlertService } from '../alert/alert.service';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';

@Component({
  selector: 'auction',
  templateUrl: './auction.component.html',
  providers: [ImageService, AlertService],
})
@Injectable({
  providedIn: 'root',
})
export class AuctionComponent {
  loadingAuction: boolean = false;
  auction: Auction = new Auction(
    0,
    '',
    '',
    0,
    0,
    new Date(),
    '',
    1,
    '',
    true,
    '',
    '',
    '',
    false
  );
  

  constructor(
    private authorizationService: AuthorizationService,
    private imageService: ImageService,
    private httpClient: HttpClient,
    private alertService: AlertService,
    private appInsights: AppInsightsService
  ) {}

  public resetFields() {
    this.auction = new Auction(
      0,
      '',
      '',
      0,
      0,
      new Date(),
      '',
      1,
      '',
      true,
      '',
      '',
      '',
      false
    );
  }

  receiveImageEventHandler($event: string | number | boolean) {
    console.log('got the event');
    this.auction.image = encodeURIComponent($event);
    console.log('set the image');
  }

  public createAuction() {
    this.loadingAuction = true;
    this.auction.userId = this.authorizationService.getLoggedUser().UserId;
    this.auction.userName = this.authorizationService.getLoggedUser().UserName;
    const headerSettings: { [name: string]: string | string[] } = {};
    headerSettings['Content-Type'] = 'application/json';
    const newHeader = new HttpHeaders(headerSettings);

    this.httpClient
      .post(environment.auctionAPI + '/auctions', this.auction, {
        headers: newHeader,
      })
      .subscribe(
        (res) => {
          var id = res;
          console.log(id);
          this.loadingAuction = false;
          this.alertService.add({
            type: 'success',
            message: 'Auction created successfully',
          });
          //hthis.imageService.processFile(this.auction.image);

          this.resetFields();
        },
        (err) => {
          this.loadingAuction = false;
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occured, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );

    console.log('Auction is created');

    this.appInsights.instance.trackEvent({ name: 'CreatedAuction' });
  }
}
