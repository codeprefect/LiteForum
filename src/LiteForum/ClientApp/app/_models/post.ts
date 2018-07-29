import { Base } from './base';
import { Comment } from './comment';
import { Category } from './category';

export class Post implements Base {
    constructor() {
        this.title = '';
    }
    modifiedDate?: Date;
    createdDate?: Date;
    user?: string;
    id?: any;
    title: string;
    category: Category;
    comments: Comment[] = [];
    commentsCount?: number;
    lastCommentAt?: Date;
    lastCommentBy?: string;
}
