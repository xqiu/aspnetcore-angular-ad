import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

declare var mymis: any;

@Component({
    selector: 'nav-menu',
    templateUrl: './navmenu.component.html',
    styleUrls: ['./navmenu.component.css']
})
export class NavMenuComponent implements OnInit {
    public userName = '';

    ngOnInit() {
        this.setName();
    }

    constructor(public translate: TranslateService) {
    }

    public setName() {
        if (mymis != null) {
            this.userName = mymis.username;
        }
    }
}
