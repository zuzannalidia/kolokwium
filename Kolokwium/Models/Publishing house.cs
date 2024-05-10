using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class Publishing_house
{
    public int IdHouse { get; set; }
    [MaxLength(100)] 
    public string HouseName { get; set; }
    [MaxLength(50)]
    public string OwnerFirstName { get; set; }
    [MaxLength(100)]
    public string OwnerLastName { get; set; }
}