import { Component, Inject, TemplateRef, OnInit, AfterViewInit } from '@angular/core';
import { Http } from '@angular/http';
import {
    MisUser, Center, Region, ModifyRight, ImportReturn, 
    QueryTime, QueryTimeClass, DownloadClass, TableChartInfo, TableChartInfoClass,
    IData, QueryOption
} from '../../models/common';
//import { BlockUI, NgBlockUI } from 'ng-block-ui';
//import { AlertComponent } from '../alert/alert.component';
import { NgxSpinnerService } from 'ngx-spinner';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['../app/app.component.css']
})
export class DashboardComponent implements AfterViewInit {
    public homeInited = false;
    public noAccess = false;

    public langs: string[] = [
    ];
    public translatedLangs: IData = {};

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, private spinner: NgxSpinnerService, public translate: TranslateService) {
        this.onLangChange();
        this.translate.onLangChange.subscribe((event: any) => {
            this.onLangChange();
        });
    }

    onLangChange() {
        //this.translate.get(this.langs).subscribe((res: any) => {
        //    this.translatedLangs = res;
        //});
    }

    ngAfterViewInit() {
        this.http.post(this.baseUrl + 'api/MisUser/HasAccess', { name: 'user' }).subscribe(result => {
            this.homeInited = true;
        }, error => {
            this.noAccess = true;
        });
    }

}
