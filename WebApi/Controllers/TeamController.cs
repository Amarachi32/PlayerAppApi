// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Interface;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamServices _teamServices;

        public TeamController(ITeamServices teamServices)
        {

            _teamServices = teamServices;
        }

        [HttpPost("process")]
        public async Task<ActionResult<IEnumerable<Player>>> SelectTeams([FromBody] List<TeamRequirement> requirements)
        {

            var selectedPlayers = await _teamServices.SelectTeamsAsync(requirements);

            return Ok(selectedPlayers);
        }

    }
}
