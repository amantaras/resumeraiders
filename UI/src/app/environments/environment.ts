// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: true,
  identityAPI: "http://localhost:5032/api/identity",
  auctionAPI: "http://localhost:3000/api/auction",
  jobAPI: "https://resumeraidersapi.azurewebsites.net/api/jobs",
  bidAPI:  "http://localhost:9090/api/bid",
  paymentAPI:  "http://localhost:5000",
  getTagsAPI: "https://resumeraiderssmartfunctions.azurewebsites.net/api/generatetags",
  getJobDescriptionAPI: "https://resumeraiderssmartfunctions.azurewebsites.net/api/GenerateJobDescription",
  PostNewJob:"https://resumeraidersapi.azurewebsites.net/api/jobs",
  appInsights: {
    instrumentationKey: "bb020e71-7aac-4c00-a18e-2160307c4bdc",
  },
};


/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
