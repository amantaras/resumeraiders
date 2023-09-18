import { Component, OnInit } from '@angular/core';
import { AuthorizationService } from './authorization.service';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { Loggeduser } from './loggeduser';
import { AlertService } from '../alert/alert.service';
import { Logger } from 'msal';

@Component({
  selector: 'resumeraider-login',
  templateUrl: './login.component.html',
})
export class LoginComponent implements OnInit {
  loginErrorMessage: boolean = false;
  //user!: Loggeduser; 
  userName: String = 'andrewbrandon@resumeraiders.com';
  password: String = 'password';

  constructor(
    private authService: AuthorizationService,
    private router: Router,
    private alertService: AlertService
  ) {}

  ngOnInit(): void {}

  login(loginForm: NgForm) {
    if (loginForm && loginForm.valid) {
      let userName = loginForm.form.value.userName;
      let password = loginForm.form.value.password;
      let user: Loggeduser ={UserId: '1', UserName: 'andrewbrandon@resumeraiders.com'};
      localStorage.setItem('userObject', JSON.stringify(user));
      if(userName == 'admin')
      {
        this.router.navigate(['/admin']);
      }else{
        this.router.navigate(['/home']);
      }
      /*this.authService
        .login(userName, password)
        .then((res) => {
          if (res == null) {
            this.alertService.add({
              type: 'danger',
              message: 'Username or password is incorrect, please try again!',
            });
          } else {
            console.log('logged in successfully, res is ' + res);
            console.log(JSON.stringify(res));
            this.user = JSON.parse(JSON.stringify(res));

            console.log('user Id is ' + this.user.UserId);
            localStorage.setItem('userObject', JSON.stringify(this.user));
            this.router.navigate(['/home']);
          }
        })
        .catch((error) => {
          this.alertService.add({
            type: 'danger',
            message: 'User name or password is incorrect, please try again!',
          });
        }); 
        */

    }
  }
}
