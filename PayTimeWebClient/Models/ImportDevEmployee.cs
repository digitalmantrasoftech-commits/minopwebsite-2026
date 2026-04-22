using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayTimeWebClient.Models
{
    public class ImportDevEmployee
    {
        public int EmpId { get; set; }
        public int EmpPunchID { get; set; }
        public string EmpName { get; set; }
        public int IsActive { get; set; }
        public IEnumerable<ImportDevEmployee> ImportedDevEmployeelist { get; set; }
    }

    public class ImportDevEmployeefaillog
    {
        public int ImportedFailId { get; set; }

        public int EmpId { get; set; }

        public string EmpPunchID { get; set; }
        public string EmpName { get; set; }

        public string Reason { get; set; }
        public IEnumerable<ImportDevEmployeefaillog> Importfailloglist { get; set; }
    }
}
