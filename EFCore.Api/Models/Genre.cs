using System.Text.Json.Serialization;

namespace EFCore.Api.Models;

public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // Changed to Shadow Property, see the mapping
    // [JsonIgnore]
    // public DateTime CreatedDate { get; set; }

    // Navigation property used to Query the relationship with EF
    // Add the JsonIgnore attribute to resolve the cyclical serialisation issue
    [JsonIgnore]
    public virtual ICollection<Movie> Movies { get; set; } = new HashSet<Movie>();
}