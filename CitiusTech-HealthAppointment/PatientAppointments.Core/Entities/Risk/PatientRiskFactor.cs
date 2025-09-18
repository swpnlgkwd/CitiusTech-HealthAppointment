using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Entities.Risk
{
    public class PatientRiskFactor : BaseEntity
    {
        [Key]
        public int RiskId { get; set; }

        public int PatientId { get; set; }

        public int RiskTypeId { get; set; }

        public int RiskLevelId { get; set; }

        public DateTime IdentifiedOn { get; set; } = DateTime.UtcNow.Date;
    }
}
