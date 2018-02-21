import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { appConfig } from '../_helpers/app.config';
import { StorageService } from '../_services/storage.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private router: Router, private store: StorageService) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        if (this.store.read(appConfig.CURRENT_USER) && this.store.read(appConfig.CURRENT_USER).length > 50) {
            // logged in so return true
            return true;
        }

        // not logged in so redirect to login page with the return url
        this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
        return false;
    }
}
