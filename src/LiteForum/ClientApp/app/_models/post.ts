import { Base } from './base';
import { Comment } from './comment';

export class Post implements Base {
    constructor() {
        this.content = '';
    }
    modifiedDate?: Date;
    createdDate?: Date;
    user?: string;
    id?: any;
    content: string;
    comments?: Comment[];
    commentsCount?: number;
    lastCommentAt?: Date;
    lastCommentBy?: string;
}
