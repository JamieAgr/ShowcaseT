using Microsoft.AspNetCore.Identity;

namespace ShowcaseP2.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int MatchesWon { get; set; }
        public int MatchesLost { get; set; }
        public int MatchesPlayer { get; set; }

    }
}
