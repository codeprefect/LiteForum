import { Base } from "./base";
import { Reply } from "./reply";

export class Comment implements Base {
    constructor() {
        this.postId = 0;
        this.content = '';
    }
    modifiedDate?: Date;
    createdDate?: Date;
    user?: string;
    id?: number;
    postId: number;
    content: string;
    replies?: Reply[];
    repliesCount?: number;
    lastReplyAt?: Date;
    lastReplyBy?: string;
}
