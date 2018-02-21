import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StorageService } from './storage.service';
import 'rxjs/add/operator/map'
import { appConfig } from '../_helpers/app.config';
import { Login } from '../_models/login';
import { Register } from '../_models/register';

@Injectable()
export class AuthService {
    private BASE_URL: string;
    constructor(private http: HttpClient, private store: StorageService) {
        this.BASE_URL = appConfig.BASE_URL;
    }

    register(user: Register) {
        return this.http.post<any>(`${this.BASE_URL}/register`, user);
    }

    login(user: Login) {
        return this.http.post<any>(`${this.BASE_URL}/login`, user)
            .map(result => {
                if(result && result.token) {
                    this.store.saveCreds(result);
                }
            });
    }

    logout() {
        this.store.clearAll();
    }
}
