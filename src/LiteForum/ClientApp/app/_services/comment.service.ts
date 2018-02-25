import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Comment } from '../_models/comment';
import { appConfig } from '../_helpers/app.config';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class CommentService {
    private BASE_URL: string;
    constructor(private http: HttpClient) {
        this.BASE_URL = `${appConfig.BASE_URL}/post`;
    }

    getAll(postId: number): Observable<Comment[]>  {
        return this.http.get<Comment[]>(this.completeUrl(postId));
    }

    getOne(postId: number, id: number, withChild?: boolean): Observable<Comment> {
        return this.http.get<Comment>(`${this.completeUrl(postId)}/${id}?withChild=${withChild}`);
    }

    create(postId: number, comment: Comment): Observable<Comment> {
        return this.http.post<Comment>(this.completeUrl(postId) , comment);
    }

    update(postId: number, comment: Comment): Observable<Comment> {
        return this.http.put<Comment>(this.completeUrl(postId), comment);
    }

    delete(postId: number, id: number): any {
        return this.http.delete<any>(`${this.completeUrl(postId)}/${id}`);
    }

    private completeUrl(postId: number) {
        return `${this.BASE_URL}/${postId}/comment`;
    }
}
