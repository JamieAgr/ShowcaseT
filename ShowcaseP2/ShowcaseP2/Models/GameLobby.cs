using System.Collections.Generic;
using WebApp_Showcase.Controllers;

namespace WebApp_Showcase.Models
{
    public class GameLobby
    {
        public string Code { get; set; }

        public Dictionary<string, char?> Players { get; set; } = new Dictionary<string, char?>();
        //public List<string> Players { get; set; } = new List<string>();
        public int Type { get; set; } //type 1 = op 1 pc | type 2 = op 2 pcs

        public GameDataT gameDataT = new GameDataT();
    }

    public class GameDataT
    {
        public char CurrentPlayer { get; set; } = 'X';
        public char[,,] Board { get; set; } = new char[3, 3, 9]; // 3x3 grid of 3x3 sub-boards
        public int[] NextSubBoard { get; set; } = new int[] { -1, -1 }; // Indicates the next sub-board to play on
        public char[,] SubBoardWin { get; set; } = new char[3, 3]; // Indicates whether a sub-board is won
    }

    public static class OpenLobbies
    {
        public static List<GameLobby> gameLobbies = new List<GameLobby>();
    }
}