export interface MisUser {
    misUserID: number;
    name: string;
    identityName: string;
    isAdmin: boolean;
    isActive: boolean;
    centerID: number;
    center: Center | null;
    modifyRights: ModifyRight[]
}

export class MisUserClass {
    static createNew(): MisUser {
        var misUser: MisUser = {
            misUserID: 0,
            name: '',
            identityName: '@mydomain.com',
            isAdmin: false,
            isActive: true,
            centerID: 0,
            center: null,
            modifyRights: []
        };
        return misUser;
    }

    static clone(misUser: MisUser): MisUser {
        var newMisUser: MisUser = {
            misUserID: misUser.misUserID,
            name: misUser.name,
            identityName: misUser.identityName,
            isAdmin: misUser.isAdmin,
            isActive: misUser.isActive,
            centerID: misUser.centerID,
            center: misUser.center == null ? null : CenterClass.clone(misUser.center),
            modifyRights: [],
        };

        for (var i = 0; i < misUser.modifyRights.length; i++) {
            newMisUser.modifyRights.push(ModifyRightClass.clone(misUser.modifyRights[i]));
        }

        return newMisUser;
    }

    static assign(misUserFrom: MisUser, misUserTo: MisUser) {
        misUserTo.misUserID = misUserFrom.misUserID;
        misUserTo.name = misUserFrom.name;
        misUserTo.identityName = misUserFrom.identityName;
        misUserTo.isAdmin = misUserFrom.isAdmin;
        misUserTo.isActive = misUserFrom.isActive;
        misUserTo.centerID = misUserFrom.centerID;
        if (misUserFrom.center != null) {
            if (misUserTo.center == null) {
                misUserTo.center = CenterClass.createNew();
            }
            CenterClass.assign(misUserFrom.center, misUserTo.center);
        }
        misUserTo.modifyRights = [];
        for (var i = 0; i < misUserFrom.modifyRights.length; i++) {
            misUserTo.modifyRights.push(ModifyRightClass.clone(misUserFrom.modifyRights[i]));
        }
    }
}

export interface Center {
    centerID: number;
    name: string;
    regionID: number;
    doNotTrackMonthStatus: boolean;
    notificationEmail: string;
}

export class CenterClass {
    static createNew(): Center {
        var center: Center = {
            centerID: 0,
            name: '',
            regionID: 0,
            doNotTrackMonthStatus: false,
            notificationEmail: ''
        };
        return center;
    }

    static clone(center: Center): Center {
        var newCenter: Center = {
            centerID: center.centerID,
            name: center.name,
            regionID: center.regionID,
            doNotTrackMonthStatus: center.doNotTrackMonthStatus,
            notificationEmail: center.notificationEmail,
        };
        return newCenter;
    }

    static assign(centerFrom: Center, centerTo: Center) {
        centerTo.centerID = centerFrom.centerID;
        centerTo.name = centerFrom.name;
        centerTo.regionID = centerFrom.regionID;
        centerTo.doNotTrackMonthStatus = centerFrom.doNotTrackMonthStatus;
        centerTo.notificationEmail = centerFrom.notificationEmail;
    }
}

export interface Region {
    regionID: number;
    name: string;
    centers: Center[];
}

export class RegionClass {
    static createNew(): Region {
        var region: Region = {
            regionID: 0,
            name: '',
            centers: []
        };
        return region;
    }

    static clone(region: Region): Region {
        var newRegion: Region = {
            regionID: region.regionID,
            name: region.name,
            centers: region.centers, // this is actually a reference
        };
        return newRegion
    }

    static assign(regionFrom: Region, regionTo: Region) {
        regionTo.regionID = regionFrom.regionID;
        regionTo.name = regionFrom.name;
        regionTo.centers = regionFrom.centers;  // this is actually a reference
    }
}


export interface ModifyRight {
    modifyRightID: number;
    centerID: number;
    misUserID: number;
    canRead: boolean;
    canWrite: boolean;
    canAdmin: boolean;
}
export class ModifyRightClass {
    static createNew(): ModifyRight {
        var modifyRight: ModifyRight = {
            modifyRightID: 0,
            centerID: 0,
            misUserID: 0,
            canRead: false,
            canWrite: false,
            canAdmin: false,
        };
        return modifyRight;
    }

    static clone(modifyRight: ModifyRight): ModifyRight {
        var newModifyRight: ModifyRight = {
            modifyRightID: modifyRight.modifyRightID,
            centerID: modifyRight.centerID,
            misUserID: modifyRight.misUserID,
            canRead: modifyRight.canRead,
            canWrite: modifyRight.canWrite,
            canAdmin: modifyRight.canAdmin,
        };
        return newModifyRight
    }
}

export interface ImportReturn {
    msg: string;
    data: string[];
}

export interface QueryTime {
    timeFrom: number;
    timeTo: number;
}

export interface QueryOption {
    timeFromTo: QueryTime
    groupBy: string,
    sortBy: string
}

export class QueryTimeClass {
    static getLastYearMonth(): number {
        var cDate = new Date();
        var month = cDate.getMonth();
        var year = cDate.getFullYear();

        if (month == 0) {
            year = year - 1;
            month = 12;
        }

        return year * 100 + month;
    }

    static getQueryMonths(timeFromYear: number, timeFromMonth: number): number[] {
        var yearmonths: number[] = [];

        var timeFromYear = timeFromYear;
        var timeToYear = (new Date()).getFullYear();

        var timeFromMonth = timeFromMonth;
        var timeToMonth = (new Date()).getMonth() + 1;

        for (var year = timeFromYear; year <= timeToYear; year++) {
            var startMonth = 1;
            var endMonth = 12;
            if (year == timeFromYear) {
                startMonth = timeFromMonth;
            }
            if (year == timeToYear) {
                endMonth = timeToMonth;
            }
            for (var month = startMonth; month <= endMonth; month++) {
                var yearmonth = year * 100 + month;
                yearmonths.push(yearmonth);
            }
        }

        return yearmonths;
    }

    static getQueryMonthsUpToNextYear(timeFromYear: number, timeFromMonth: number): number[] {
        var yearmonths: number[] = [];

        var timeToYear = (new Date()).getFullYear() + 1;

        var timeToMonth = 12;

        for (var year = timeFromYear; year <= timeToYear; year++) {
            var startMonth = 1;
            var endMonth = 12;
            if (year == timeFromYear) {
                startMonth = timeFromMonth;
            }
            if (year == timeToYear) {
                endMonth = timeToMonth;
            }
            for (var month = startMonth; month <= endMonth; month++) {
                var yearmonth = year * 100 + month;
                yearmonths.push(yearmonth);
            }
        }

        return yearmonths;
    }

    static getQueryYearsUpToNextYear(timeFromYear: number): number[] {
        var years: number[] = [];

        var timeToYear = (new Date()).getFullYear() + 1;

        for (var year = timeFromYear; year <= timeToYear; year++) {
            years.push(year);
        }

        return years;
    }

    static pad(number: number) {
        if (number < 10) {
            return `0${number}`;
        }
        return number;
    }

    static getDateString(date: Date | null): string {
        if (date == null) {
            return "";
        }
        let mydate = new Date(date);
        let month = QueryTimeClass.pad(mydate.getMonth() + 1);
        let day = QueryTimeClass.pad(mydate.getDate());

        return `${mydate.getFullYear()}-${month}-${day}`;
    }
}

export class DownloadClass {
    static b64EncodeUnicode(str: string): string {
        return btoa(encodeURIComponent(str).replace(/%([0-9A-F]{2})/g, function (match, p1) {
            return String.fromCharCode(parseInt('0x' + p1));
        }));
    };

    static downloadFile(myFileName: string, content: string) {
        var data = "data:text/csv;charset=utf-8;base64," + this.b64EncodeUnicode('\ufeff' + content);

        var now = new Date();
        var datepart = now.getFullYear() + "" + now.getMonth() + "" + now.getDate() + "" + now.getHours() + "" + now.getMinutes() + "" + now.getSeconds();
        var fileName = myFileName + datepart + '.xls';

        var element = document.createElement('a');
        element.setAttribute('target', '_blank');
        element.setAttribute('download', fileName);
        element.setAttribute('href', data);
        element.click();
    }
}

export interface IData {
    [key: string]: any;
}

export interface TableChartInfo {
    showBar: boolean,
    showLegend: boolean;
    barChartType: string;
    barChartHeight: number;

    data: any[];

    label: string[],

    showPie: boolean,
    pieData: number[],
    pieLabel: string[],

    toggleBar(): void,
    togglePie(useBarDataIndex: number): void
}
export class TableChartInfoClass implements TableChartInfo {
    constructor(dataNumber: number) {
        this.data = [];
        for (var i = 0; i < dataNumber; i++) {
            this.data.push({ data: [], label: '' });
        }
    }
    showBar = false;
    showLegend = true;
    barChartType = "horizontalBar";
    barChartHeight = 200;

    data: any[] = [];
    label: string[] = [];

    showPie = false;
    pieData: number[] = [];
    pieLabel: string[] = [];

    toggleBar() {
        this.showBar = !this.showBar;
    }
    togglePie(useBarDataIndex: number) {
        if (this.pieData != this.data[useBarDataIndex].data) {
            this.pieLabel = this.label;
            this.pieData = this.data[useBarDataIndex].data;
            this.showPie = true;
            return;
        }

        if (this.showPie) {
            this.pieLabel = [];
            this.pieData = [];
        }
        else {
            this.pieLabel = this.label;
            this.pieData = this.data[useBarDataIndex].data;
        }
        this.showPie = !this.showPie;
    }

    addDataSetLabel(labels: string[], langs: IData) {
        for (var i = 0; i < this.data.length; i++) {
            if (i < labels.length) {
                this.data[i].label = langs[labels[i]];
            }
        }
    }

    getTotal(dataIndex: number) {
        var total: number = 0;
        if (dataIndex >= this.data.length) {
            return 0;
        }
        for (var i = 0; i < this.data[dataIndex].data.length; i++) {
            total += this.data[dataIndex].data[i];
        }
        return total;
    }

    getAvg(dataIndex: number) {
        var total: number = 0;
        if (dataIndex >= this.data.length) {
            return 0;
        }

        var dataLength = this.data[dataIndex].data.length;
        if (dataLength == 0) {
            return 0;
        }

        for (var i = 0; i < dataLength; i++) {
            total += this.data[dataIndex].data[i];
        }
        return Math.round(total / dataLength);
    }

    getTotalPercent(targetIndex: number, actualIndex: number) {
        var targetTotal: number = this.getTotal(targetIndex);
        var actualTotal: number = this.getTotal(actualIndex);

        if (targetTotal == 0) {
            return 0;
        }

        var percent: number = Math.round((actualTotal * 100) / targetTotal);
        return percent;
    }

    resetAllData() {
        for (var i = 0; i < this.data.length; i++) {
            this.data[i].data = [];
        }
    }

    static getColumnExportString(columns: string[], headers: IData): string {
        let row = '<tr>';

        for (var i = 0; i < columns.length; i++) {
            row += '<td>' + headers[columns[i]] + '</td>';
        }
        row += '</tr>';

        return row;
    }
}

export interface EmailTemplate {
    emailTemplateID: number;
    subject: string;
    message: string;
    isForMonthStatusNotification: boolean;
}

export class EmailTemplateClass {
    static createNew(): EmailTemplate {
        var newEmailTemplate: EmailTemplate = {
            emailTemplateID: 0,
            subject: '',
            message: '',
            isForMonthStatusNotification: false
        };
        return newEmailTemplate;
    }

    static clone(record: EmailTemplate): EmailTemplate {
        var newrecord: EmailTemplate = {
            emailTemplateID: record.emailTemplateID,
            subject: record.subject,
            message: record.message,
            isForMonthStatusNotification: record.isForMonthStatusNotification,
        };
        return newrecord;
    }

    static assign(recordFrom: EmailTemplate, recordTo: EmailTemplate) {
        recordTo.emailTemplateID = recordFrom.emailTemplateID;
        recordTo.subject = recordFrom.subject;
        recordTo.message = recordFrom.message;
        recordTo.isForMonthStatusNotification = recordFrom.isForMonthStatusNotification;
    }
}

