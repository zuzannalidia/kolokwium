namespace Kolokwium.Options;

public class DataOptions
{
    /// <summary>
    /// Połączenie do bazy danych
    /// </summary>
    public string DbConnection { get; set; }
    
    /// <summary>
    /// Czy usunąć bazę danych przy starcie
    /// </summary>
    public bool DeleteDatabaseOnStart { get; set; }
}