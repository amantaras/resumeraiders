import { Injectable } from '@angular/core';
import { ApplicationInsights } from '@microsoft/applicationinsights-web';
import { environment } from './environments/environment.prod';
@Injectable({
  providedIn: 'root',
})
export class AppInsightsService {
  instance: ApplicationInsights;
  constructor() {
    this.instance = new ApplicationInsights({
      config: {
        instrumentationKey: environment.appInsights.instrumentationKey,
      },
    });
    this.instance.loadAppInsights();
    this.instance.trackPageView();
  }
}
