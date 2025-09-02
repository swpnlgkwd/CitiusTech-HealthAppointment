using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientAppointments.Business.Dtos
{
    public class AgentSummaryResponseDto
    {
        public string SummaryMessage { get; set; } = string.Empty;

        public IEnumerable<QuickReplyDto>  QuickReplies { get; set; } = Enumerable.Empty<QuickReplyDto>();
    }
}
