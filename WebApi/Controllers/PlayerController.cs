// /////////////////////////////////////////////////////////////////////////////
// YOU CAN FREELY MODIFY THE CODE BELOW IN ORDER TO COMPLETE THE TASK
// /////////////////////////////////////////////////////////////////////////////

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTO;
using WebApi.Entities;
using WebApi.Interface;

[ApiController]
[Route("api/[controller]")]
public class PlayerController : ControllerBase
{
    private readonly IPlayersServices _playersServices;


    public PlayerController(IPlayersServices playersServices)
    {
        _playersServices = playersServices;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetAll()
    {

        var response = await _playersServices.GetAllAsync();
        return Ok(response);

    }

    [HttpPost]
    public async Task<ActionResult<Player>> PostPlayer([FromBody] CreatePlayerDto createPlayerDto)
    {

        var player = await _playersServices.CreatePlayerAsync(createPlayerDto);

        return Ok(player);
    }


    [HttpPut("{id}")]
    public async Task<ActionResult> PutPlayer(int id, [FromBody] CreatePlayerDto playerDto)
    {
        var existingPlayer = await _playersServices.UpdatePlayerAsync(id, playerDto);
        return Ok(existingPlayer);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult<Player>> DeletePlayer(int id)
    {
        var player = await _playersServices.DeletePlayerAsync(id);
        return NoContent();

    }
}