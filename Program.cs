internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Register the Movie list with the Dependency Injection Container
        builder.Services.AddSingleton<IMovieService, MovieService>();
        var app = builder.Build();

        // READ: Get all movies
        app.MapGet("/movies", (IMovieService movieService) =>
        {
            return movieService.GetAllMovies();
        });
        // CREATE: Adds a new movie
        app.MapPost("/movies", (Movie? movie, IMovieService movieService) =>
        {
            // User input validation
            if (movie == null)
            {
                return Results.BadRequest();
            }

            // Calling the business logic
            var createdMovie = movieService.CreateMovie(movie);

            return Results.Created($"/movies/{createdMovie.Id}", createdMovie);
        });

        // DELETE: Delete a movie with id
        app.MapDelete("/movies/{Id}", (int Id, IMovieService movieService) =>
        {
            movieService.DeleteMovieWithId(Id);
            return Results.Ok();
        });

        // UPDATE: Update a movie with id
        app.MapPut("/movies/{Id}", (int Id, Movie updatedMovie, IMovieService movieService) =>
        {
            if (updatedMovie == null)
            {
                return Results.BadRequest();
            }

            var movie = movieService.UpdateMovieWithId(Id, updatedMovie);

            if (movie == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(movie);
        });

        // System status
        app.MapGet("/health", () => "System healthy");

        app.Run();
    }
}