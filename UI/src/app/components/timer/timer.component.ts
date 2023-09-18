import {
  Component,
  OnInit,
  Output,
  Input,
  EventEmitter,
  ChangeDetectorRef,
} from '@angular/core';
import { Subscription, Observable, timer } from 'rxjs';
import * as moment from 'moment';
@Component({
  selector: 'timer-auto-refresh',
  templateUrl: './timer.component.html',
})
export class AutoRefreshComponent implements OnInit {
  private subscription: Subscription = new Subscription();
  @Output() TimerExpired: EventEmitter<any> = new EventEmitter<any>();
  @Input() SearchDate: moment.Moment = moment();
  @Input() ElapsTime: number = 0;
  searchEndDate: moment.Moment;
  remainingTime!: number;
  days!: number;
  hours!: number;
  minutes!: number;
  seconds!: number;
  total_hours!: number;
  total_minutes!: number;
  total_seconds!: number;
  displayText!: string;
  everySecond: Observable<number> = timer(0, 1000);
  constructor(private ref: ChangeDetectorRef) {
    this.searchEndDate = this.SearchDate.add(this.ElapsTime, 'minutes');
  }
  ngOnInit() {
    this.subscription = this.everySecond.subscribe((seconds) => {
      var currentTime: moment.Moment = moment();
      this.remainingTime = this.searchEndDate.diff(currentTime);
      this.remainingTime = this.remainingTime / 1000;
      if (this.remainingTime <= 0) {
        this.SearchDate = moment();
        this.searchEndDate = this.SearchDate.add(this.ElapsTime, 'minutes');
        this.TimerExpired.emit();
      } else {
        this.days = Math.floor(this.remainingTime / 60 / 60 / 24);
        this.hours = Math.floor(this.remainingTime / 60 / 60);
        this.minutes = Math.floor(this.remainingTime / 60);
        this.seconds = Math.floor(this.remainingTime - this.minutes * 60);

        this.total_seconds = Math.floor(this.remainingTime - this.minutes * 60);
        this.total_minutes = Math.floor(this.remainingTime / 60);
        this.total_hours = Math.floor(this.total_minutes / 60);
        this.days = Math.floor(this.total_hours / 24);

        this.seconds = this.total_seconds % 60;
        this.minutes = this.total_minutes % 60;
        this.hours = this.total_hours % 24;

        this.displayText =
          this.days +
          ' days ' +
          this.hours +
          ':' +
          this.minutes +
          ':' +
          this.seconds;
      }

      /*

if(this.days>0){
        this.displayText= this.days +" days";
}else if(this.hours>0){
        this.displayText=this.hours +" hours";
}else if(this.minutes > 0){
     
}*/

      this.ref.markForCheck();
    });
  }
  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
