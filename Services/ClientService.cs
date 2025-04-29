using MySql.Data.MySqlClient;
using NuGet.DependencyResolver;
using Tutorial8.Models.DTOs;
using Tutorial8.Repository;

namespace Tutorial8.Services;

public class ClientService : IClientService
{
    private readonly IClientRepository _clientRepository;
    private readonly ITripsRepository _tripsRepository;
    
    public ClientService(IClientRepository clientRepository, ITripsRepository tripsRepository)
    {
        _clientRepository = clientRepository;
        _tripsRepository = tripsRepository;
    }

    // wyswietl wszystkie wycieczki zwiazane z klientem
    public async Task<List<TripClientDTO>> GetClientTrips(int clientId)
    {
        var list = await _clientRepository.GetClientTrips(clientId);
        return list;
    }
    

    // dodaj klienta
    public async Task<int> AddClient(ClientDTO clientDto)
    {
        var id = await _clientRepository.AddClient(clientDto);
        return id;
    }

    // dodaj klienta i wycieczke zwiazana z nim
    public async Task<bool> AddClientTrip(int clientId, int tripId)
    {
        var clientExists = await _clientRepository.CheckIfClientExists(clientId);
        var tripExists = await _tripsRepository.CheckIfTripExists(tripId);
        var maxPeople = await _tripsRepository.GetTripMaxCount(tripId);
        var currentPeopleCount = await _tripsRepository.GetTripPeopleCount(tripId);
        bool allow = clientExists & tripExists & (currentPeopleCount < maxPeople);

        bool result = false;
        if (allow)
        {
            result = await _clientRepository.AddClientTrip(clientId, tripId);
        }

        return result;
    }

    
    // usun wycieczke klienta
    public async Task<bool> DeleteClientTrip(int clientId, int tripId)
    {
        bool result = false;
        var tripExists = await _clientRepository.CheckIfClientTripExists(clientId, tripId);
        
        if (tripExists)
        {
            result = await _clientRepository.DeleteClientTrip(clientId, tripId);
        }

        return result;
    }
}