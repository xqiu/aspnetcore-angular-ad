import { NgModule } from '@angular/core';
import { ServerModule } from '@angular/platform-server';
import { AppModuleShared } from './app.shared.module';
import { AppComponent } from './components/app/app.component';
import { TranslateService } from '@ngx-translate/core';

@NgModule({
    bootstrap: [ AppComponent ],
    imports: [
        ServerModule,
        AppModuleShared
    ]
})
export class AppModule {
    constructor(public translate: TranslateService) {

        translate.addLangs(['en', 'zh-TW', 'zh-CN']);
        translate.setDefaultLang('zh-TW');

        const browserLang = translate.getBrowserLang();
        const browserCultureLang = translate.getBrowserCultureLang();

        if (browserLang.match(/en/)) {
            translate.use('en');
        }
        else if (browserCultureLang.match(/zh-TW|zh-CN/)) {
            var test = translate.use(browserCultureLang);
            var a = 1;
        }
        else {
            translate.use('en');
        }
    }
}
