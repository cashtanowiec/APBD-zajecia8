using System.Data;
using MySql.Data.MySqlClient;
using Tutorial8.Models.DTOs;
using Tutorial8.Repository;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly ITripsRepository _tripsRepository;

    public TripsService(ITripsRepository tripsRepository)
    {
        _tripsRepository = tripsRepository;
    }
    
    
    // wyswietl wszystkie wycieczki lacznie z lista krajow
    public async Task<List<TripDTO>> GetTrips()
    {
        var list = await _tripsRepository.GetTrips();
        return list;
    }
    
}