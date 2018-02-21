import { Base } from "./base";

export class Reply implements Base {
    constructor() {
        this.content = '';
        this.commentId = 0;
    }
    modifiedDate?: Date;
    createdDate?: Date;
    user?: string;
    id?: number;
    commentId: number;
    content: string;
}
