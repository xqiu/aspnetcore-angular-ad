import { Component, Inject, TemplateRef, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['../app/app.component.css']
})
export class HomeComponent implements AfterViewInit {
    public homeInited = false;

    constructor(public router: Router, public translate: TranslateService) {
    }

    ngAfterViewInit() {
        //
    }

    public gotoProperRoute() {
        var path = this.router.url;

        if (path == '/home') {
            this.router.navigateByUrl('/dashboard');
            return;
        }

        if (path.startsWith('/home?path=')) {
            this.router.navigateByUrl(path.substr(11));
        }
    }
}
