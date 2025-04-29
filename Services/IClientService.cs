using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public interface IClientService
{
    Task<List<TripClientDTO>> GetClientTrips(int clientId);
    Task<int> AddClient(ClientDTO clientDto);
    Task<bool> AddClientTrip(int clientId, int tripId);
    Task<bool> DeleteClientTrip(int clientId, int tripId);
}