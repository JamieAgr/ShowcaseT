using Microsoft.AspNetCore.SignalR;
using WebApp_Showcase.Controllers;
using WebApp_Showcase.Models;

namespace WebApp_Showcase
{
    public class GameHub : Hub
    {
        public async Task Send()
        {
            await Clients.All.SendAsync("Receive");
        }

        /*public async Task SetSubBoard()
        {
            int row = GameData.NextSubBoard[0];
            int col = GameData.NextSubBoard[1];
            await Clients.All.SendAsync("GetSub", row, col);
        }

        public async Task SetCurrentPlayer()
        {
            await Clients.All.SendAsync("GetPlayer", GameData.CurrentPlayer.ToString());
        }

        public async Task MakeMove(int row, int col, int cellRow, int cellCol, bool winner)
        {
            string currentPlayer = GameData.CurrentPlayer.ToString();
            GameData.CurrentPlayer = GameData.CurrentPlayer == 'X' ? 'O' : 'X';
            // Broadcast the move information to all connected clients
            await Clients.All.SendAsync("ReceiveMove", row, col, cellRow, cellCol, currentPlayer, winner);
        }

        public async Task SendMessage(string user, string message)
        {
            // Broadcast the received message to all clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }*/

        

        public async Task JoinGame(string lobbyCode, string username)
        {
            // Find the lobby with the given code
            var lobby = OpenLobbies.gameLobbies.Find(l => l.Code == lobbyCode);
            if (lobby != null)
            {
                // Add the player to the lobby
                lobby.Players.Add(username, 'O');
                // Notify the client that the join was successful
                await Clients.Caller.SendAsync("JoinedGame", lobbyCode);
            }
            else
            {
                // Notify the client that the lobby does not exist
                await Clients.Caller.SendAsync("LobbyNotFound");
            }
        }

        public async Task CreateGame(string username)
        {
            // Generate a unique lobby code (you can use a more robust method for generating codes)
            string lobbyCode = GenerateLobbyCode();

            // Create a new lobby
            var newLobby = new GameLobby { Code = lobbyCode };
            newLobby.Players.Add(username, 'X');
            OpenLobbies.gameLobbies.Add(newLobby);

            // Notify the client that the lobby was created
            await Clients.Caller.SendAsync("CreatedGame", lobbyCode);
        }

        public async Task CreateLocal(string username)
        {
            // Generate a unique lobby code (you can use a more robust method for generating codes)
            string lobbyCode = GenerateLobbyCode();

            // Create a new lobby
            var newLobby = new GameLobby { Code = lobbyCode };
            newLobby.Players.Add(username, null);
            OpenLobbies.gameLobbies.Add(newLobby);

            // Notify the client that the lobby was created
            await Clients.Caller.SendAsync("CreatedGame", lobbyCode);
        }

        private string GenerateLobbyCode()
        {
            // Generate a random 6-character code (you can use a more robust method)
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task Group(string code)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, code);
            Console.WriteLine($"{Context.ConnectionId}");
            await Clients.All.SendAsync("Grouped");
        }

        public async Task SetSubBoardT(string code)
        {
            GameDataT gameData = FindLobbyByCode(code).gameDataT;
            int row = gameData.NextSubBoard[0];
            int col = gameData.NextSubBoard[1];
            await Clients.Group(code).SendAsync("GetSubT", row, col);
        }

        public async Task SetCurrentPlayerT(string code)
        {
            GameDataT gameData = FindLobbyByCode(code).gameDataT;
            await Clients.Group(code).SendAsync("GetPlayerT", gameData.CurrentPlayer.ToString());
        }

        public async Task MakeMoveT(string code, int row, int col, int cellRow, int cellCol, bool winner)
        {
            Console.WriteLine(Context.ConnectionId);
            GameDataT gameData = FindLobbyByCode(code).gameDataT;
            string currentPlayer = gameData.CurrentPlayer.ToString();
            gameData.CurrentPlayer = gameData.CurrentPlayer == 'X' ? 'O' : 'X';

            // Send the move information to all clients in the same group (game lobby)
            await Clients.Group(code).SendAsync("ReceiveMoveT", row, col, cellRow, cellCol, currentPlayer, winner);
        }

        public async Task SendMessageT(string user, string message, string code)
        {
            // Broadcast the received message to all clients in the same group (game lobby)
            await Clients.Group(code).SendAsync("ReceiveMessageT", user, message);
        }

        private GameLobby FindLobbyByCode(string code)
        {
            // Return the first matching lobby or null if not found
            return OpenLobbies.gameLobbies.FirstOrDefault(i => i.Code == code);
        }
    }
}
