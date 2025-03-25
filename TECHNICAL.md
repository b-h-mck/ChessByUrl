# Chess By URL Technical Information


## URL scheme
Chess By URL uses the following URL scheme:
```
https://chessbyurl.com/{action}/{ruleset}/{board}/{moves}
```

### Action
Currently supported action are:
- `play`: Displays a UI for playing the game.
- `svg`: Returns an SVG image file of the board.

### Ruleset
Rulesets define the rules of the game, including pieces and their movements, but not including the initial board layout.
The ruleset also defines how the `board` and `moves` parameters are interpreted. Rulesets include a version number.

Currently, only one ruleset is supported:
- `c1`: Standard chess.

### Board
The board string is a sequence of characters representing the *initial* board layout. The final board layout will be
determined by playing `moves` onto this board.

Board strings currently available for ruleset `c1` are:
- `s`: Standard chess starting position.
- `x{string}`: Custom board layout, with the rest of the string representing the layout. (TODO: Describe it)

### Moves
The moves string represents which moves to play on the initial board. These moves will also be displayed in Play mode.

Moves strings currently available for ruleset `c1` are:
- (empty): No moves. The position will be the initial board layout in `board`.
- `m{string}`: Move sequence, with the rest of the string respresenting the moves. This is calculated by enumerating 
               the legal moves from each position, picking the selected moves, packing them into a byte array, and 
               Base64-encoding the final byte array.

## Parsers
The `Parsers` namespace contains classes for parsing and serialising the URL parameters. Each parser will generally 
examine the first character of the string and decide whether it can parse it; if not, it returns `null`. Parser classes
are also responsible for serialising back to a string. The `ParserCollection` class aggregates all the parsers and
provides methods for parsing/serialising with the first parser that doesn't return `null`.

## Rulesets
The `IRuleset` interface defines the broad rules of the game. This include the players, the board size, the valid 
piece types, and how to decide whether the game is over. Most of the interesting logic is stored within the piece 
types themselves as Behaviours (see below).

## Board state
The only state stored for the board is the position of the pieces and the current player.

There are two other pieces of state that are typically stored with the board: castling rights and en passant 
vulnerability. Chess By URL avoids storing these by instead having distinct piece types for each situation (i.e. both 
`Rook` & `RookWithCastlingRights`, and `Pawn` & `PawnVulnerableToEnPassant`), and swapping between them as needed.

## Moves
Moves consist of a From square, a To square, and optionally a Variant integer. The Variant is used when the From
and To square along aren't enough, and need further input from the user. Currently that's just for Pawn promotion
(to select the piece to promote to). But when Chess960 is implemented, it will also be used to differentiate between
moving the King and castling when this is ambiguous.

## Behaviours
Behaviours are the core of the application. Each piece type has a set of behaviours that define what it can do.
There are four different kinds of behaviour:
- `IGetLegalMovesBehaviour`: This is used to get legal moves for a particular piece. If a piece has multiple behaviours
                             of this type, all behaviours' legal moves are aggregated.
- `IFilterLegalMovesBehaviour`: This behaviour gives pieces an opportunity to adjust or veto other pieces' legal moves.
                             This is used by the King to veto moves that would put it in check.
- `IApplyMoveBehaviour`: This is called for a piece after it has moved, to apply any side effects of the move. It's only
                required for special moves that do more than just move the piece and possibly capture in the destination.
- `IAdjustBoardAfterMoveBehaviour`: This is called after a move (by any piece) has been made, to allow other pieces to
                adjust the board state. This is used to remove castling rights from the Rooks after the King moves, and
                to remove en passant vulnerability from Pawns after the next move.

Here is the anatomy of a move:
- For each piece on the board that belongs to the current player:
  - Use all `IGetLegalMovesBehaviour`s to get a list of legal move candidates.
  - Pass those candidates through `IFilterLegalMovesBehaviour`s for *all* pieces on the board to get a final list of legal moves.
  - Display those legal moves to the player.
- After the player selects a move:
  - Apply the default move behaviour (move the piece from the From square to the To square, capturing any piece in the To square).
  - Call any `ApplyMoveBehaviour`s for the moving piece.
  - Call any `AdjustBoardAfterMoveBehaviour`s for *all* pieces on the board.