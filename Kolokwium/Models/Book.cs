using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class Book
{
    /// <summary>
    /// Klucz główny
    /// </summary>
    public int IdBook { get; set; }
    
    /// <summary>
    /// Nazwa książki
    /// </summary>
    [MaxLength(100)]
    public string Title { get; set; }
    
}