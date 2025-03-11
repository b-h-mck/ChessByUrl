function onSquareClick(square) {
    var coords = square.getAttribute('data-coords');
    var board = square.closest('.board');


    var isSelected = square.classList.contains('selected');
    var anySelected = board.querySelectorAll('.square.selected').length > 0;
    var isMoveFrom = square.classList.contains('moveFrom');
    var isMoveTo = square.classList.contains('moveTo');

    if (isMoveTo) {
        var moveTo = JSON.parse(square.getAttribute('data-moveTo'));
        handleMoveTo(board, square, moveTo);
    } else if (!anySelected && isMoveFrom) {
        var movesFrom = JSON.parse(square.getAttribute('data-moveFrom'));
        handleMoveFrom(board, square, coords, movesFrom);
    } else {
        handleUnselect(board);
    }
}

function handleMoveFrom(board, square, coords, movesFrom) {
    console.log("handleMoveFrom", coords, movesFrom);
    board.classList.remove('showMoveFrom');
    board.classList.add('showMoveTo');

    square.classList.add('selected');

    movesFrom.forEach(move => {
        let targetSquare = board.querySelector(`.square[data-coords='${move.To}']`);
        if (targetSquare) {
            var movesTo = movesFrom[targetSquare]
            targetSquare.classList.add('moveTo');
            targetSquare.setAttribute('data-moveTo', JSON.stringify(move));
        }
    });
}

function handleUnselect(board) {
    console.log("handleUnselect", board);
    board.classList.remove('showMoveTo');
    board.classList.add('showMoveFrom');

    board.querySelectorAll('.square').forEach(e => {
        e.classList.remove('selected', 'secondarySelected', 'moveTo');
        e.removeAttribute('data-moveTo');
    });

    var promotionsElement = document.getElementById('promotions');
    Array.from(promotionsElement.children[1].children).forEach(e => e.onclick = null);
    promotionsElement.classList.add('hidden');
}

function handleMoveTo(board, square, moveTo) {
    console.log("handleMoveTo", square, moveTo);
    // If we've got a URL (i.e. no variants), redirect to it.
    if (moveTo.Url) {
        window.location.href = moveTo.Url;
        return;
    }

    // Show the variants in the promotions panel.
    board.classList.remove('showMoveTo');
    square.classList.add('secondarySelected');
    var promotionsElement = document.getElementById('promotions');
    promotionsElement.classList.remove('hidden');
    for (i = 0; i < moveTo.Variants.length; i++) {
        let variant = moveTo.Variants[i];
        let promotionElement = promotionsElement.children[1].children[i];
        promotionElement.onclick = function() {
            window.location.href = variant.Url;
        };
    }
}