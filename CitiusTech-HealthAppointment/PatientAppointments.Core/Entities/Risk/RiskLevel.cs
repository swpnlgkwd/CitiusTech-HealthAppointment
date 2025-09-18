using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Entities.Risk
{
    public class RiskLevel
    {
        [Key]
        public int RiskLevelId { get; set; }

        public string RiskLevelName { get; set; }
    }
}
