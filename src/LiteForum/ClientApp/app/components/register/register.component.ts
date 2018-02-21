import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../_services/auth.service';
import { AlertService } from '../../_services/alert.service';

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
export class RegisterComponent {
    model: any;
    loading = false;
    
    constructor(
        private router: Router,
        private authService: AuthService,
        private alertService: AlertService
    ) { }

    register() {
        this.loading = true;
        this.authService.register(this.model)
            .subscribe(
                data => {
                    this.alertService.success('Registration successful', true);
                    this.router.navigate(['/login']);
                },
                error => {
                    this.alertService.error(error);
                    this.loading = false;
                });
    }
}
