namespace WebApplication1.Models
{
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string FavoriteAnimals { get; set; } // Comma-separated list of animals
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
