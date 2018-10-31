import { Component, Inject, TemplateRef, OnInit} from '@angular/core';
import { Http } from '@angular/http';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import {
    MisUser, MisUserClass, Center, CenterClass, Region, RegionClass, ModifyRight, IData,
    EmailTemplate, EmailTemplateClass} from '../../models/common';
import { TranslateService } from '@ngx-translate/core';

@Component({
    selector: 'useradmin',
    templateUrl: './useradmin.component.html',
    styleUrls: ['../app/app.component.css', './useradmin.component.css']
})
export class UserAdminComponent implements OnInit{
    public users: MisUser[];
    public regions: Region[];
    //public centers: Center[];
    //public rights: ModifyRight[];

    public regionModify: Region = RegionClass.createNew();
    public regionReference: Region = this.regionModify;
    public regionModalRef: BsModalRef | null = null;

    public centerModify: Center = CenterClass.createNew();
    public centerReference: Center = this.centerModify;
    public centerModalRef: BsModalRef | null = null;

    public misUserModify: MisUser = MisUserClass.createNew();
    public misUserReference: MisUser = this.misUserModify;
    public misUserModalRef: BsModalRef | null = null;

    public regionInited = false;
    public userInited = false;
    public noAccess = false;

    public emailTemplates: EmailTemplate[] = [];
    public emailTemplateModify: EmailTemplate = EmailTemplateClass.createNew();
    public emailTemplateReference: EmailTemplate = this.emailTemplateModify;
    public emailTemplateModalRef: BsModalRef | null = null;

    public langs: string[] = ['admin.DeleteRegionConfirm', 'admin.DeleteCenterConfirm', 'admin.DeleteUserConfirm', 'admin.DeleteEmailTemplateConfirm'];
    public translatedLangs: IData = {};

    constructor(private http: Http, @Inject('BASE_URL') private baseUrl: string, private modalService: BsModalService, public translate: TranslateService) {
        this.users = [];
        this.regions = [];

        this.http.post(this.baseUrl + 'api/MisUser/HasAccess', { name: 'useradmin' }).subscribe(result => {
            this.refreshUsers();
            this.refreshRegions();
            this.queryEmailTemplates();
        }, error => {
            this.noAccess = true;
        });
        this.onLangChange();
        this.translate.onLangChange.subscribe((event: any) => {
            this.onLangChange();
        });
    }

    onLangChange() {
        this.translate.get(this.langs).subscribe((res: any) => {
            this.translatedLangs = res;
        });
    }

    ngOnInit() {
    }

    public refreshUsers() {
        this.http.get(this.baseUrl + 'api/MisUser/GetUsers').subscribe(result => {
            this.users = result.json() as MisUser[];
            this.userInited = true;
        }, error => this.showError(error));
    }

    public refreshRegions() {
        this.http.get(this.baseUrl + 'api/Region/GetWritableRegions').subscribe(result => {
            this.regions = result.json() as Region[];
            this.regionInited = true;
        }, error => this.showError(error));
    }

    public showError(error: any) {
        console.error(error);
        if ((typeof error) == "string") {
            alert(error);
        }
        else {
            alert(JSON.stringify(error));
        }
    }

    //region
    public editRegion(template: TemplateRef<any>, region: Region) {
        this.regionModify = RegionClass.clone(region);
        this.regionReference = region;

        this.regionModalRef = this.modalService.show(template, { class: 'modal-sm' });
    }

    public addRegion(template: TemplateRef<any>) {
        this.regionModify = RegionClass.createNew();
        this.regionReference = this.regionModify;

        this.regionModalRef = this.modalService.show(template, { class: 'modal-sm' });
    }

    public deleteRegion(region: Region) {
        this.regionModify = RegionClass.clone(region);
        this.regionReference = region;

        if (window.confirm(this.translatedLangs["admin.DeleteRegionConfirm"] + " -- " + region.name)) {
            this.http.post(this.baseUrl + 'api/Region/Delete', this.regionModify).subscribe(result => {
                this.refreshRegions();
            }, error => this.showError(error));
        }
    }

    public modifyRegion() {
        if (this.regionModify.regionID == 0) {
            //new
            this.http.post(this.baseUrl + 'api/Region/Create', this.regionModify).subscribe(result => {
                this.regionModify = result.json() as Region;
                if (this.regionModalRef != null) {
                    this.regionModalRef.hide();
                }
                this.refreshRegions();
            }, error => this.showError(error));
        }
        else {
            //modify
            var regionForUpdate = {
                regionID: this.regionModify.regionID,
                name: this.regionModify.name
            };
            this.http.post(this.baseUrl + 'api/Region/Update', regionForUpdate).subscribe(result => {
                var regionUpdated = result.json() as any;

                RegionClass.assign(regionUpdated, this.regionReference);

                if (this.regionModalRef != null) {
                    this.regionModalRef.hide();
                }
            }, error => this.showError(error));
        }
    }

    //center
    public editCenter(template: TemplateRef<any>, center: Center) {
        this.centerModify = CenterClass.clone(center);
        this.centerReference = center;

        this.centerModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public addCenter(template: TemplateRef<any>, region: Region) {
        this.centerModify = CenterClass.createNew();
        this.centerModify.regionID = region.regionID;
        this.centerReference = this.centerModify;

        this.centerModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public deleteCenter(center: Center) {
        this.centerModify = CenterClass.clone(center);
        this.centerReference = center;

        if (window.confirm(this.translatedLangs['admin.DeleteCenterConfirm'] + ' -- ' + center.name)) {
            this.http.post(this.baseUrl + 'api/Center/Delete', this.centerModify).subscribe(result => {
                this.refreshRegions();
            }, error => this.showError(error));
        }
    }

    public modifyCenter() {
        if (this.centerModify.centerID == 0) {
            //new center, post centerID as parent regionID
            this.http.post(this.baseUrl + 'api/Center/Create', this.centerModify).subscribe(result => {
                this.centerModify = result.json() as Center;
                if (this.centerModalRef != null) {
                    this.centerModalRef.hide();
                }
                this.refreshRegions();
            }, error => this.showError(error));
        }
        else {
            //modify
            var centerForUpdate = CenterClass.clone(this.centerModify);
            this.http.post(this.baseUrl + 'api/Center/Update', centerForUpdate).subscribe(result => {
                var centerUpdated = result.json() as any;

                CenterClass.assign(centerUpdated, this.centerReference);
                if (this.centerModalRef != null) {
                    this.centerModalRef.hide();
                }
            }, error => this.showError(error));
        }
    }

    //misUser
    public getUserCenterName(misUser: MisUser) {
        if (misUser.center != null)
            return misUser.center.name;

        return "";
    }

    public getUserModifyRight(misUser: MisUser) {
        var rightStr = "";
        for (var i = 0; i < misUser.modifyRights.length; i++) {
            var right = misUser.modifyRights[i];
            var found = false;
            for (var j = 0; j < this.regions.length; j++) {
                for (var k = 0; k < this.regions[j].centers.length; k++) {
                    if (this.regions[j].centers[k].centerID == right.centerID) {
                        found = true;
                        rightStr += this.regions[j].centers[k].name;
                        break;
                    }
                }
                if (found) {
                    break;
                }
            }

            rightStr += "(";
            if (right.canRead) {
                rightStr += "R";
            }
            else {
                rightStr += "-";
            }
            if (right.canWrite) {
                rightStr += "W";
            }
            else {
                rightStr += "-";
            }
            if (right.canAdmin) {
                rightStr += "A";
            }
            else {
                rightStr += "-";
            }
            rightStr += ") ";
        }

        return rightStr;
    }

    public editMisUser(template: TemplateRef<any>, misUser: MisUser) {
        this.misUserModify = MisUserClass.clone(misUser);
        this.misUserReference = misUser;

        this.misUserModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public addMisUser(template: TemplateRef<any>) {
        this.misUserModify = MisUserClass.createNew();
        this.misUserReference = this.misUserModify;

        this.addUserRight(this.misUserModify);

        this.misUserModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public deleteMisUser(misUser: MisUser) {
        this.misUserModify = MisUserClass.clone(misUser);
        this.misUserReference = misUser;

        if (window.confirm(this.translatedLangs['admin.DeleteUserConfirm'] + " -- " + misUser.name + "(" + misUser.identityName + ")")) {
            this.http.post(this.baseUrl + 'api/MisUser/Delete', this.misUserModify).subscribe(result => {
                this.refreshUsers();
            }, error => this.showError(error));
        }
    }

    public modifyMisUser() {
        if (this.misUserModify.misUserID == 0) {
            //new misUser, post misUserID as parent regionID
            this.http.post(this.baseUrl + 'api/MisUser/Create', this.misUserModify).subscribe(result => {
                this.misUserModify = result.json() as MisUser;
                if (this.misUserModalRef != null) {
                    this.misUserModalRef.hide();
                }
                this.refreshUsers();
            }, error => this.showError(error));
        }
        else {
            //modify
            this.http.post(this.baseUrl + 'api/MisUser/Update', this.misUserModify).subscribe(result => {
                var misUserUpdated = result.json() as MisUser;

                MisUserClass.assign(misUserUpdated, this.misUserReference);

                if (this.misUserModalRef != null) {
                    this.misUserModalRef.hide();
                }
            }, error => this.showError(error));
        }
    }

    public addUserRight(user: MisUser) {
        var modifyRight = {
            modifyRightID: 0,
            centerID: 0,
            misUserID: user.misUserID,
            canRead: true,
            canWrite: true,
            canAdmin: false,
        };
        this.misUserModify.modifyRights.push(modifyRight);
    }

    public deleteModifyRights(index: number) {
        this.misUserModify.modifyRights.splice(index, 1);
    }

    //emailTemplate
    public editEmailTemplate(template: TemplateRef<any>, emailTemplate: EmailTemplate) {
        this.emailTemplateModify = EmailTemplateClass.clone(emailTemplate);
        this.emailTemplateReference = emailTemplate;

        this.emailTemplateModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public addEmailTemplate(template: TemplateRef<any>) {
        this.emailTemplateModify = EmailTemplateClass.createNew();
        this.emailTemplateReference = this.emailTemplateModify;

        this.emailTemplateModalRef = this.modalService.show(template, { class: 'modal-lg' });
    }

    public modifyEmailTemplate() {
        if (this.emailTemplateModify.emailTemplateID == 0) {
            //new
            this.http.post(this.baseUrl + 'api/Notification/CreateEmailTemplate', this.emailTemplateModify).subscribe(result => {
                this.emailTemplateModify = result.json() as EmailTemplate;
                if (this.emailTemplateModalRef != null) {
                    this.emailTemplateModalRef.hide();
                }
                this.queryEmailTemplates();
            }, error => {
                this.showError(error);
            });
        }
        else {
            //modify
            this.http.post(this.baseUrl + 'api/Notification/UpdateEmailTemplate', this.emailTemplateModify).subscribe(result => {
                var emailTemplateUpdated = result.json() as EmailTemplate;

                EmailTemplateClass.assign(emailTemplateUpdated, this.emailTemplateReference);

                if (this.emailTemplateModalRef != null) {
                    this.emailTemplateModalRef.hide();
                }
            }, error => this.showError(error));
        }
    }

    public deleteEmailTemplate(record: EmailTemplate) {
        if (window.confirm(this.translatedLangs['admin.DeleteEmailTemplateConfirm'] +
            ' -- ' + record.subject)) {

            this.http.post(this.baseUrl + 'api/Notification/DeleteEmailTemplate', record).subscribe(result => {
                this.queryEmailTemplates();
            }, error => {
                this.showError(error);
            });
        }
    }

    public queryEmailTemplates() {
        this.http.get(this.baseUrl + 'api/Notification/GetEmailTemplates').subscribe(result => {
            this.emailTemplates = result.json() as EmailTemplate[];
        }, error => {
            this.showError(error);
        });
    }

}
