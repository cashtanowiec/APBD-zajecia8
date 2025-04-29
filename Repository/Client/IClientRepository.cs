using Tutorial8.Models.DTOs;

namespace Tutorial8.Repository;

public interface IClientRepository
{
    Task<List<TripClientDTO>> GetClientTrips(int clientId);
    Task<int> AddClient(ClientDTO clientDto);
    Task<bool> CheckIfClientExists(int clientId);
    Task<bool> AddClientTrip(int clientId, int tripId);
    Task<bool> CheckIfClientTripExists(int clientId, int tripId);
    Task<bool> DeleteClientTrip(int clientId, int tripId);
}