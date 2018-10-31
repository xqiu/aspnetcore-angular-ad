import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';

import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';

import { Http } from '@angular/http';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { NgxSpinnerModule } from 'ngx-spinner';
import { ChartsModule } from 'ng2-charts';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';
import { UserAdminComponent } from './components/useradmin/useradmin.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: Http) {
    return new TranslateHttpLoader(http);
}

//import { AlertComponent } from './components/alert/alert.component';

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        UserAdminComponent,
        HomeComponent,
        DashboardComponent,
//        AlertComponent,
    ],
    imports: [
        TooltipModule.forRoot(),
        ModalModule.forRoot(),
        TabsModule.forRoot(),
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: HttpLoaderFactory,
                deps: [Http]
            }
        }),
        NgxSpinnerModule,
        CommonModule,
        HttpModule,
        FormsModule,
        ChartsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },
            { path: 'home', component: HomeComponent },
            { path: 'dashboard', component: DashboardComponent },
            { path: 'user-admin', component: UserAdminComponent },
            { path: '**', redirectTo: 'home' }
        ])
    ]
    //entryComponents: [
    //    AlertComponent
    //]
})
export class AppModuleShared {
}
