using EFCore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Api.Data;

public class MoviesContext : DbContext
{
    /*
     * Creates the Movies property as a get-only property, much better than using a null forgiving operator when using
     * .Net nullable. This works by convention and will attempt to map to a DB table called Movies.
     */
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /*
         * For the development image of SQLServer, you need to add TrustServerCertificate=True to the connection string
         * to allow EF to connect as you will get exceptions otherwise. Might need a different approach for Production.
         */
        optionsBuilder.UseSqlServer("""
                                    Data Source=localhost;
                                    Initial Catalog=MoviesDB;
                                    User Id=sa;
                                    Password=P@ssw0rd123;
                                    TrustServerCertificate=True;
                                    """);
        // Add this to log EF translated queries to the console for debugging (this is not proper logging). This does
        // not log any info or results, only the executed queries so its GDPR safe
        optionsBuilder.LogTo(Console.WriteLine);
        
        base.OnConfiguring(optionsBuilder);
    }
}