import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { HomeComponent } from '../app/components/home/home.component';
import { AuctionComponent } from '../app/components/auction/auction.component';
import { AutoRefreshComponent } from '../app/components/timer/timer.component';
import { ActiveJobsComponent} from './components/jobs/activejobs.component';
import { UpdateProfileComponent } from './components/profiles/updateprofile.component';
import { MyAppsComponent } from './components/applications/myapps.component';
import { FormsModule } from '@angular/forms';
import { ImageService } from '../app/components/image/imageservice.component';
import { AlertComponent } from '../app/components/alert/alert.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserModule } from '@angular/platform-browser';
import { MainComponent } from '../app/components/main/main.component';
import { BidDetailComponent } from './components/biddetail/biddetail.component';
import { JobDetailComponent } from './components/jobdetail/jobdetail.component';
import { SecurityGaurdService } from '../app/services/securitygaurd.service';
import { PaymentComponent } from '../app/components/payment/payment.component';
import { LoginComponent } from '../app/components/login/login.component';
import { AdminComponent } from '../app/components/admin/admin.component';
import { NewJobComponent } from '../app/components/newjob/newjob.component';

const routes: Routes = [
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [SecurityGaurdService],
  },
  {
    path: 'auction',
    component: AuctionComponent,
    canActivate: [SecurityGaurdService],
  },
  { path: 'jobs', 
    component: ActiveJobsComponent, 
    canActivate: [SecurityGaurdService] 
  },
  { path: 'newjob', 
  component: NewJobComponent, 
  canActivate: [SecurityGaurdService] 
},
  {
    path: 'myapps',
    component: MyAppsComponent,
    canActivate: [SecurityGaurdService],
  },
  { path: 'alert', component: AlertComponent },
  { path: 'main', component: MainComponent },
  {
    path: 'biddetail',
    component: BidDetailComponent,
    canActivate: [SecurityGaurdService],
  },
  {
    path: 'jobdetail',
    component: JobDetailComponent,
    canActivate: [SecurityGaurdService],
  },
  {
    path: 'updateprofile',
    component: UpdateProfileComponent,
    canActivate: [SecurityGaurdService],
  },
  {
    path: 'makePayment',
    component: PaymentComponent,
    canActivate: [SecurityGaurdService],
  },
  { path: 'login', component: LoginComponent },
  {
    path: 'admin',
    component: AdminComponent,
    canActivate: [SecurityGaurdService],
  },
];

@NgModule({
  declarations: [
    MainComponent,
    BidDetailComponent,
    JobDetailComponent,
    AuctionComponent,
    HomeComponent,
    PaymentComponent,
    ActiveJobsComponent,
    UpdateProfileComponent,
    MyAppsComponent,
    ImageService,
    AlertComponent,
    AdminComponent,
    AutoRefreshComponent,
    LoginComponent,
    NewJobComponent
  ],
  imports: [
    RouterModule.forRoot(routes),
    FormsModule,
    HttpClientModule,
    BrowserModule,
    NgbModule,
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}