import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Job } from '../jobs/job';
import { JobDetail } from './jobdetail';
import { ActivatedRoute } from '@angular/router';
import { AlertComponent } from '../alert/alert.component';
import { AlertService } from '../alert/alert.service';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';



@Component({
  selector: 'jobdetail',
  templateUrl: './jobdetail.component.html',
  providers: [AlertService, AlertComponent],
})
export class JobDetailComponent implements OnInit {
  imageFile: any;
  bids: any;
  jobs: any;
  JobID!: number;
/*  job: Job = new Job(
    0,
    '',
    '',
    '',
    0,
    0,
    0,
    '',
    '',
    ''
  );

  jobDetail: JobDetail = new JobDetail(
    0,
    '',
    '',
    '',
    0,
    0,
    0,
    '',
    '',
    ''
    );
*/
  jobDetail: any;

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
      this.JobID = params.job;
      console.log("Job ID is "+ this.JobID);
      this.loadJobDetailByJobId();
    });
  }

  public refresh() {
    if (this.elapsedTime < 0 && this.elapsedTime != -1) {
      this.alertService.add({
        type: 'warning',
        message: 'Auction closed now!',
      });
    }
  }
 
  public loadJobDetailByJobId() {
    console.log('Loading Job Detail by Job ID ' + this.JobID);
    this.httpClient
      .get(environment.jobAPI + '/' + this.JobID)
      .subscribe(
        (res) => {
         // var resObj: { [index: string]: any } = res;

          console.log('Response is ' + res);
         // console.log(resObj[0]);
          this.jobDetail = res;
          console.log('Job title is ' + this.jobDetail.JobTitle);
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
    this.appInsights.instance.trackEvent({ name: 'LoadedJobDetailbyJobID' });
  }

  /*
  public submitBid() {
   this.loadMakeOffer = true;
    console.log('submitting Job');
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
*/
    //console.log('Job is applied');
    //this.appInsights.instance.trackEvent({ name: 'SubmittedBid' });
  //}
}
