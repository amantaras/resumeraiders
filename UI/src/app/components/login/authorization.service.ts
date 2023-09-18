import { Injectable } from '@angular/core';
import { Loggeduser } from './loggeduser';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthorizationService {
  user!: Loggeduser;
  redirectUrl!: string;
  loading!: boolean;
  result!: boolean;

  constructor(private httpClient: HttpClient) {}

  login(userName: string, password: string) {
    console.log('Identity API ' + environment.identityAPI);
    return this.httpClient
      .get(
        environment.identityAPI +
          '/signin?user=' +
          userName +
          '&pwd=' +
          password
      )
      .toPromise();
  }

  isUserLoggedIn(): boolean {
    //this.user = JSON.parse(localStorage.getItem('userObject') || '{}');
    if (localStorage.getItem('userObject') != null) return true;
    else return false;
  }

  logout(): void {
    if(localStorage.getItem('userObject')!=null)
    {
      localStorage.removeItem('userObject')
    }
  }

  getLoggedUser(): Loggeduser {
    this.user = JSON.parse(localStorage.getItem('userObject') || '{}');
    return this.user;
  }
}
