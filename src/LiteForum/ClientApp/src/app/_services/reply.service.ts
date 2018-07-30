import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appConfig } from '../_helpers/app.config';
import { Observable } from 'rxjs/Observable';
import { Reply } from '../_models/reply';

@Injectable()
export class ReplyService {
    private BASE_URL: string;
    constructor(private http: HttpClient) {
        this.BASE_URL = `${appConfig.BASE_URL}/comment`;
    }

    getAll(commentId: number): Observable<Reply[]>  {
        return this.http.get<Reply[]>(this.completeUrl(commentId));
    }

    getOne(commentId: number, id: number, withParent?: boolean): Observable<Reply> {
        return this.http.get<Reply>(`${this.completeUrl(commentId)}/${id}?withParent=${withParent}`);
    }

    create(commentId: number, reply: Reply): Observable<Reply> {
        return this.http.post<Reply>(this.completeUrl(commentId), reply);
    }

    update(commentId: number, reply: Reply): Observable<Reply> {
        return this.http.put<Reply>(this.completeUrl(commentId), reply);
    }

    delete(commentId: number, id: number): any {
        return this.http.delete<any>(`${this.completeUrl(commentId)}/${id}`);
    }

    private completeUrl(commentId: number): string {
        return `${this.BASE_URL}/${commentId}/reply`;
    }
}
