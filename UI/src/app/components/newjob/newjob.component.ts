import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Job } from '../jobs/job';
import { NewJob } from './newjob';
import { ActivatedRoute } from '@angular/router';
import { AlertComponent } from '../alert/alert.component';
import { AlertService } from '../alert/alert.service';
import { DomSanitizer } from '@angular/platform-browser';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';
import { AuthorizationService } from '../login/authorization.service';
import { NgbTooltipModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'newjob',
  templateUrl: './newjob.component.html',
  providers: [AlertService, AlertComponent, NgbTooltipModule],

})
export class NewJobComponent implements OnInit {
  imageFile: any;
  bids: any;
  jobs: any={};
  JobID!: number;
  jobDetail: any;

  elapsedTime: number = -1;
  constructor(
    private authorizationService: AuthorizationService,
    private httpClient: HttpClient,
    private alertService: AlertService,
    private route: ActivatedRoute,
    private _sanitizer: DomSanitizer,
    private appInsights: AppInsightsService
  ) { }


  ngOnInit() {
    this.route.queryParams.subscribe((params) => {


    });
  }

  public generateTag(title: String) {
    console.log("Generating Tag");
    this.jobs.Tags = "Generating Tag .......";
    this.httpClient
      .post(environment.getTagsAPI, title,{responseType: 'text'})
      .subscribe(
        (res) => {
          console.log('Response is ' + res);
          this.jobs.Tags = res;
          /* this.alertService.add({
            type: 'success',
            message: 'Tag Generated Successfully',
          }); */
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occurred, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );
  }


  public generateJD(tag: String) {
    console.log("Generating Description");

    //texttest: '';
    this.jobDetail =
     {
      "JobTitle": this.jobs.jobTitle ,
      "ContractType": "Temporary",
      "ApplicationProcess": "apply in the web page and subscribe by email",
      "ReportsTo" : "hiringmanager@company.com",
      //"Location" : "UAE",
      "Location" :  this.jobs.Location,
      //"Benefits" : "competitive salary, working from home",
      "Benefits" : this.jobs.Benefits,
      "CompanyAbout" :"Microsoft Corporation is an American multinational technology company with headquarters in Redmond, Washington. It develops, manufactures, licenses, supports, and sells computer software, consumer electronics, personal computers, and related services." ,
      "Tags" : tag
    };

    this.jobs.Description = "Generating Description ...";
    this.httpClient
      .post(environment.getJobDescriptionAPI, JSON.stringify(this.jobDetail),{responseType: 'text'})
      .subscribe(
        (res) => {
          console.log('Response is ' + res);
          this.jobs.Description = res;
          /* this.alertService.add({
            type: 'success',
            message: 'Description Generated Successfully',
          });*/
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occurred, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );
  }

  public postNewJob(job: any) {
    console.log("Generating New Job");

    this.jobDetail =
     {
      "JobTitle": this.jobs.jobTitle ,
      "Description": this.jobs.Description,
      "ContractType": "Temporary",
      "ApplicationProcess": "apply in the web page and subscribe by email",
      "ReportsTo" : "hiringmanager@company.com",
      "Location" :  this.jobs.Location,
      "Salary": 100000,
      "Benefits" : this.jobs.Benefits,
      "CompanyID": 27,
      "CategoryID": 27,
      "CompanyAbout" :"Microsoft Corporation is an American multinational technology company with headquarters in Redmond, Washington. It develops, manufactures, licenses, supports, and sells computer software, consumer electronics, personal computers, and related services." ,
      "Tags" : this.jobs.Tags
    };

    //this.jobs.Description = "Generating Description ...";
    this.httpClient
      .post(environment.PostNewJob, JSON.stringify(this.jobDetail),{responseType: 'text'})
      .subscribe(
        (res) => {
          console.log('Response is ' + res);
          this.alertService.add({
            type: 'success',
            message: 'Position Posted Successfully',
          });
        },
        (err) => {
          console.log(err);
          this.alertService.add({
            type: 'danger',
            message: 'Some error occurred, please contact Administrator',
          });
          this.appInsights.instance.trackException(err);
        }
      );
  }


}


