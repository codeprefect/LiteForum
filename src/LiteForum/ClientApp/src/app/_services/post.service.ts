import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Post } from '../_models/post';
import { appConfig } from '../_helpers/app.config';
import { Observable } from 'rxjs';

@Injectable()
export class PostService {
    private BASE_URL: string;
    constructor(private http: HttpClient) {
        this.BASE_URL = `${appConfig.BASE_URL}/post`;
    }

    getAll(): Observable<Post[]>  {
        return this.http.get<Post[]>(this.BASE_URL);
    }

    getOne(id: number, withChild?: boolean): Observable<Post> {
        return this.http.get<Post>(`${this.BASE_URL}/${id}?withChild=${withChild}`);
    }

    create(post: Post): Observable<Post> {
        return this.http.post<Post>(this.BASE_URL, post);
    }

    update(post: Post): Observable<Post> {
        return this.http.put<Post>(this.BASE_URL, post);
    }

    delete(id: number): any {
        return this.http.delete<any>(`${this.BASE_URL}/${id}`);
    }
}
