import { Component, OnInit, Input, HostBinding } from '@angular/core';
import { PostService } from '../../_services/post.service';
import { Post } from '../../_models/post';
import { AlertService } from '../../_services/alert.service';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { CommentService } from '../../_services/comment.service';
import { StorageService } from '../../_services/storage.service';
import { appConfig } from '../../_helpers/app.config';

@Component({
    selector: 'lfc-post-detail',
    templateUrl: './postdetail.component.html',
    styleUrls: ['./postdetail.component.css']
})
export class PostDetailComponent implements OnInit {
    loading = false;
    id: number;
    post: Post;
    newComment: any = {};

    constructor(
        private route: ActivatedRoute,
        private posts: PostService,
        private comments: CommentService,
        private store: StorageService,
        private alert: AlertService
    ) { }

    ngOnInit() {
        this.route.params.subscribe(params => {
            this.id = +params['id'];
            console.log(this.id);
            this.posts.getOne(this.id, true).subscribe(post => {
                this.post = post;
                this.initComments();
            });
        });
    }

    initComments(): void {
        if (!this.post.comments) {
            this.post.comments = [];
        }
    }

    addComment(): void {
        this.loading = true;
        this.comments.create(this.post.id || 0, this.newComment)
            .subscribe(
                data => {
                    console.log(data);
                    data.user = this.store.read(appConfig.LOGGED_IN_USER);
                    this.post.comments.push(data);
                    this.loading = false;
                    this.newComment = { };
                },
                error => {
                    this.alert.error(error.error.message);
                    this.loading = false;
                });
    }
}
