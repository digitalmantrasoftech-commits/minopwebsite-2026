
namespace PayTimeWebClient.Models
{
    public class Devicecommand
    {
        public int Devicecmdid { get; set; }
        public int Deviceid { get; set; }
        public string DeviceCode { get; set; }
        public string Commandid { get; set; }
        public string Commandname { get; set; }
        public string Commanddate { get; set; }
        public string Commandexecutedate { get; set; }
        public int IsExecuted { get; set; }
        public string RespSatus { get; set; }
        public string Responce { get; set; }
        public string Commandpara { get; set; }
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }




    }
}