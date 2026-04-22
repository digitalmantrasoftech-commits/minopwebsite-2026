using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PayTimeWebClient.Models.Notifications_Module
{
    public class NotificationAlertModel
    {

    }
    public class NotificationTemplateModel
    {
        //[Required]
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Language { get; set; }
        public List<string> Placeholders { get; set; } = new List<string> { "{user.name}", "{event.date}" };
        public List<SelectListItem> LanguageOptions { get; set; }
    }

    public class NotificationDetailsViewModel
    {
        public List<NotificationRule> Table1 { get; set; } // Notification headers
        public List<NotificationConditionRule> Table2 { get; set; }  // Notification steps or logs
        public List<NotificationEscalationRule> Table3 { get; set; } // Recipients or other related info
    }
    public class NotificationRule
    {
        public int NotifiId { get; set; }
        public string RuleName { get; set; }
        public string Description { get; set; }
        public int RecipientRoleType { get; set; }
        public bool ChannelsApp { get; set; }
        public bool ChannelsEmail { get; set; }
        public bool ChannelsPush { get; set; }
        public bool IsMandatory { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; } // 'Active' or 'InActive'
        public int EventID { get; set; }
        public string EventName { get; set; }
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string Recipients { get; set; } // RoleName
        public int RecipientRoleId { get; set; }
    }

    public class NotificationConditionRule
    {
        public int ConditionRuleID { get; set; }
        public int NotificationRuleID { get; set; }
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string FieldValue { get; set; }
        public string OperatorName { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int64 CreatedByID { get; set; }
    }


    public class NotificationEscalationRule
    {
        public int EscalateHours { get; set; }
        public int EscalationRuleID { get; set; }
        public int NotificationRuleID { get; set; }
       
        public int EscalateToRoleTypeID { get; set; }
        public string EscalateRoleNameID { get; set; }
        public DateTime CreatedDate { get; set; }
        public Int64 CreatedByID { get; set; }

        public string EscalateToRoleType { get; set; }

        public string EscalateRoleName { get; set; }
    }

    

}