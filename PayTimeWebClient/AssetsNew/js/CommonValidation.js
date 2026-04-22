/// <reference path="CommonValidation.js" />
/*This function are mobile number Change event this are call in Companymaster,BranchMaster,Departmentmaster,Designatonmaster,EmployeeMaster,AccountSetting*/
function ValidateMobNumber(Mobile) {
    var fld = Mobile;
    var OutMessage = "";
    if (!(fld.length == 10)) {
        OutMessage = "Required 10 digits, match requested format!"
        return OutMessage;
    }
    return "success"
}
/*This function are EmailId Change event this are call in Companymaster,BranchMaster,Departmentmaster,Designatonmaster,EmployeeMaster,AccountSetting*/
function ValidateEmailid(EmailId) {
    const filter = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    var outmsg = "";
    if (!filter.test(EmailId)) {
        outmsg = "Please enter valid email id.";
        return outmsg;
    }
    return "success"
}
/*This function are CompanyURL Change event this are call in Companymaster,Wizard-Setting*/
function ValidateURL(CompanyURL) {
    var regUrl = /(http(s)?:\\)?([\w-]+\.)+[\w-]+[.com|.in|.org]+(\[\?%&=]*)?/;
    var regUrlmsg = "";
    if (!regUrl.test((CompanyURL))) {
        regUrlmsg = "Please enter valid url.";
        return regUrlmsg;
    }
    return "success"
}
/*This function are CompanyURL Change event this are call in Companymaster,Wizard-Setting*/
function ValidatePhone(PhoneNumber) {
    var OutMessage = "";
    if ((PhoneNumber.length < 6)) {
        OutMessage = "Required 6 digits, match requested format!"
        return OutMessage;
    }
    return "success"
}
/*This function are keypress event this are call in Companymaster,BranchMaster,Departmentmaster,Designatonmaster,EmployeeMaster
   ,Religionmaster,Devicemaster,Shiftmaster,LeaveModule,HolidayMaster,User Management,Utilities module,Policy,Alert*/
function validateKeyPress(keyinput) {
    var keyCode = keyinput;
    if (!((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90)
      || (keyCode >= 97 && keyCode <= 122)) && keyCode != 8 && keyCode != 32) {
        return false;
    }
    return true;
}
/*This is call AccountSetting,Companymaster,BranchMaster,Employeemaster*/
function validateAddress(Address) {
    var keyCode = Address;
    if (!((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90)
        || (keyCode >= 97 && keyCode <= 122)) && keyCode != 8 && keyCode != 32 && keyCode != 45 && keyCode != 58 && keyCode != 47
        && keyCode != 44 && keyCode != 46) {
        return false;
    }
    return true;
}
/* This function is called when the remove the blanck sapce */
function removeTags(string) {
    return string.replace(/<[^>]*>/g, ' ')
                 .replace(/\s{2,}/g, ' ')
    .trim();
}
function validateDateFormate(SelectedDate, isdateformat) {
    var SystemDateFormate = "";
    if (isdateformat != '' && SelectedDate != '') {
        if (isdateformat == 'dd-mm-yyyy') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "dd-mm-yyyy"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            /*
            array position[0]:date,position[1]:Month,position[2]:year
            */
            if (array[0] <= 31) {
                if (array[1] <= 12) { }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            if (array[1] <= 12) { }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            var newdate1 = SelectedDate.split("-").reverse().join("-");
            SystemDateFormate = '' + newdate1 + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var Day = date.getDate();
            if (month.toLocaleString().length < 2)
            { month = '0' + month; }
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = Day + '-' + month + '-' + year;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
        else if (isdateformat == 'yyyy-mm-dd') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "yyyy-mm-dd"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            if (array[2] <= 31) {
                if (array[1] <= 12) {
                }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            SystemDateFormate = '' + newDate + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var Day = date.getDate();
            if (month.toLocaleString().length < 2)
            { month = '0' + month; }
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = year + '-' + month + '-' + Day;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
        else if (isdateformat == 'mm-dd-yyyy') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "mm-dd-yyyy"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            if (array[1] <= 31) {
                if (array[0] <= 12) {
                }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            SystemDateFormate = '' + newDate + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var month = date.getMonth() + 1;
            var Day = date.getDate();
            if (month.toLocaleString().length < 2)
            { month = '0' + month; }
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = month + '-' + Day + '-' + year;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
        else if (isdateformat == 'dd-M-yyyy') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "dd-mm-yyyy"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            /*
            array position[0]:date,position[1]:Month,position[2]:year
            */
            if (array[0] <= 31) {
                var getmonth = array[1];
                if (getmonth == GetMonths(getmonth)) { }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            var getmonth = array[1];
            if (getmonth == GetMonths(getmonth)) { }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            var newdate1 = SelectedDate.split("-").reverse().join("-");
            SystemDateFormate = '' + newdate1 + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var locale = "en-us";
            var month = date.toLocaleString(locale, { month: "short" })
            var Day = date.getDate()
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = Day + '-' + month + '-' + year;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
        else if (isdateformat == 'yyyy-M-dd') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "yyyy-M-dd"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            if (array[2] <= 31) {
                var getmonth = array[1];
                if (getmonth == GetMonths(getmonth)) { }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            SystemDateFormate = '' + newDate + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var locale = "en-us";
            var month = date.toLocaleString(locale, { month: "short" })
            var Day = date.getDate();
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = year + '-' + month + '-' + Day;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
        else if (isdateformat == 'M-dd-yyyy') {
            var array = new Array();
            //split string and store it into array
            //from array concatenate into new date string format: "mm-dd-yyyy"
            var newDate = SelectedDate;
            array = SelectedDate.split('-');
            var getmonth = array[0];
            if (array[1] <= 31) {
                if (getmonth == GetMonths(getmonth)) { }
                else { return SystemDateFormate = ""; }
            }
            else { return SystemDateFormate = ""; }
            if (array[0] != '' && array[1] != undefined && array[2] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1] + "-" + array[2]);
            }
            else if (array[0] != '' && array[1] != undefined) {
                newDate = '';
                newDate = (array[0] + "-" + array[1]);
            }
            else if (array[0] != '') {
                newDate = array[0];
            }
            else { newDate = SelectedDate; }
            SystemDateFormate = '' + newDate + '';
            var date = new Date(SystemDateFormate);
            var year = date.getFullYear();
            var locale = "en-us";
            var month = date.toLocaleString(locale, { month: "short" })
            var Day = date.getDate();
            if (month.toLocaleString().length < 2)
            { month = '0' + month; }
            if (Day.toLocaleString().length < 2)
            { Day = '0' + Day; }
            if ((Day.toString() != "NaN") && (month.toString() != "NaN") && (year.toString() != "NaN")) {
                SystemDateFormate = month + '-' + Day + '-' + year;
            }
            else { SystemDateFormate = ""; }
            return SystemDateFormate;
        }
    }


    return SystemDateFormate;
} (50);
function getDateformatSelected(isdateformat, data) {
    // for the adding  dateformat of  dd-MMM-YYYY ,MMM-dd-YYYY,YYYY-MMM-dd.
    var date = new Date(data);
    var locale = "en-us";
    var month = date.toLocaleString(locale, { month: "short" });
    var day = date.getDate();
    if (isdateformat == "yyyy-M-dd") {
        return date.getFullYear() + "-" + month + "-" + (day > 9 ? day : "0" + day);
    }
    else if (isdateformat == "M-dd-yyyy") {
        return month + "-" + (day > 9 ? day : "0" + day) + "-" + date.getFullYear();
    }
    else if (isdateformat == "dd-M-yyyy") {
        return (day > 9 ? day : "0" + day) + "-" + month + "-" + date.getFullYear();
    }
    else {
        var month = date.getMonth() + 1;
        if (month < 10) {
            month = '0' + month;
        }
        return date.getFullYear() + "-" + month + "-" + (day > 9 ? day : "0" + day);
    }
}
function getDateformatSelectedmonth(isdateformat, data) {
    // for the adding  dateformat of  dd-MMM-YYYY ,MMM-dd-YYYY,YYYY-MMM-dd.
    var date = new Date(data);
    var locale = "en-us";
    var month = date.toLocaleString(locale, { month: "short" });
    var day = date.getDate();
    if (isdateformat == "yyyy-M-dd") {
        return date.getFullYear() + "-" + month + "-" + (day > 9 ? day : "0" + day) + " " + (date.getHours() > 10 ? date.getHours() : "0" + date.getHours()) + ":" + (date.getMinutes() > 10 ? date.getMinutes() : "0" + date.getMinutes()) + ":" + (date.getSeconds() > 10 ? date.getSeconds() : "0" + date.getSeconds());
    }
    else if (isdateformat == "M-dd-yyyy") {
        return month + "-" + (day > 9 ? day : "0" + day) + "-" + date.getFullYear() + " " + (date.getHours() > 10 ? date.getHours() : "0" + date.getHours()) + ":" + (date.getMinutes() > 10 ? date.getMinutes() : "0" + date.getMinutes()) + ":" + (date.getSeconds() > 10 ? date.getSeconds() : "0" + date.getSeconds());;
    }
    else if (isdateformat == "dd-M-yyyy") {
        return (day > 9 ? day : "0" + day) + "-" + month + "-" + date.getFullYear() + " " + (date.getHours() > 10 ? date.getHours() : "0" + date.getHours()) + ":" + (date.getMinutes() > 10 ? date.getMinutes() : "0" + date.getMinutes()) + ":" + (date.getSeconds() > 10 ? date.getSeconds() : "0" + date.getSeconds());;
    }
    else {
        var month = date.getMonth() + 1;
        if (month < 10) {
            month = '0' + month;
        }
        return date.getFullYear() + "-" + month + "-" + (day > 9 ? day : "0" + day);
    }
}
/* this function is used in _report filter page to get the note of report filter in a  dd-MMM-yyy ,MMM-dd-yyyy,yyyy-MMM-dd format.*/
function GetMonthName(monthNumber) {
    var months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'];
    return months[monthNumber - 1];
}
/* this function is used in a employee master in resigndate to get the date of monthname to the  month number. */
function GetMonthNumber(monthName) {
    var dateHash = {
        Jan: 1,
        Feb: 2,
        Mar: 3,
        Apr: 4,
        May: 5,
        Jun: 6,
        Jul: 7,
        Aug: 8,
        Sep: 9,
        Oct: 10,
        Nov: 11,
        Dec: 12
    };
    return dateHash[monthName];
}
/*this function is used for the compare the month name in a validatedateformat function*/
function GetMonths(monthname) {
    var datehash = {
        "Jan": "Jan",
        "Feb": "Feb",
        "Mar": "Mar",
        "Apr": "Apr",
        "May": "May",
        "Jun": "Jun",
        "Jul": "Jul",
        "Aug": "Aug",
        "Sep": "Sep",
        "Oct": "Oct",
        "Nov": "Nov",
        "Dec": "Dec"

    }
    return datehash[monthname];
}
/*This function is called when the user time spend by the particular Webpage.*/
function timeSpentOnPage() {
    var timeSpentOnPage = TimeMe.getTimeOnCurrentPageInSeconds();
    var timeSpentOnTransactionYearPage = timeSpentOnPage.toFixed(2);
    return timeSpentOnTransactionYearPage;
}
/* TimeMeintialize javascrpit is called user time spend by the webpage.*/
function timeMeInitialize() {
    TimeMe.initialize({
        currentPageName: "my-home-page", // current page
        idleTimeoutInSeconds: 5, // stop recording time due to inactivity
    });
}
/* Analyticsdatausage function save dbname of the particular user, form name of the current user usage and so on.*/
function analyticsDataUsage(url, dbname, form, tokan, count, operation, timeSpent) {
    var obj = {
        'DbName': dbname,
        'FormName': form,
        'TimeSpend': timeSpent,
        'Operation': operation,
        'count': count
    }
    $.ajax({
        type: 'POST',
        url: url,
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(obj),
        headers: { 'Authorization': tokan },
        dataType: "json",
        async: "false",
    });
}
function astMobileno(mobileno) {
    
    return mobileno;
}
function obfuscateEmail(email) {

    
return email;
}
function GetMonthFullName(month)
{
    const months = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
    ];
    return months[month];
}

