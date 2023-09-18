import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { AuthorizationService } from '../components/login/authorization.service';
@Injectable() 
export class SecurityGaurdService implements CanActivate {

  constructor(private _router:Router, private authorizationService: AuthorizationService) { }

  canActivate(route:ActivatedRouteSnapshot): boolean{
    if(this.authorizationService.isUserLoggedIn()){
        return true;
    }else{
        return false;
    }
     
  }
}
 