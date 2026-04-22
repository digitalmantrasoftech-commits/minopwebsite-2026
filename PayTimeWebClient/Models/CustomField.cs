
namespace PayTimeWebClient.Models
{
    public class CustomField
    {
        public int CustomFieldID { get; set; }
        public int FieldTypeID { get; set; }
        public int CustomPageID { get; set; }
        public string FieldLabel { get; set; }
        public string FieldID { get; set; }
        public string FieldClass { get; set; }
        public string FieldPlaceHolder { get; set; }
        public bool Iscompulsory { get; set; }
        public int SequenceNo { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public int ModifyBy { get; set; }
        public string ModifyDate { get; set; }
    }
}