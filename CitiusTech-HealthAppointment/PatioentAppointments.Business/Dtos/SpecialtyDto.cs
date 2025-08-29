using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public class SpecialtyDto
    {
        public int SpecialityId { get; set; }

        public string SpecialtyName { get; set; } = string.Empty;
    }
}
