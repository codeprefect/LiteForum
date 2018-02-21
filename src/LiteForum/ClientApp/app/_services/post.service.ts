import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Post } from '../_models/post';
import { appConfig } from '../_helpers/app.config';

@Injectable()
export class PostService {
    constructor(private http: HttpClient) { }

    getAll() {
        return;
    }
}
