using System.ComponentModel.DataAnnotations;

namespace Kolokwium.Models;

public class Authors
{
    public int IdAuthor { get; set; }
    [MaxLength(50)]
    public string FirstName { get; set; }
    [MaxLength(100)]
    public string LastName { get; set; }
}