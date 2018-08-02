import { Component, Input, OnInit } from '@angular/core';
import { Reply } from '../../_models/reply';
import { StorageService } from '../../_services/storage.service';
import { appConfig } from '../../_helpers/app.config';

@Component({
    selector: 'lfc-reply-card',
    templateUrl: './replycard.component.html',
    styleUrls: ['./replycard.component.css']
})
export class ReplyCardComponent implements OnInit {
    @Input() reply: Reply;
    editable = false;

    constructor(private store: StorageService) {}

    ngOnInit(): void {
        if (this.reply && this.reply.user === this.store.read(appConfig.LOGGED_IN_USER)) {
            this.editable = true;
        }
    }
}
