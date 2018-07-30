import { Component, OnInit, Input } from '@angular/core';
import { PostService } from '../../_services/post.service';
import { Post } from '../../_models/post';
import { AlertService } from '../../_services/alert.service';

@Component({
    selector: 'post-card',
    templateUrl: './postcard.component.html',
    styleUrls: ['./postcard.component.css']
})
export class PostCardComponent {
    @Input() post: Post;
}
