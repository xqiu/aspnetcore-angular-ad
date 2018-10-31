import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(public translate: TranslateService) {
        translate.addLangs(['en', 'zh-TW', 'zh-CN']);
        translate.setDefaultLang('zh-TW');

        const browserLang = translate.getBrowserLang();
        const browserCultureLang = translate.getBrowserCultureLang();

        if (browserLang.match(/en/)) {
            translate.use('en');
        }
        else if (browserCultureLang.match(/zh-TW|zh-CN/)) {
            translate.use(browserCultureLang);
        }
        else {
            translate.use('en');
        }
    }
}
