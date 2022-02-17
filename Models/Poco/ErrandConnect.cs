using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class ErrandConnect //class for connecting errand table with other tables
    {
        public DateTime DateOfObservation { get; set; }
        public int ErrandId { get; set; }
        public string RefNumber { get; set; }
        public string TypeOfCrime { get; set; }
        public string StatusName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeName { get; set; }
    }
}
