using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.Models.DTOs;
using Tutorial8.Services;

namespace Tutorial8.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }


    // Zad. 2
    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTrips(int id)
    {
        var trips = await _clientService.GetClientTrips(id);
        if (trips.IsNullOrEmpty()) return NotFound();
        return Ok(trips);
    }

    // Zad.3
    [HttpPost]
    public async Task<IActionResult> AddClient(ClientDTO client)
    {
        // walidacja danych jest wykonywana w ClientDTO
        var id = await _clientService.AddClient(client);
        return Ok(id);
    }

    
    // Zad. 4
    [HttpPut("{clientId}/trips/{tripId}")]
    public async Task<IActionResult> AddClientTrip(int clientId, int tripId)
    {
        var result = await _clientService.AddClientTrip(clientId, tripId);
        if (result) return NoContent();
        return BadRequest("Something is wrong with input data!");
    }
    
    
    // Zad. 5
    [HttpDelete("{clientId}/trips/{tripid}")]
    public async Task<IActionResult> DeleteClientTrip(int clientId, int tripId)
    {
        var result = await _clientService.DeleteClientTrip(clientId, tripId);
        if (result) return NoContent();
        return BadRequest("Something is wrong with input data!");
    }
}