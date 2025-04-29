using System.Data;
using MySql.Data.MySqlClient;
using Tutorial8.Models.DTOs;


namespace Tutorial8.Repository;

public class ClientRepository : Repository, IClientRepository
{
    public ClientRepository(IConfiguration configuration) : base(configuration) {}
    
    
    // pobierz liste wycieczek zwiazanych z klientem, zwracamy liste zawierajaca szczegoly kazdej wycieczki i informacje o kliencie
    public async Task<List<TripClientDTO>> GetClientTrips(int clientId)
    {
        var trips = new List<TripClientDTO>();

        // pobierz szczegoly wycieczki razem z informacjami o kliencie (data platnosci, data rejestracji wycieczki)
        // staralem sie unikac problemu n+1, dlatego czasami jest kilka wierszy z tym samym ID, ale roznymi krajami
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

                    // dodawanie krajow do istniejacej listy krajow wycieczki
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
                        // pobieranie danych
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
                        
                        // data platnosci moze byc null
                        if (!reader1.IsDBNull(paymentOrdinal))
                        {
                            paymentDate = reader1.GetDateTime(paymentOrdinal);
                        }
                        else paymentDate = null;
                        DateTime registered = reader1.GetDateTime(registeredOrdinal);
                    
                        
                        // dodaj nowe DTO...
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

    
    // wstaw klienta do bazy danych, zwracamy jego ID
    public async Task<int> AddClient(ClientDTO clientDto)
    {
        
        // wstaw klienta i pobierz ostatnie wstawione ID klienta
        string command =
            "INSERT INTO Client (FirstName, LastName, Email, Telephone, Pesel) VALUES(@FirstName, @LastName, @Email, @Telephone, @Pesel); SELECT LAST_INSERT_ID();";

        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("FirstName", clientDto.FirstName);
            cmd.Parameters.AddWithValue("LastName", clientDto.LastName);
            cmd.Parameters.AddWithValue("Email", clientDto.Email);
            cmd.Parameters.AddWithValue("Telephone", clientDto.Telephone);
            cmd.Parameters.AddWithValue("Pesel", clientDto.Pesel);

            await conn.OpenAsync();
            
            // zwracamy ID klienta
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }

    // sprawdz czy klient istnieje w bazie danych, zwraca 1 jesli istnieje, 0 jestli nie istnieje
    public async Task<bool> CheckIfClientExists(int clientId)
    {
        // zwroc 1 jesli klient istnieje
        string command = "SELECT 1 FROM CLIENT WHERE IdClient = @IdClient";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdClient", clientId);
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
    
    public async Task<bool> AddClientTrip(int clientId, int tripId)
    {
        string command = "INSERT INTO Client_Trip(IdClient, IdTrip, RegisteredAt, PaymentDate) VALUES(@IdClient, @IdTrip, @Registered, @Payment)";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdClient", clientId);
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            cmd.Parameters.AddWithValue("Registered", DateTime.Now);
            cmd.Parameters.AddWithValue("Payment", null);
            await conn.OpenAsync();

            try
            {
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0) return true;
            }
            catch (MySqlException exc)
            {
                return false;
            }
        }

        return false;
    }
    
    
    // sprawdz czy jest zarejestrowana wycieczka dla danego klienta
    public async Task<bool> CheckIfClientTripExists(int clientId, int tripId)
    {
        // zwroc 1 jesli jest zarejestrowana taka wycieczka
        string command = "SELECT 1 FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdClient", clientId);
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

    // usun rekord z tabeli Client_Trip, zwraca true jesli operacja sie udala, inaczej zwraca false
    public async Task<bool> DeleteClientTrip(int clientId, int tripId)
    {
        // usun rekord z tabeli
        string command = "DELETE FROM Client_Trip WHERE IdClient = @IdClient AND IdTrip = @IdTrip";
        using (MySqlConnection conn = new MySqlConnection(_connectionString))
        using (MySqlCommand cmd = new MySqlCommand(command, conn))
        {
            cmd.Parameters.AddWithValue("IdClient", clientId);
            cmd.Parameters.AddWithValue("IdTrip", tripId);
            await conn.OpenAsync();

            try
            {
                var rowsAffected = await cmd.ExecuteNonQueryAsync();
                if (rowsAffected > 0) return true;
            }
            catch (MySqlException exc)
            {
                return false;
            }

            
        }

        // inaczej zwroc 0
        return false;
    }
}