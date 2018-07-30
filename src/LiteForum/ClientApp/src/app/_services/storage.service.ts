import { Injectable } from '@angular/core';
import { Login } from '../_models/login';
import { appConfig } from '../_helpers/app.config';

@Injectable()
export class StorageService {
    private store: Storage;
    constructor() {
        this.store = localStorage;
    }

    saveCreds(payload: any) {
        this.save(appConfig.CURRENT_USER, payload.token);
        this.save(appConfig.EXPIRY_DATE, payload.expiration);
    }

    save(key: string, value: string): void {
        this.store.setItem(key, value);
    }

    read(key: string): string {
        return this.store.getItem(key) || '';
    }

    clear(key: string) {
        this.store.removeItem(key);
    }

    clearAll() {
        this.store.clear();
    }
}
