using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShowcaseP2.Models;
using System;
using System.Linq;
using WebApp_Showcase.Models;

namespace WebApp_Showcase.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GameController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost("moveT/{lobbyCode}")]
        public async Task<IActionResult> MakeMoveT(string lobbyCode, [FromBody] MoveRequest request)
        {
            // Find the lobby with the given code
            GameLobby lobby = FindLobbyByCode(lobbyCode);
            if (lobby == null)
                return NotFound("Lobby not found");

            // Get the game data for this lobby
            GameDataT gameData = lobby.gameDataT;

            int row = request.Row;
            int col = request.Col;
            int cellRow = request.CellRow;
            int cellCol = request.CellCol;
            int innercell = (cellRow * 3) + cellCol;

            if (!IsValidMoveT(gameData, row, col, cellRow, cellCol, innercell))
            {
                return BadRequest("Invalid move");
            }
            if (lobby.Players[request.Username] != gameData.CurrentPlayer && lobby.Players[request.Username] != null)
            {
                return BadRequest("wrong player");
            }

            gameData.Board[row, col, innercell] = gameData.CurrentPlayer; // Update the board

            char? winner = CheckWinT(gameData, row, col);

            if (IsBoardFullT(gameData, row, col, innercell) || gameData.SubBoardWin[cellRow, cellCol] != '\0')
            {
                // If the current sub-board is full, allow the player to choose any sub-board
                gameData.NextSubBoard = new int[] { -1, -1 };
            }
            else
            {
                // Set the next sub-board based on the cell just played
                gameData.NextSubBoard = new int[] { cellRow, cellCol };
            }

            if (winner != null)
            {
                char? mainWinner = CheckMainBoardWinT(gameData);
                if (mainWinner != null)
                {
                    if (lobby.Players[request.Username] != null)
                    {
                        var user = await _userManager.FindByNameAsync(request.Username);
                        user.MatchesWon += 1;
                        user.MatchesPlayer += 1;
                        await _userManager.UpdateAsync(user);

                        var loser = await _userManager.FindByNameAsync(lobby.Players.Where(i => i.Key != request.Username).FirstOrDefault().Key);
                        loser.MatchesLost += 1;
                        loser.MatchesPlayer += 1;
                        await _userManager.UpdateAsync(loser);
                    }

                    return Ok(new MoveResponse { Winner = mainWinner, IsMainBoardWon = true });
                }
                return Ok(new MoveResponse { Winner = winner, CurrentPlayer = gameData.CurrentPlayer });
            }

            return Ok(new MoveResponse { CurrentPlayer = gameData.CurrentPlayer, NextSubBoard = gameData.NextSubBoard });
        }

        private bool IsValidMoveT(GameDataT gameData, int row, int col, int cellRow, int cellCol, int innercell)
        {
            // Check if the move is within the bounds of the board and the cell is empty
            return row >= 0 && row < 3 && col >= 0 && col < 3 &&
                   cellRow >= 0 && cellRow < 3 && cellCol >= 0 && cellCol < 3 &&
                   gameData.Board[row, col, innercell] == '\0' &&
                   ((gameData.NextSubBoard[0] == row && gameData.NextSubBoard[1] == col) || gameData.NextSubBoard[0] == -1);
        }

        private char? CheckWinT(GameDataT gameData, int row, int col)
        {
            char[,,] board = gameData.Board;
            for (int i = 0; i < 9; i += 3) // horizontal
            {
                if (board[row, col, i] == board[row, col, i + 1] && board[row, col, i + 1] == board[row, col, i + 2] && board[row, col, i] != '\0')
                {
                    gameData.SubBoardWin[row, col] = board[row, col, i];
                    return board[row, col, i];
                }
            }

            for (int i = 0; i < 3; i += 1) // vertical
            {
                if (board[row, col, i] == board[row, col, i + 3] && board[row, col, i + 3] == board[row, col, i + 6] && board[row, col, i] != '\0')
                {
                    gameData.SubBoardWin[row, col] = board[row, col, i];
                    return board[row, col, i];
                }
            }

            if (board[row, col, 0] == board[row, col, 4] && board[row, col, 4] == board[row, col, 8] && board[row, col, 0] != '\0')
            {
                gameData.SubBoardWin[row, col] = board[row, col, 0];
                return board[row, col, 0];
            }
            if (board[row, col, 2] == board[row, col, 4] && board[row, col, 4] == board[row, col, 6] && board[row, col, 2] != '\0')
            {
                gameData.SubBoardWin[row, col] = board[row, col, 1];
                return board[row, col, 2];
            }

            return null;
        }

        private char? CheckMainBoardWinT(GameDataT gameData)
        {
            char[,] subBoardWin = gameData.SubBoardWin;
            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (subBoardWin[i, 0] != '\0' && subBoardWin[i, 0] == subBoardWin[i, 1] && subBoardWin[i, 1] == subBoardWin[i, 2])
                {
                    return subBoardWin[i, 0];
                }
            }

            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (subBoardWin[0, i] != '\0' && subBoardWin[0, i] == subBoardWin[1, i] && subBoardWin[1, i] == subBoardWin[2, i])
                {
                    return subBoardWin[0, i];
                }
            }

            // Check diagonals
            if (subBoardWin[0, 0] != '\0' && subBoardWin[0, 0] == subBoardWin[1, 1] && subBoardWin[1, 1] == subBoardWin[2, 2])
            {
                return subBoardWin[0, 0];
            }
            if (subBoardWin[0, 2] != '\0' && subBoardWin[0, 2] == subBoardWin[1, 1] && subBoardWin[1, 1] == subBoardWin[2, 0])
            {
                return subBoardWin[0, 2];
            }

            return null;
        }

        private bool IsBoardFullT(GameDataT gameData, int row, int col, int innercell)
        {
            char[,,] board = gameData.Board;
            for (int i = 0; i < 9; i++)
            {
                if (board[row, col, i] == '\0')
                {
                    return false;
                }
            }
            return true;
        }

        private GameLobby FindLobbyByCode(string code)
        {
            // Return the first matching lobby or null if not found
            return OpenLobbies.gameLobbies.FirstOrDefault(i => i.Code == code);
        }

    }


    public class MoveRequest
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public int CellRow { get; set; }
        public int CellCol { get; set; }
        public string Username {  get; set; }
    }

    public class MoveResponse
    {
        public char? Winner { get; set; }
        public char? CurrentPlayer { get; set; }
        public int[] NextSubBoard { get; set; }
        public bool IsMainBoardWon { get; set; }
    }

}
