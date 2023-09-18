import { Component, OnInit } from '@angular/core';
import { AlertService } from '../alert/alert.service';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { AppInsightsService } from 'src/app/app-insights.service';

@Component({
  selector: 'activejobs',
  templateUrl: './activejobs.component.html',
})
export class ActiveJobsComponent {
  jobs: any;
  job: any;
  loading: boolean = false;

  constructor(
    private httpClient: HttpClient,
    private alertService: AlertService,
    private router: Router,
    private appInsights: AppInsightsService
  ) {}

  ngOnInit() {
    this.getActiveJobs();
  }

  public rowSelected(job: any) {
    console.log("Inside row selected");
    this.job = job;
    console.log(job);
    this.router.navigate(['/jobdetail'], {
      queryParams: { job: job.JobID }
    });
  }

  public getActiveJobs() {
    this.loading = true;
    this.httpClient.get(environment.jobAPI).subscribe(
      (res) => {
        this.jobs = res;
        console.log('Active jobs are loaded');
        console.log(res);
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.alertService.add({
          type: 'danger',
          message: 'Some error occured, please contact Administrator',
        });
        this.loading = false;
        this.appInsights.instance.trackException(err);
      }
    );
    this.appInsights.instance.trackEvent({ name: 'LoadedActiveJobs' });
  }
}
