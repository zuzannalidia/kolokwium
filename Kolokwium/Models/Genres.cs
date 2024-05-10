using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class Genres
{
    public int IdGenre { get; set; }
    
    [MaxLength(100)]
    public string NameGenre { get; set; }
}