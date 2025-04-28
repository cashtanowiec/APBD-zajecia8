using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Tutorial8.Services;

namespace Tutorial8.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ClientsController : ControllerBase
{
    private readonly ITripsService _tripsService;

    public ClientsController(ITripsService tripsService)
    {
        _tripsService = tripsService;
    }


    [HttpGet("{id}/trips")]
    public async Task<IActionResult> GetTrips(int id)
    {
        var trips = await _tripsService.GetClientTrips(id);
        if (trips.IsNullOrEmpty()) return NotFound();
        return Ok(trips);
    }
}