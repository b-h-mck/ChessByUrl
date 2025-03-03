function onSquareClick(square) {
    var coords = square.getAttribute('data-coords');
    var legalMoves = JSON.parse(square.getAttribute('data-legalmoves'));

    var board = square.closest('.board');

    var isSelected = square.classList.contains('selected');
    var anySelected = board.querySelectorAll('.square.selected').length > 0;
    var isMoveFrom = square.classList.contains('moveFrom');
    var isMoveTo = square.classList.contains('moveTo');

    if (isMoveTo) {
        handleMoveTo(square);
    } else if (!anySelected && isMoveFrom) {
        handleSelect(board, square, coords, legalMoves);
    } else {
        handleUnselect(board);
    }
}

function handleSelect(board, square, coords, legalMoves) {
    console.log("Select", coords, legalMoves);
    board.classList.remove('showMoveFrom');
    board.classList.add('showMoveTo');

    square.classList.add('selected');

    legalMoves.forEach(move => {
        let targetSquare = board.querySelector(`.square[data-coords='${move.To}']`);
        if (targetSquare) {
            targetSquare.classList.add('moveTo');
            targetSquare.setAttribute('data-url', move.Url);
        }
    });
}

function handleUnselect(board) {
    console.log("Unselect", board);
    board.classList.remove('showMoveTo');
    board.classList.add('showMoveFrom');

    board.querySelectorAll('.square').forEach(e => {
        e.classList.remove('selected', 'moveTo');
        e.removeAttribute('data-url');
    });
}

function handleMoveTo(square) {
    console.log("MoveTo", square);
    var url = square.getAttribute('data-url');
    window.location.href = url;
}