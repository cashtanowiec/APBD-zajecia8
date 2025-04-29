using Tutorial8.Models.DTOs;

namespace Tutorial8.Repository;

public interface ITripsRepository
{
    Task<List<TripDTO>> GetTrips();
    Task<bool> CheckIfTripExists(int tripId);
    Task<int> GetTripMaxCount(int tripId);
    Task<int> GetTripPeopleCount(int tripId);
}