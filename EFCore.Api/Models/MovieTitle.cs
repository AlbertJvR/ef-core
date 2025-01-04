namespace EFCore.Api.Models;

// This class is used as a Projection to return only needed fields
public class MovieTitle
{
    public int Id { get; set; }
    public string? Title { get; set; }
}