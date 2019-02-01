import { Component, OnInit } from '@angular/core';
import { PostService } from '../../_services/post.service';
import { Post } from '../../_models/post';
import { AlertService } from '../../_services/alert.service';
import { AuthService } from '../../_services/auth.service';
import { CategoryService } from '../../_services/category.service';
import { Category } from '../../_models/category';

@Component({
    selector: 'lfc-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    categories: Category[];
    posts: Post[] = [];
    newPost: any = {};
    adding = false;

    constructor(private forum: PostService,
        private alertService: AlertService,
        private auth: AuthService,
        private categoryService: CategoryService
    ) { }

    ngOnInit(): void {
        this.forum.getAll().subscribe(result => {
            this.posts = result.reverse();
        }, error => {
            this.alertService.error(error);
        });

        this.categoryService.getAll().subscribe(result => {
            this.categories = result;
        }, error => {
            this.alertService.error(error);
        });
    }

    isLoggedIn(): boolean {
        return this.auth.isLoggedIn();
    }

    openAdd(): void {
        this.adding = true;
    }

    addPost(): void {
        this.forum.create(this.newPost).subscribe(post => {
            this.forum.getOne(post.id, true).subscribe(thePost => {
                this.newPost = {};
                this.posts.push(thePost);
            });
        });
    }
}
