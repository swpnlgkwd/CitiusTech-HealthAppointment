using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Entities
{
    public class AppointmentType : BaseEntity
    {
        [Key]
        public int type_id { get; set; }

        public string type_name { get; set; } = string.Empty;
    }
}
