import { Component } from '@angular/core';
import { AuthService } from '../../_services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent {
    constructor(private auth: AuthService, private router: Router) { }

    isLoggedIn() {
        return this.auth.isLoggedIn();
    }

    logout() {
        console.log("logging out");
        this.auth.logout();
        this.router.navigate(['/home']);
    }
}
