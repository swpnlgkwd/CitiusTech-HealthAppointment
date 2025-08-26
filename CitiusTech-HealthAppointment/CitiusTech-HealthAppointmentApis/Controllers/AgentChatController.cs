using Azure.AI.Agents.Persistent;
using CitiusTech_HealthAppointmentApis.Agent.Services;
using CitiusTech_HealthAppointmentApis.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitiusTech_HealthAppointmentApis.Controllers
{

    [ApiController]
    //[Route("api/[controller]")]
    //[Authorize]
    public class AgentChatController : ControllerBase
    {
        public readonly IAgentService _agentService;

        public AgentChatController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        /// <summary>
        /// Sends a user message to the persistent agent and returns the agent's response.
        /// </summary>
        /// <param name = "request" > The message input from the user.</param>
        /// <returns>A response from the agent or a bad request if the response is invalid.</returns>
        [HttpPost("ask")]
        public async Task<IActionResult> AskAgent([FromBody] UserMessageRequestDto request)
        {
            var response = await _agentService.GetAgentResponseAsync(MessageRole.User, request.Message);
            if (response is MessageTextContent textResponse)
            {
                return Ok(new { reply = textResponse.Text });
            }

            return BadRequest("No valid response from agent.");
        }
    }
}
