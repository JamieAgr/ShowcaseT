document.addEventListener('DOMContentLoaded', function () {
    const gameBoard = document.getElementById('main-board');

    function createCell(row, col) {
        const cell = document.createElement('div');
        cell.classList.add('cell');
        cell.dataset.row = row;
        cell.dataset.col = col;
        return cell;
    }

    function createSubCell(row, col) {
        const subCell = document.createElement('div');
        subCell.classList.add('sub-cell', 'hoverable');
        subCell.dataset.row = row;
        subCell.dataset.col = col;
        return subCell;
    }

    async function updateBoard() {
        const response = await fetch('/game/board');
        const board = await response.json();
        gameBoard.innerHTML = '';
        for (let i = 0; i < 9; i++) {
            for (let j = 0; j < 9; j++) {
                const cell = createCell(i, j);
                const subCell = createSubCell(i % 3, j % 3);
                subCell.textContent = board[i][j];
                subCell.addEventListener('click', () => makeMove(i, j));
                cell.appendChild(subCell);
                gameBoard.appendChild(cell);
            }
        }
    }

    async function makeMove(row, col) {
        const response = await fetch('/game/move', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ row, col })
        });
        if (response.ok) {
            updateBoard();
        } else {
            alert('Invalid move');
        }
    }

    updateBoard();
});
