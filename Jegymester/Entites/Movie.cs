namespace Jegymester.Entites
{
    public enum Genre
    {
        Thriller,
        ScienceFiction,
        Western,
        Fantasy
    }
    public class Movie
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Length { get; set; }
        public Genre Genre { get; set; }
        
     
    }
}
