using EFCore.Api.Data.ValueConverters;
using EFCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Api.Data.EntityMapping;

public class MovieMapping : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        // builder
        //     .ToTable("Pictures")
        //     .HasKey(movie => movie.Id);
        
        /*
         * Global Query Filters - provide a way to add a filter to all queries to a table, e.g. if you have a logical
         *      delete and wish to filter out those records by default.
         * These filters can be overriden in specific queries by specifying the ".IgnoreQueryFilters()" operator as
         * part of your LINQ query, but by default the filter is applied to all queries.
         */
        builder
            .ToTable("Pictures")
            .HasQueryFilter(movie => movie.ReleaseDate >= new DateTime(1997, 1, 1))
            .HasKey(movie => movie.Id);

        builder.Property(movie => movie.Title)
            .HasColumnType("varchar")
            .HasMaxLength(128)
            .IsRequired();

        // builder.Property(movie => movie.ReleaseDate)
        //     .HasColumnType("date");

        /*
         * Simplest form of a ValueConverter. They are used when the type on the application side does not match or
         * cannot be automatically converted to a corresponding db type. The converter in this case will convert the
         * DateTime type to a char when writing TO the db, and when reading, will convert the char to the DateTime type.
         * In summary, ValueConverters tell EF how to Serialize / Deserialize properties
         * There are plenty of predefined converters defined by Microsoft:
         * https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations
         */
        // builder.Property(movie => movie.ReleaseDate)
        //     .HasColumnType("char(23)")
        //     .HasConversion<string>();

        // Here is an example using a custom value converter
        builder.Property(movie => movie.ReleaseDate)
            .HasColumnType("char(8)")
            .HasConversion(new DateTimeToChar8Converter());

        builder.Property(movie => movie.Synopsis)
            .HasColumnType("varchar(max)")
            .HasColumnName("Plot");

        /*
         * By using ComplexProperty, we do not create a new table, but instead the properties of the Person class are
         * added as columns to the Movies Table (or Pictures as we have renamed it above). The properties can be set
         * as shown below
         */
        // builder.ComplexProperty(movie => movie.Director, directorBuilder =>
        // {
        //     directorBuilder.Property(director => director.FirstName)
        //         .HasColumnName("DirectorFirstName");
        //     
        //     directorBuilder.Property(director => director.LastName)
        //         .HasColumnName("DirectorLastName");
        // });

        /*
         * Another way to achieve the same result as ComplexProperty is to use OwnsOne.
         * The key difference is that ComplexProperty is a grouping for things like ValueTypes or street addresses,
         * whereas OwnsOne is treated as a SEPARATE ENTITY.
         * 1. Behind the scenes it treats it like a table with a PK that is also the FK (in this case the MovieId) and
         *    fields are added there.
         * 2. No need for Include statements.
         * 3. Entry is deleted when the Movie is deleted.
         * 4. Cannot seed ComplexTypes (at the moment) but you can seed Owned Types
         *
         * You can see the behaviour if you move it to another table in the db as per below:
         */
        builder.OwnsOne(movie => movie.Director)
            .ToTable("Picture_Directors");

        builder.OwnsMany(movie => movie.Actors)
            .ToTable("Picture_Actors");

        // Relationship definitions
        builder
            .HasOne(movie => movie.Genre)
            .WithMany(genre => genre.Movies)
            .HasPrincipalKey(genre => genre.Id)
            .HasForeignKey(movie => movie.MainGenreId);

        // Seed - data that needs to be created always
        builder.HasData(new Movie
        {
            Id = 1,
            Title = "Harry Potter and the Half Blood Prince",
            ReleaseDate = new DateTime(2009, 7, 15),
            Synopsis = "Harry waves a stick and Snape goes pew pew",
            MainGenreId = 1,
            AgeRating = AgeRating.Adolescent
        });

        /*
         * This is how you seed an Owned type. You need to add the Identifier that EF creates in the "behind the scenes"
         * table (or the actual table if you provided the ToTable property)
         */
        builder.OwnsOne(movie => movie.Director)
            .HasData(new { MovieId = 1, FirstName = "David", LastName = "Yates" });

        builder.OwnsMany(movie => movie.Actors)
            .HasData(new { MovieId = 1, Id = 1, FirstName = "Daniel", LastName = "Radcliffe" },
                new { MovieId = 1, Id = 2, FirstName = "Emma", LastName = "Watson" },
                new { MovieId = 1, Id = 3, FirstName = "Rupert", LastName = "Grint" });
    }
}