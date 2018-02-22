import { Base } from './base';
import { Comment } from './comment';

export class Post implements Base {
    constructor() {
        this.title = '';
    }
    modifiedDate?: Date;
    createdDate?: Date;
    user?: string;
    id?: any;
    title: string;
    comments?: Comment[];
    commentsCount?: number;
    lastCommentAt?: Date;
    lastCommentBy?: string;
}
