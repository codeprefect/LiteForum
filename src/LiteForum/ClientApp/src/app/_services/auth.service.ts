import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { StorageService } from './storage.service';

import { appConfig } from '../_helpers/app.config';
import { Login } from '../_models/login';
import { Register } from '../_models/register';
import { map } from 'rxjs/operators';

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
          .pipe(map(result => {
            if (result && result.token) {
              this.store.saveCreds(result);
              this.store.save(appConfig.LOGGED_IN_USER, user.username);
            }
          }));
    }

    isLoggedIn() {
        if (this.store.read(appConfig.CURRENT_USER)) {
            return true;
        }
        return false;
    }

    isValid() {

    }

    logout() {
        this.store.clearAll();
    }
}
