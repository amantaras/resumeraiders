import { Injectable } from '@angular/core';

export interface Alert {
  type: string;
  message: string;
}

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  messages: Alert[] = [];

  add(message: Alert) {
    console.log('added message :' + message.message);
    this.messages.push(message);
  }

  close(alert: Alert) {
    this.messages.splice(this.messages.indexOf(alert), 1);
  }

  clear(alert: Alert) {
    console.log('called close');
    this.messages.splice(this.messages.indexOf(alert), 1);
  }
}
