function highlightSquare(cell) {
    console.log("highlight", cell)
    if (cell.classList.contains('selected')) {
        cell.classList.remove('selected');
    } else {
        document.querySelectorAll('.selected').forEach(e => e.classList.remove('selected'));
        cell.classList.add('selected');
    }
}