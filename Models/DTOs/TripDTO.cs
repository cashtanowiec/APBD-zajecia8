namespace Tutorial8.Models.DTOs;

public class TripDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public String Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDTO> Countries { get; set; }
}

public class TripClientDTO
{
    public TripDTO TripDto { get; set; }
    public TripInfoDTO TripInfo { get; set; }
}

public class CountryDTO
{
    public string Name { get; set; }
}

public class TripInfoDTO
{
    public DateTime RegisteredAt { get; set; }
    public DateTime? PaymentDate { get; set; }
}