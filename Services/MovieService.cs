// The 'Movie' class defines the structure of a "movie" resource
// that this service will handle. It has two properties: 'Id' and 'Title'.
// In larger systems, there could be more complex classes with additional properties and behaviors.
class Movie
{
  // '_id' is a private static field that will be used to assign a unique ID
  // to each movie. Static means the field is shared across all instances of the class.
  private static int _id = 0;

  // 'Id' is a public property representing the unique identifier of a movie.
  // 'Title' is a public property representing the title of the movie.
  public int Id { get; set; }
  public string Title { get; set; }

  // The constructor initializes a new Movie object with a given title.
  // Each time a new movie is created, it gets a unique Id based on the static '_id' field.
  public Movie(string title)
  {
    Title = title;
    // Assigns the current value of _id to Id, then increments _id.
    Id = _id++;
  }
}

// 'IMovieService' is an interface, which is like a blueprint for any movie service class.
// It defines the methods that any class implementing this interface must have.
// This is useful because it allows us to swap different implementations of a movie service
// without changing the code that uses it.
interface IMovieService
{
  // Get all movies in the system as a collection (IEnumerable).
  public IEnumerable<Movie> GetAllMovies();

  // Create a new movie and return the created movie.
  public Movie CreateMovie(Movie movie);
  // Update the title of an existing movie with a given Id.
  // If the movie doesn't exist, return null (optional Movie? means it can return null).
  public Movie? UpdateMovieWithId(int id, Movie updateMovieInfo);

  // Delete the movie with the given Id.
  public void DeleteMovieWithId(int id);
}

// 'MovieService' is a class that implements the 'IMovieService' interface.
// This means it must provide concrete logic for each method defined in the interface.
class MovieService : IMovieService
{
  // A private list to store all movie objects in memory.
  private List<Movie> movies;

  // Constructor initializes the movie list.
  public MovieService()
  {
    movies = new List<Movie>();
  }

  // This method retrieves all movies in the system.
  // Since 'List<Movie>' is a type of IEnumerable, we can return it directly.
  public IEnumerable<Movie> GetAllMovies()
  {
    // Returns the list of movies.
    return movies;
  }


  // This method adds a new movie to the list.
  // It accepts a 'Movie' object, adds it to the list, and returns it.
  public Movie CreateMovie(Movie movie)
  {
    // The passed-in movie object is assumed to be correctly created,
    // so we just add it to the list of movies.
    movies.Add(movie);

    // Return the added movie.
    return movie;
  }

  // This method updates the title of a movie with the specified 'id'.
  // If the movie with that Id is not found, it returns null.
  public Movie? UpdateMovieWithId(int id, Movie updateMovieInfo)
  {
    // Use 'Find' to locate the movie with the matching Id.
    var movie = movies.Find((movie) => movie.Id == id);

    // If no movie is found with the given Id, return null (no update performed).
    if (movie == null)
    {
      return null;
    }

    // Since we're only updating the title, we directly set the title of the found movie.
    movie.Title = updateMovieInfo.Title;

    // Return the updated movie.
    return movie;
  }

  // This method deletes a movie with the specified Id from the list.
  public void DeleteMovieWithId(int id)
  {
    // Find the movie by its Id using the 'Find' method.
    var movie = movies.Find((movie) => movie.Id == id);

    // If no movie is found, we just return without performing any action.
    if (movie == null)
    {
      return;
    }

    // If the movie is found, remove it from the list.
    movies.Remove(movie);
  }
}