namespace EFCore.Api.Models;

public class Movie
{
    public int Id { get; set; }
    public string? Title { get; set; }    
    public DateTime ReleaseDate { get; set; }
    public string? Synopsis { get; set; }
}

// This class is used as a Projection to return only needed fields
public class MovieTitle
{
    public int Id { get; set; }
    public string? Title { get; set; }
}