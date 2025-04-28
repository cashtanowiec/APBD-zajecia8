using System.Data;
using MySql.Data.MySqlClient;
using Tutorial8.Models.DTOs;

namespace Tutorial8.Services;

public class TripsService : ITripsService
{
    private readonly string _connectionString;

    public TripsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    
    public async Task<List<TripDTO>> GetTrips()
    {
        var trips = new List<TripDTO>();

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

    public async Task<List<TripClientDTO>> GetClientTrips(int clientId)
    {
        var trips = new List<TripClientDTO>();

        string command =
            "SELECT t.IdTrip, t.Name, t.Description, t.DateFrom, t.DateTo, t.MaxPeople, c.Name AS CountryName, RegisteredAt, PaymentDate FROM Trip t JOIN Country_Trip ct ON t.IdTrip = ct.IdTrip JOIN Country c ON ct.IdCountry = c.IdCountry JOIN Client_Trip clt ON clt.IdTrip = t.IdTrip WHERE clt.IdClient = @IdClient ORDER BY t.IdTrip";
        
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            await conn.OpenAsync();
            cmd.Parameters.AddWithValue("IdClient", clientId);
            
            using (var reader1 = await cmd.ExecuteReaderAsync())
            {
                int lastId = -1;
                while (await reader1.ReadAsync())
                {
                    int idOrdinal = reader1.GetOrdinal("IdTrip");
                    int id = reader1.GetInt32(idOrdinal);

                    if (id == lastId)
                    {
                        int countryOrdinal = reader1.GetOrdinal("CountryName");
                        string country = reader1.GetString(countryOrdinal);
                        trips[trips.Count-1].TripDto.Countries.Add(new CountryDTO()
                        {
                            Name = country
                        });
                    }
                    else
                    {
                        lastId = id;
                        int nameOrdinal = reader1.GetOrdinal("Name");
                        int descriptionOrdinal = reader1.GetOrdinal("Description");
                        int dateFromOrdinal = reader1.GetOrdinal("DateFrom");
                        int dateToOrdinal = reader1.GetOrdinal("DateTo");
                        int maxOrdinal = reader1.GetOrdinal("MaxPeople");
                        int countryOrdinal = reader1.GetOrdinal("CountryName");
                        int paymentOrdinal = reader1.GetOrdinal("PaymentDate");
                        int registeredOrdinal = reader1.GetOrdinal("RegisteredAt");

                    
                        string name = reader1.GetString(nameOrdinal);
                        string description = reader1.GetString(descriptionOrdinal);
                        DateTime dateFrom = reader1.GetDateTime(dateFromOrdinal);
                        DateTime dateTo = reader1.GetDateTime(dateToOrdinal);
                        int max = reader1.GetInt32(maxOrdinal);
                        string country = reader1.GetString(countryOrdinal);
                        DateTime? paymentDate;
                        if (!reader1.IsDBNull(paymentOrdinal))
                        {
                            paymentDate = reader1.GetDateTime(paymentOrdinal);
                        }
                        else paymentDate = null;
                        DateTime registered = reader1.GetDateTime(registeredOrdinal);
                    
                        List<CountryDTO> countries = new List<CountryDTO>();
                        countries.Add(new CountryDTO()
                        {
                            Name = country
                        });

                        TripDTO tripDto = new TripDTO()
                        {
                            Id = id,
                            Name = name,
                            Description = description,
                            DateFrom = dateFrom,
                            DateTo = dateTo,
                            MaxPeople = max,
                            Countries = countries
                        };

                        TripInfoDTO tripInfoDto = new TripInfoDTO()
                        {
                            PaymentDate = paymentDate,
                            RegisteredAt = registered
                        };
                        
                        trips.Add(new TripClientDTO()
                        {
                            TripDto = tripDto,
                            TripInfo = tripInfoDto
                        });
                    }
                }
            }
        }

        return trips;
    }
}