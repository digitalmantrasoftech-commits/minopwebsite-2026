using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class Templates
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateDesc { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string ModifyBy { get; set; }
        public string ModifyDate { get; set; }

        public IEnumerable<Templates> TemplateList { get; set; }
    }
}