import { Component, OnInit } from '@angular/core';
import { PostService } from '../../_services/post.service';
import { Post } from '../../_models/post';
import { AlertService } from '../../_services/alert.service';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    posts: Post[] = [];
    
    constructor(private forum: PostService, private alertService: AlertService) { }

    ngOnInit(): void {
        this.forum.getAll().subscribe(result => {
            this.posts = result;
        },
        error => {
            this.alertService.error(error);
        });
    }
}
