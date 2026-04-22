using System;
using System.Collections.Generic;

namespace PayTimeWebClient.Models
{
	public class TransactionYear
	{
        //public int TransactionYearId { get; set; }
        //public string TransactionYearName { get; set; }
        //public Int32 FromYear { get; set; }
        //public string FromDate { get; set; }
        //public string FromMonth { get; set; }
        //public Int32 ToYear { get; set; }
        //public string ToDate { get; set; }
        //public string ToMonth { get; set; }
        //public bool IsActive { get; set; }
		public int TransactionYearId { get; set; }
		public string TransactionYearName { get; set; }
        public int FromYear { get; set; }
        public string FromDate { get; set; }
        public int ToYear { get; set; }
        public string ToDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public int ModifyBy { get; set; }
		public IEnumerable<TransactionYear> TransactionYearList { get; set; }
	}
 
}