import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { appConfig } from '../_helpers/app.config';
import { Observable } from 'rxjs';
import { Category } from '../_models/category';

@Injectable()
export class CategoryService {
    private BASE_URL: string;
    constructor(private http: HttpClient) {
        this.BASE_URL = `${appConfig.BASE_URL}/category`;
    }

    getAll(): Observable<Category[]>  {
        return this.http.get<Category[]>(this.BASE_URL);
    }

    getOne(id: number, withChild?: boolean): Observable<Category> {
        return this.http.get<Category>(`${this.BASE_URL}/${id}?withChild=${withChild}`);
    }

    create(category: Category): Observable<Category> {
        return this.http.post<Category>(this.BASE_URL, category);
    }

    update(category: Category): Observable<Category> {
        return this.http.put<Category>(this.BASE_URL, category);
    }

    delete(id: number): any {
        return this.http.delete<any>(`${this.BASE_URL}/${id}`);
    }
}
