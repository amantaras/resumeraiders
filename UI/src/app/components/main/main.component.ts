import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthorizationService } from '../login/authorization.service';

@Component({
  selector: 'main',
  templateUrl: './main.component.html',
})
export class MainComponent implements OnInit {
  constructor(
    private router: Router,
    private authorizationService: AuthorizationService
  ) {}

  ngOnInit() {
    if (this.authorizationService.isUserLoggedIn()) {
      this.router.navigateByUrl('/home');
    }
  }
}
