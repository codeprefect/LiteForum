import { Base } from './base';
import { Comment } from './comment';

export class Category implements Base {
    constructor() {
        this.name = '';
    }
    modifiedDate?: Date;
    createdDate?: Date;
    id?: number;
    name: string;
}
