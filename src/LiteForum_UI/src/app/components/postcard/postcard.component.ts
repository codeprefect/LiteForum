import { Component, Input } from '@angular/core';
import { Post } from '../../_models/post';

@Component({
    selector: 'lfc-post-card',
    templateUrl: './postcard.component.html',
    styleUrls: ['./postcard.component.css']
})
export class PostCardComponent {
    @Input() post: Post;
}
