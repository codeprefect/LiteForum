import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { appConfig } from './app.config';
import { StorageService } from '../_services/storage.service';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
    constructor(private store: StorageService) { }
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // add authorization header with jwt token if available
        let userCreds = this.store.read(appConfig.CURRENT_USER);
        if (userCreds !== '') {
            request = request.clone({
                setHeaders: {
                    Authorization: `Bearer ${userCreds}`
                }
            });
        }

        return next.handle(request);
    }
}