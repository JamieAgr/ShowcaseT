namespace WebApp_Showcase.Models
{
    public class Player
    {
        public Player(int id, string email, string name, int gamesPlayed, int gamesWon, int gamesLost, int role) 
        {
            Id = id;
            Email = email;
            Name = name;
            GamesPlayed = gamesPlayed;
            GamesWon = gamesWon;
            GamesLost = gamesLost;
            Role = role;
        }

        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int Role {  get; set; }


    }
}
