document.addEventListener('DOMContentLoaded', function () {
    const mainBoard = document.getElementById('main-board');
    let currentPlayer = 'X'; // Player X starts
    let activeSubBoard = null; // Keep track of the active sub-board

    function createSubCell() {
        const subCell = document.createElement('div');
        subCell.classList.add('sub-cell', 'hoverable');
        return subCell;
    }

    function createSubBoard(i, j) {
        const subBoard = document.createElement('div');
        subBoard.classList.add('sub-board', `board-${i}-${j}`);
        subBoard.dataset.row = i;
        subBoard.dataset.col = j;

        for (let i = 0; i < 3; i++) {
            for (let j = 0; j < 3; j++) {
                const subCell = createSubCell();
                subCell.addEventListener('click', function () {
                    handleSubCellClick(subCell, i, j);
                });
                subBoard.appendChild(subCell);
            }
        }

        return subBoard;
    }

    function createCell(i, j) {
        const cell = document.createElement('div');
        cell.classList.add('cell');
        const subBoard = createSubBoard(i, j);
        cell.appendChild(subBoard);
        return cell;
    }

    function createMainBoard() {
        for (let i = 0; i < 3; i++) {
            for (let j = 0; j < 3; j++) {
                const cell = createCell(i, j);
                mainBoard.appendChild(cell);
            }
        }
    }

    async function handleSubCellClick(subCell, i, j) {
        if (!subCell.textContent && ((!activeSubBoard || subCell.parentElement === activeSubBoard) || activeSubBoard == null)) {
            const subBoard = subCell.parentElement;
            const row = subBoard.dataset.row;
            const col = subBoard.dataset.col;
            const cellRow = i % 3;
            const cellCol = j % 3;

            try {
                const response = await makeMove(row, col, cellRow, cellCol);
                const { winner, draw, currentPlayer: newPlayer, isMainBoardWon } = response;

                subCell.textContent = currentPlayer;
                subCell.classList.remove('hoverable');

                if (winner) {
                    replaceSubBoardWithSymbol(subCell.parentElement, winner);
                } else if (draw) {
                    replaceSubBoardWithSymbol(subCell.parentElement, 'draw');
                }

                activeSubBoard = null;

                if (isMainBoardWon) {
                    alert(`Main board is won by ${winner}!`); // Display an alert or any other UI indication
                }

                const subBoards = document.querySelectorAll('.sub-board');
                subBoards.forEach(board => {
                    const [boardRow, boardCol] = extractBoardPosition(board);
                    if (boardRow === cellRow && boardCol === cellCol) {
                        activeSubBoard = board;
                        return;
                    }
                });

                // Reset border color for all subcells
                const allSubCells = document.querySelectorAll('.sub-cell');
                allSubCells.forEach(cell => {
                    cell.style.borderColor = '';
                });

                // Set border color to green for subcells of active subboard
                const activeSubCells = activeSubBoard.querySelectorAll('.sub-cell');
                activeSubCells.forEach(cell => {
                    cell.style.borderColor = 'green';
                });

                currentPlayer = newPlayer;
            } catch (error) {
                console.error('Error making move:', error);
            }
        }
    }

    async function makeMove(row, col, cellRow, cellCol) {
        const response = await fetch('/game/move', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ row, col, cellRow, cellCol })
        });
        if (!response.ok) {
            throw new Error('Failed to make move');
        }
        return await response.json();
    }

    function replaceSubBoardWithSymbol(subBoard, symbol) {
        const cell = subBoard.parentElement;
        if (symbol === 'draw') {
            //cell.textContent = 'Draw';
        } else {
            cell.removeChild(subBoard);
            cell.textContent = symbol;
        }
    }

    function extractBoardPosition(subBoard) {
        const className = subBoard.classList[1]; // Assuming the class follows the format "board-i-j"
        const [, boardRow, boardCol] = className.split('-');
        return [parseInt(boardRow), parseInt(boardCol)];
    }

    createMainBoard();
});
