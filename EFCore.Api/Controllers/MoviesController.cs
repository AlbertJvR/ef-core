using EFCore.Api.Data;
using EFCore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MoviesContext _context;

    public MoviesController(MoviesContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<Movie>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        /*
         * The call to _context.Movies will do a round trip to the db to retrieve the values. This is an awaitable task
         * that can be performed async so a better way to do it is with the ToListAsync call provided by EF and awaiting
         * the result.
         */
        return Ok(await _context.Movies.ToListAsync());
    }

    // [HttpGet("by-year/{year:int}")]
    // [ProducesResponseType(typeof(List<Movie>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetAllByYear([FromRoute] int year)
    // {
    //     /*
    //      * These 2 lines:
    //      * var allMovies = _context.Movies;
    //      * var filteredMovies = allMovies.Where(m => m.ReleaseDate.Year == year);
    //      *
    //      * Are similar to these:
    //      * IQueryable<Movie> allMovies = _context.Movies;
    //      * IQueryable<Movie> filteredMovies = allMovies.Where(m => m.ReleaseDate.Year == year);
    //      *
    //      * Nothing is executed until a materialisation call such as ToListAsync() is called or you use a FOREACH.
    //      */
    //     
    //     // There is also a query syntax, which is an alternative to the fluent syntax:
    //     // var filteredMovies =
    //     //     from movie in _context.Movies
    //     //     where movie.ReleaseDate.Year == year
    //     //         select movie;
    //
    //     // This is a neater / shorter way of doing the above.
    //     var filteredMovies = _context.Movies
    //         .Where(m => m.ReleaseDate.Year == year);
    //
    //     // Nothing before this line is executed against the DB. Only when ToListAsync is called is the db hit.
    //     return Ok(await filteredMovies.ToListAsync());
    // }
    
    /*
     * Projections are done in this method. Sometimes you dont need all the fields in a table to be returned and would
     * be a massively unnecessary performance hit to return everything. In such cases use projections to only return the
     * fields that you need.
     */
    [HttpGet("by-year/{year:int}")]
    [ProducesResponseType(typeof(List<MovieTitle>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllByYear([FromRoute] int year)
    {
        var filteredMovies = await _context.Movies
            .Where(m => m.ReleaseDate.Year == year)
            .Select(movie => new MovieTitle { Id = movie.Id, Title = movie.Title })
            .ToListAsync();
        
        return Ok(filteredMovies);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] int id)
    {
        // Queries the database, returns first match, null if not found
        // var movie = await _context.Movies.FirstOrDefaultAsync(x => x.Id == id);

        // Similar to FirstOrDefault, but throws if more than one match is found.
        // var movie = await _context.Movies.SingleOrDefaultAsync(x => x.Id == id);

        // Servers from memory if already fetched, otherwise queries the DB. Run the risk of serving stale data.
        var movie = await _context.Movies.FindAsync(id);

        return movie == null
            ? NotFound()
            : Ok(movie);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] Movie movie)
    {
        await _context.Movies.AddAsync(movie);

        /*
         * EF implements a unit of work pattern, therefore all changes are QUEUED pending a call to the SaveChanges
         * function. When called EF creates a transaction, executes all the actions in the queue, and then commits
         * the transaction.
         *
         * At this point, the Movie has no ID. Once called, EF will set the ID of this object.
         */
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Movie), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] Movie movie)
    {
        /*
         * "existingMovie" is a proxy object for the movie entry which keeps a connection to the dbContext. When values
         * for the properties are changed, the DbContext is notified of these changes by means of the change tracker.
         * For in-depth info, see this article from Microsoft: https://learn.microsoft.com/en-us/ef/core/change-tracking/
         */
        var existingMovie = await _context.Movies.FindAsync(id);

        if (existingMovie is null)
        {
            return NotFound();
        }

        existingMovie.Title = movie.Title;
        existingMovie.ReleaseDate = movie.ReleaseDate;
        existingMovie.Synopsis = movie.Synopsis;

        await _context.SaveChangesAsync();

        return Ok(existingMovie);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove([FromRoute] int id)
    {
        var existingMovie = await _context.Movies.FindAsync(id);

        if (existingMovie is null)
        {
            return NotFound();
        }

        /*
         * If we did not need to return a NotFound, we could save a round trip to the DB by doing the following and
         * removing the code above:
         *
         * _context.Movies.Remove(new Movie { Id = id });
         *
         * This is enough info for EF to figure out which movie we want to remove and delete it once SaveChanges is called.
         * A similar approach can be used for Updates as well.
         */

        // You can call this on the DbSet or the context, as the context has the type info and therefore performs the same
        // _context.Remove(existingMovie);
        _context.Movies.Remove(existingMovie);
        await _context.SaveChangesAsync();

        return Ok();
    }
}