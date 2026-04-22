
using System.Collections.Generic;
namespace PayTimeWebClient.Models
{
    public class EndPoints
    {
        public int Endpointid { get; set; }
        public string SayHelloURL { get; set; }
        public string TransactionDataURL { get; set; }
        public string New_SayHelloURL { get; set; }
        public string New_TransactionDataURL { get; set; }
        public IEnumerable<EndPoints> EndPointList { get; set; }
    }
}