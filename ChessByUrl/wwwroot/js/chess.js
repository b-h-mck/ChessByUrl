function onSquareClick(square) {
    var coords = square.getAttribute('data-coords');
    var board = square.closest('.board');


    var isSelected = square.classList.contains('selected');
    var anySelected = board.querySelectorAll('.square.selected').length > 0;
    var isMoveFrom = square.classList.contains('moveFrom');
    var isMoveTo = square.classList.contains('moveTo');

    if (isMoveTo) {
        var moveTo = JSON.parse(square.getAttribute('data-moveTo'));
        handleMoveTo(square, moveTo);
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
        e.classList.remove('selected', 'moveTo');
        e.removeAttribute('data-moveTo');
        document.getElementById('promotions').innerHTML = ''
    });
}

function handleMoveTo(square, moveTo) {
    console.log("handleMoveTo", square, moveTo);
    if (moveTo.Url) {
        window.location.href = moveTo.Url;
    }
    else if (moveTo.Variants.length > 0) {
        // show variants
        var promotionsElement = document.getElementById('promotions');
        promotionsElement.innerHTML = '';
        moveTo.Variants.forEach(variant => {
            var a = document.createElement('a');
            a.href = variant.Url;
            a.innerText = variant.VariantInfo.VariantName;
            promotionsElement.appendChild(a);
            promotionsElement.appendChild(document.createElement('br'));
        });
    }
}