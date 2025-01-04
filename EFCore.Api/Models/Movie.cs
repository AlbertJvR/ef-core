/*
 * System.ComponentModel.DataAnnotations is a general model validation set of annotations in ASP.NET, not specific to
 * Entity Framework.
 *
 * System.ComponentModel.DataAnnotations.Schema is used for anything that affects the storage schema we are using.
 */

// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Api.Models;

/*
 * The following approach uses the annotations approach which has 2 problems:
 * 1. We are adding persistence logic in the form of attributes to a domain class that we can use in the rest of
 *    codebase, so either we map it to something else or it doesnt belong here
 * 2. When switching between different database engines, this might be a problem as some of the data types used are
 *    specific to a particular database engine.
 */

// [Table("Pictures")]
// public class Movie
// {
//     [Key]
//     public int Id { get; set; }
//     
//     [MaxLength(128)]
//     [Column(TypeName = "varchar")]
//     [Required]
//     public string? Title { get; set; }
//     
//     [Column(TypeName = "date")]
//     public DateTime ReleaseDate { get; set; }
//     
//     [Column("Plot", TypeName = "varchar(max)")]
//     public string? Synopsis { get; set; }
// }

public class Movie
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string? Synopsis { get; set; }
    public AgeRating AgeRating { get; set; }
    
    /*
     * By convention, EF is smart enough to figure out that we want a foreign key with the below, even without the Id
     * field, and with the dirty hack in Program.cs to recreate the db, it will automatically generate the FK for us.
     *
     * It is also smart enough that if you specify the ID like below, to figure out that they below together until you
     * deviate from the convention of [Entity]Id
     */
    public int MainGenreId { get; set; }
    public Genre Genre { get; set; }

    public Person Director { get; set; }
    public ICollection<Person> Actors { get; set; }
}