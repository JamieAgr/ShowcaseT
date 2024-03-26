namespace WebApp_Showcase.Models
{
    public class SuperTicTacToeGame
    {
        public int Id { get; set; }

        private char[,] board;
        private char currentPlayer;
        private char[,] subBoards;
        private int activeSubBoardRow;
        private int activeSubBoardCol;

        public SuperTicTacToeGame()
        {
            board = new char[9, 9];
            subBoards = new char[3, 3];
            currentPlayer = 'X';
            activeSubBoardRow = -1;
            activeSubBoardCol = -1;
        }

        public string[][] GetBoard()
        {
            string[][] boardStrings = new string[9][];
            for (int i = 0; i < 9; i++)
            {
                boardStrings[i] = new string[9];
                for (int j = 0; j < 9; j++)
                {
                    boardStrings[i][j] = board[i, j].ToString();
                }
            }
            return boardStrings;
        }


        public char GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public bool MakeMove(int row, int col)
        {
            // Check if the move is valid
            if (!IsValidMove(row, col))
                return false;

            // Update the board
            board[row, col] = currentPlayer;

            // Check for a win in the current sub-board
            char winner = CheckWin(row / 3, col / 3);
            if (winner != '\0')
                subBoards[row / 3, col / 3] = winner;

            // Update active sub-board
            activeSubBoardRow = row % 3;
            activeSubBoardCol = col % 3;

            // Update current player
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
            return true;
        }

        private bool IsValidMove(int row, int col)
        {
            // Check if the move is within bounds and in the active sub-board
            return row >= 0 && row < 9 && col >= 0 && col < 9 && (activeSubBoardRow == -1 || (row / 3 == activeSubBoardRow && col / 3 == activeSubBoardCol)) && board[row, col] == '\0';
        }

        private char CheckWin(int row, int col)
        {
            // Check for a win in the specified sub-board
            char[,] subBoard = new char[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    subBoard[i, j] = board[row * 3 + i, col * 3 + j];
                }
            }
            // Check rows, columns, and diagonals for a win
            for (int i = 0; i < 3; i++)
            {
                if (subBoard[i, 0] != '\0' && subBoard[i, 0] == subBoard[i, 1] && subBoard[i, 1] == subBoard[i, 2])
                    return subBoard[i, 0]; // Row win
                if (subBoard[0, i] != '\0' && subBoard[0, i] == subBoard[1, i] && subBoard[1, i] == subBoard[2, i])
                    return subBoard[0, i]; // Column win
            }
            if (subBoard[0, 0] != '\0' && subBoard[0, 0] == subBoard[1, 1] && subBoard[1, 1] == subBoard[2, 2])
                return subBoard[0, 0]; // Diagonal win
            if (subBoard[0, 2] != '\0' && subBoard[0, 2] == subBoard[1, 1] && subBoard[1, 1] == subBoard[2, 0])
                return subBoard[0, 2]; // Diagonal win
            return '\0'; // No win
        }

        public char[,] GetSubBoards()
        {
            return subBoards;
        }
    }
}