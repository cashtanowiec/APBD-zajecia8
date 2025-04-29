using System.Data;
using MySql.Data.MySqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Repository;

public class TripsRepository : Repository, ITripsRepository
{
    public TripsRepository(IConfiguration configuration) : base(configuration) {}


    // pobierz wszystkie wycieczki, zwracana jest lista wycieczek
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

        // zwroc informacje o wycieczkach wraz z przypisanymi krajami
        // staralem sie unikac problemu n+1, dlatego czasami jest kilka wierszy z tym samym ID, ale roznymi krajami
        string command =
            "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName FROM Trip t LEFT JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip LEFT JOIN Country c ON ct.IdCountry = c.IdCountry ORDER BY t.IdTrip";

        
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            await conn.OpenAsync();

            using (var reader1 = await cmd.ExecuteReaderAsync())
            {
                int lastId = -1;
                while (await reader1.ReadAsync())
                {
                    int idOrdinal = reader1.GetOrdinal("IdTrip");
                    int id = reader1.GetInt32(idOrdinal);

                    // dodawanie krajow do istniejacej listy krajow wycieczki
                    if (id == lastId)
                    {
                        int countryOrdinal = reader1.GetOrdinal("CountryName");
                        string country = reader1.GetString(countryOrdinal);
                        trips[trips.Count-1].Countries.Add(new CountryDTO()
                        {
                            Name = country
                        });
                    }
                    else
                    {
                        // pobieranie danych
                        lastId = id;
                        int nameOrdinal = reader1.GetOrdinal("Name");
                        int descriptionOrdinal = reader1.GetOrdinal("Description");
                        int dateFromOrdinal = reader1.GetOrdinal("DateFrom");
                        int dateToOrdinal = reader1.GetOrdinal("DateTo");
                        int maxOrdinal = reader1.GetOrdinal("MaxPeople");
                        int countryOrdinal = reader1.GetOrdinal("CountryName");

                    
                        string name = reader1.GetString(nameOrdinal);
                        string description = reader1.GetString(descriptionOrdinal);
                        DateTime dateFrom = reader1.GetDateTime(dateFromOrdinal);
                        DateTime dateTo = reader1.GetDateTime(dateToOrdinal);
                        int max = reader1.GetInt32(maxOrdinal);
                        string country = reader1.GetString(countryOrdinal);
                    
                        // dodawanie DTO...
                        List<CountryDTO> countries = new List<CountryDTO>();
                        countries.Add(new CountryDTO()
                        {
                            Name = country
                        });
                        
                        trips.Add(new TripDTO()
                        {
                            Id = id,
                            Name = name,
                            Description = description,
                            DateFrom = dateFrom,
                            DateTo = dateTo,
                            MaxPeople = max,
                            Countries = countries
                        });
                    }
                }
            }
        }
        return trips;
    }

    
    // sprawdz czy wycieczka istnieje w bazie danych, zwraca 1 jesli istnieje, 0 jesli nie istnieje
    public async Task<bool> CheckIfTripExists(int tripId)
    {
        // zwroc 1 jesli wycieczka istnieje
        string command = "SELECT 1 FROM TRIP WHERE IdTrip = @IdTrip";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                
                while (await reader.ReadAsync())
                {
                    // jesli zapytanie cokolwiek zwrocilo, zwroc 1
                    return true;
                }
            }
        }

        // inaczej zwroc 0
        return false;
    }

    // zwroc maksymalna ilosc osob wycieczki
    public async Task<int> GetTripMaxCount(int tripId)
    {
        string command = "SELECT MaxPeople FROM TRIP WHERE IdTrip = @IdTrip";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                
                while (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
            }
        }

        return 0;
    }

    // sluzy do sprawdzania maksymalnej ilosci osob na wycieczce, zwraca ilosc osob zarejestrowanych do danej wycieczki
    public async Task<int> GetTripPeopleCount(int tripId)
    {
        
        // zwroc ilosc osob zarejestrowanych z dana wycieczka
        string command = "SELECT COUNT(*) FROM TRIP JOIN Client_Trip ON Trip.IdTrip = Client_Trip.IdTrip WHERE Client_Trip.IdTrip = @IdTrip";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            await conn.OpenAsync();

            using (var reader = await cmd.ExecuteReaderAsync())
            {
                
                while (await reader.ReadAsync())
                {
                    return reader.GetInt32(0);
                }
            }
        }

        return 0;
    }

    
}