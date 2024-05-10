using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class BookEdition
{
    public int IdEdition {get; set;}
    public int IdHouse { get; set; }
    public int IdBook { get; set; }
    [MaxLength(100)]
    public string EditionTitle { get; set; }
    //public string
}