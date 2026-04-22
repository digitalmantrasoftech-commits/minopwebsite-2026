using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PayTimeWebClient.Models
{
    public class WorkPolicy
    {
        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Policy Name is required")]
        [StringLength(50, ErrorMessage = "Policy Name cannot exceed 50 characters")]
        public string PolicyName { get; set; }

        // Employment Settings (Same as HR Policy)
        public int EmpWeekOff { get; set; }
        public int EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public int EmpHalfDay { get; set; }
        public string EmpHalfDayRule { get; set; }

        // Overtime Settings (Simplified for Field Sense)
        [Range(0, 24, ErrorMessage = "Minimum OT hour must be between 0 and 24")]
        public int OTMinHour { get; set; }
        public int OTAllowWO { get; set; } // 0 or 1 for checkbox
        [Range(0, 24, ErrorMessage = "Week off OT hour must be between 0 and 24")]
        public int WeekOffOTHour { get; set; }
        public int OTAllowHO { get; set; } // 0 or 1 for checkbox
        [Range(0, 24, ErrorMessage = "Holiday OT hour must be between 0 and 24")]
        public int HolidayOffOTHour { get; set; }

        // Field Sense Configuration
        [Required(ErrorMessage = "Default Radius is required")]
        [Range(50, 500, ErrorMessage = "Default Radius must be between 50 and 500 meters")]
        public int DefaultRadius { get; set; }

        [Required(ErrorMessage = "Location Update Frequency is required")]
        public int LocationUpdateFrequency { get; set; } // in minutes: 2, 5, 10, 15, 30

        public bool EmpCanCreateTask { get; set; }
        public bool AutoCheckIn { get; set; }

        public int ProofOfWork { get; set; } // 0: None, 1: Photo, 2: Signature, 3: Photo+Signature, 4: OTP

        // Audit Fields
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class AddWorkPolicy
    {

        public int PolicyId { get; set; }

        [Required(ErrorMessage = "Policy Name is required")]
        [StringLength(50, ErrorMessage = "Policy Name cannot exceed 50 characters")]
        public string PolicyName { get; set; }

        // Employment Settings (Same as HR Policy)
        public int EmpWeekOff { get; set; }
        public int EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public int EmpHalfDay { get; set; }
        public string EmpHalfDayRule { get; set; }

        // Overtime Settings (Simplified for Field Sense)
        [Range(0, 24, ErrorMessage = "Minimum OT hour must be between 0 and 24")]
        public int OTMinHour { get; set; }
        public bool OTAllowWO { get; set; } // Boolean for API compatibility
        [Range(0, 24, ErrorMessage = "Week off OT hour must be between 0 and 24")]
        public int WeekOffOTHour { get; set; }
        public bool OTAllowHO { get; set; } // Boolean for API compatibility
        [Range(0, 24, ErrorMessage = "Holiday OT hour must be between 0 and 24")]
        public int HolidayOffOTHour { get; set; }

        // Field Sense Configuration
        [Required(ErrorMessage = "Default Radius is required")]
        [Range(50, 500, ErrorMessage = "Default Radius must be between 50 and 500 meters")]
        public int DefaultRadius { get; set; }

        [Required(ErrorMessage = "Location Update Frequency is required")]
        public int LocationUpdateFrequency { get; set; } // in minutes: 2, 5, 10, 15, 30

        public bool EmpCanCreateTask { get; set; }
        public bool AutoCheckIn { get; set; }

        public int ProofOfWork { get; set; } // 0: None, 1: Photo, 2: Signature, 3: Photo+Signature, 4: OTP

        // Audit Fields

    };

    // Update Work Policy Request (same structure as Add)
    public class UpdateWorkPolicy
    {
        [Required(ErrorMessage = "Policy Name is required")]
        [StringLength(50, ErrorMessage = "Policy Name cannot exceed 50 characters")]
        public string PolicyName { get; set; }

        // Employment Settings (Same as HR Policy)
        public int EmpWeekOff { get; set; }
        public int EmpSecondWeekOff { get; set; }
        public string EmpSecondWeekOffRule { get; set; }
        public int EmpHalfDay { get; set; }
        public string EmpHalfDayRule { get; set; }

        // Overtime Settings (Simplified for Field Sense)
        [Range(0, 24, ErrorMessage = "Minimum OT hour must be between 0 and 24")]
        public int OTMinHour { get; set; }
        public bool OTAllowWO { get; set; } // Boolean for API compatibility
        [Range(0, 24, ErrorMessage = "Week off OT hour must be between 0 and 24")]
        public int WeekOffOTHour { get; set; }
        public bool OTAllowHO { get; set; } // Boolean for API compatibility
        [Range(0, 24, ErrorMessage = "Holiday OT hour must be between 0 and 24")]
        public int HolidayOffOTHour { get; set; }

        // Field Sense Configuration
        [Required(ErrorMessage = "Default Radius is required")]
        [Range(50, 500, ErrorMessage = "Default Radius must be between 50 and 500 meters")]
        public int DefaultRadius { get; set; }

        [Required(ErrorMessage = "Location Update Frequency is required")]
        public int LocationUpdateFrequency { get; set; } // in minutes: 2, 5, 10, 15, 30

        public bool EmpCanCreateTask { get; set; }
        public bool AutoCheckIn { get; set; }

        public int ProofOfWork { get; set; } // 0: None, 1: Photo, 2: Signature, 3: Photo+Signature, 4: OTP
    };

    public class WorkPolicyViewModel
    {
        public WorkPolicy WorkPolicy { get; set; }
        public List<WorkPolicy> WorkPolicyList { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class WorkPolicyListResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<WorkPolicy> Data { get; set; }
        public int TotalCount { get; set; }
    }

    public class WorkPolicyResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public WorkPolicy Data { get; set; }
    }
}