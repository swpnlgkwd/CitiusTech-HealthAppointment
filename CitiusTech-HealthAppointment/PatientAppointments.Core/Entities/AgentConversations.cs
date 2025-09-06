using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Core.Entities
{
    public class AgentConversations
    {
        public string user_id { get; set; } = string.Empty;

        public string thread_id { get; set; } = string.Empty;

        public DateTime created_at { get; set; }
    }
}
