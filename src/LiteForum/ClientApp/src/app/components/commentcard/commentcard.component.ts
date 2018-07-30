import { Component, OnInit, Input } from '@angular/core';
import { PostService } from '../../_services/post.service';
import { Comment } from '../../_models/comment';
import { AlertService } from '../../_services/alert.service';
import { Reply } from '../../_models/reply';
import { ReplyService } from '../../_services/reply.service';
import { StorageService } from '../../_services/storage.service';
import { appConfig } from '../../_helpers/app.config';

@Component({
    selector: 'comment-card',
    templateUrl: './commentcard.component.html',
    styleUrls: ['./commentcard.component.css']
})
export class CommentCardComponent implements OnInit {
    @Input() comment: Comment;
    loading: boolean = false;
    editable: boolean = false;
    newReply: any = {};

    constructor(
        private replies: ReplyService,
        private alert: AlertService,
        private store: StorageService
    ) { }

    ngOnInit(): void {
        if(!this.comment.replies) {
            this.comment.replies = [ ];
        }
        if (this.comment && this.comment.user === this.store.read(appConfig.LOGGED_IN_USER)) {
            this.editable = true;
        }
    }

    addReply(): void {
        this.loading = true;
        this.replies.create(this.comment.id || 0, this.newReply)
            .subscribe(
                data => {
                    console.log(data);
                    data.user = this.store.read(appConfig.LOGGED_IN_USER);
                    this.comment.replies.push(data);
                    this.loading = false;
                    this.newReply = { };
                },
                error => {
                    this.alert.error(error.error.message);
                    this.loading = false;
                });
    }
}
