internal class Program
{
    // The Main method is the entry point of the application.
    // This is where the program starts executing.
    private static void Main(string[] args)
    {
        // 'CreateBuilder' initializes a new WebApplication builder.
        // The 'args' parameter is an array of command-line arguments passed to the application.
        var builder = WebApplication.CreateBuilder(args);

        // Register the MovieService with Dependency Injection (DI).
        // AddSingleton registers a single instance of the service to be used throughout the application.
        // This means that the same instance of MovieService will be used every time IMovieService is requested.
        builder.Services.AddSingleton<IMovieService, MovieService>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
             policy => 
             {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
             });
        });

        // 'Build' creates the WebApplication object from the configuration set up by the builder.
        var app = builder.Build();
        app.UseCors("AllowAll");

        // Mapping routes to HTTP endpoints:
        // This defines what happens when a user makes a request to specific URLs.
        // Here, we handle the I/O operations (HTTP requests and responses).
        // The actual business logic is kept separate in the MovieService class.

        // READ: Get all movies.
        // 'app.MapGet' maps a GET HTTP request to the "/movies" URL.
        // It uses the IMovieService that was registered to retrieve all movies.
        app.MapGet("/movies", (IMovieService movieService) =>
        {
            // This is the I/O handling layer where we respond to HTTP requests.
            // We delegate the data retrieval to the business logic layer (MovieService).
            // This separation keeps our HTTP handling code clean and focused on I/O.
            return movieService.GetAllMovies();
        });

        // CREATE: Add a new movie.
        // 'app.MapPost' maps a POST HTTP request to the "/movies" URL.
        // A POST request is typically used to create new resources.
        app.MapPost("/movies", (Movie? movie, IMovieService movieService) =>
        {
            // Input validation: Ensure that the data sent by the user is correct (not null).
            if (movie == null)
            {
                // If the movie is null, return a "400 Bad Request" response.
                return Results.BadRequest();
            }

            // Delegate the creation logic to the business logic layer.
            // This separation allows us to modify the business logic without changing the I/O code.
            var createdMovie = movieService.CreateMovie(movie);

            // Return a "201 Created" response with the new movie's location (URL) and the movie data.
            return Results.Created($"/movies/{createdMovie.Id}", createdMovie);
        });

        // DELETE: Delete a movie by Id.
        // 'app.MapDelete' maps a DELETE HTTP request to the "/movies/{Id}" URL.
        app.MapDelete("/movies/{Id}", (int Id, IMovieService movieService) =>
        {
            // Delegate the deletion logic to the business logic layer.
            movieService.DeleteMovieWithId(Id);

            // Return a "200 OK" response indicating the movie was successfully deleted.
            return Results.Ok();
        });

        // UPDATE: Update a movie's information by Id.
        // 'app.MapPut' maps a PUT HTTP request to the "/movies/{Id}" URL.
        app.MapPut("/movies/{Id}", (int Id, Movie updatedMovie, IMovieService movieService) =>
        {
            // Input validation: Ensure that the data sent by the user is correct (not null).
            if (updatedMovie == null)
            {
                // If the movie data is null, return a "400 Bad Request" response.
                return Results.BadRequest();
            }

            // Delegate the update logic to the business logic layer.
            var movie = movieService.UpdateMovieWithId(Id, updatedMovie);

            // If no movie with the specified Id is found, return a "404 Not Found" response.
            if (movie == null)
            {
                return Results.NotFound();
            }

            // Return a "200 OK" response with the updated movie data.
            return Results.Ok(movie);
        });

        // Health Check: Check if the system is running.
        // 'app.MapGet' maps a GET HTTP request to the "/health" URL.
        app.MapGet("/health", () =>
        {
            // This endpoint is part of the I/O layer, providing system status.
            return "System healthy";
        });

        // 'app.Run' starts the web server and begins listening for incoming HTTP requests.
        app.Run();
    }
}