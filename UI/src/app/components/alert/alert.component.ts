import { Component, OnInit } from '@angular/core';
import { AlertService } from './alert.service';

@Component({
  selector: 'alert',
  templateUrl: './alert.component.html',
})
export class AlertComponent implements OnInit {
  constructor(public alertService: AlertService) {}

  ngOnInit() {}
}
