using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class Audit
    {
        public int Type;
        public int tblid;
        public string FromDate;
        public string ToDate;
        public string UserEmail;

    }
    public class Employee
    {
        public string EmpName;
        public string Empcode;
        public string EmpPunchID;
        public string BranchName;
        public string UserEmail;
        public string status;
        public string ModifyBy;
        public string ModifyDate;
    }
    public class Device
    {
        public string BranchName;
        public string DeviceName;
        public string DeviceSrNo;
        public string DeviceCode;
        public string DeviceIP;
        public string DeviceMode;
        public string DeviceType;
        public string DeviceStatus;
        public string LastActivity;
        public string ModifyBy;
        public string ModifyDate;
    }
    public class Shift
    {
        public string Shiftname;
        public string ShiftShortName;
        public string StartTime;
        public string Endtime;
        public string  MinHrsHalfDay;
        public string MinHrsFullDay;
        public string RecessDur;
        public string ModifyBy;
        public string ModifyDate;
    }
    public  class Policy
    {
        public int PolicyId;
        public string PolicyName;
        public string ModifyBy;
        public string ModifyDate;
    }
    public class Attendance
    {
        public int EmpId;
        public DateTime punchdate;
        public string punchmode;
        public string punchtime;
        public string punchremark;
        public DateTime inoutduration;
        public string ModifyBy;
        public string ModifyDate;

    }
    public class HrpolicyAudit
    {
        #region Properties
        public int LPolicyId;
        public string LPolicyName;
        public int LEmpWeekOff;
        public int LEmpSecondWeekOff;
        public string LEmpSecondWeekOffRule;
        public int LEmpHalfDay;
        public string LEmpHalfDayRule;
        public int LEmpAllowOT;
        public int LMaxOutDur;
        public int LTimeBetPunch;
        public int LGraceLateIN;
        public int LGraceEarlyOUT;
        public string LApplicationMode;
        public int LMonthlyRptStaDay;
        public int LWeekOffOTHour;
        public int LHolidayOffOTHour;
        public int LOTMinHour;
        public int LOTFormula;
        public int LOTAllowWO;
        public int LOTAllowHO;
        public int LNotPresentInMnth;
        public int LAllErrorCase;
        public int LErrorCaseWH;
        public int LHdLate;
        public int LHdEd;
        public int LHdLateorEd;
        public int LALate;
        public int LAED;
        public int LALateorED;
        public int LTotalHrafterStatus;
        public int LTotalHrbeforeStatus;
        public int LLateInMin;
        public int LEDMin;
        public int LIssedwrule;
        public int LIssedwruleType;
        #endregion

    }
    public class branch
    {
        public string BranchName;
        public string ReportingBranchName;
        public string CompanyName;
        public string ModifyBy;
        public string ModifyDate;

    }
}