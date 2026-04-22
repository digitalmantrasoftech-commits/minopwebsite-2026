using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
    public class TransactionData
    {
        public long txnId { get; set; }
        public int dvcId { get; set; }
        public string dvcIP { get; set; }
        public int punchId { get; set; }
        public string txnDateTime { get; set; }
        public string mode { get; set; }
        public bool isSync { get; set; }
        public string LastActivity { get; set; }
        public string EntryDate { get; set; }

        public string Remarks { get; set; }

        public string RemarksDate { get; set; }
        public IEnumerable<TransactionData> TransactionList { get; set; }
        
    }
}