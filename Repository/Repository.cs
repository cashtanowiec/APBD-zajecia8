namespace Tutorial8.Repository;

public class Repository
{
    protected readonly string _connectionString;
    
    public Repository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }
}