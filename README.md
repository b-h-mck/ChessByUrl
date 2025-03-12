# ChessByURL
ChessByURL is a web application that allows users to play chess by sharing URLs (via email, message boards, IM apps, etc). 
The URL contains the full state of the game, so players don't need to worry about user accounts, link expiry, etc.

ChessByURL was developed with the intention of eventually supporting chess variants, including Chess960 and several fairy
chess variants, with the rulesets also embedded in the URLs. Currently, only standard chess is supported.

## Getting started
If you just want to play some chess, [visit the website here](https://chessbyurl.com/).

If you want to run/develop the application yourself, you'll just need a development environment for .NET 9.0. This application 
is very light on dependencies, so after cloning the repository you should be able to just open the .sln file in 
Visual Studio 2022 Community (or similar) and run it.

## Roadmap
- [x] **Standard Chess ruleset**
- [x] **UI for playing the game**
- [x] **SVG board image generation**
- [ ] **UI improvements:** There are several improvements that could be made to the UI, such as highlighting the last move
                and allowing drag-and-drop moves. It's probably not worth going too far with this since the client-side
                application (below) will replace it.
- [ ] **[Chess960](https://en.wikipedia.org/wiki/Chess960) starting layouts:** Support generating Chess960 positions either 
                randomly or by number. This is almost ready, but castling rules need a bit more work (especially since some 
                layouts require user input to differentiate between moving and castling the King).
- [ ] **Board editor UI:** Allow users to edit the board layout and generate a URL for it. Once the rules are generalised
                enough to support Chess960, this will be a lot easier to implement.
- [ ] **API:** Allow client/third-party applications to generate/interpret ChessByUrl URLs for their own purposes.
- [ ] **Caching:** This application is extremely cacheable. The board state is interpreted entirely from the URL, and
                intermediate board state calculations are useful for future calculations. A Redis cache or similar could
                make this application much faster.
- [ ] **Client-side application:** Currently the application displays a server-generated web page with very simple 
                Javascript to select and navigate between moves. A client-side application would be a lot more responsive
                and allow for more fancy features. If it's written with Blazor WebAssembly, it could share the existing
                C# calculation logic, allowing for offline play and greatly easing the load on the server (we'd still want       
                to retain that logic on the server for generating SVGs).
- [ ] **[Chess On A Really Big Board](https://en.wikipedia.org/wiki/Chess_on_a_really_big_board) ruleset:** This is where
                we introduce fairy pieces and (hopefully) start seeing the Behaviour system shine.
- [ ] **Other rulesets:** Lots of different variants to research and implement. The application would probably need
                significant refactoring to support non-rectangular boards (e.g. hexagonal chess & 3D chess).
- [ ] **Custom rulesets:** This is the end goal. Allow users to define their own rulesets and generate URLs for them. Allow
                users to mix-and-match pieces, and even define their own pieces, all embedded in the URL.

## Technical information
This section is only relevant if you want to contribute to the project, or you're curious about how it works.

### URL scheme
ChessByURL uses the following URL scheme:
```
https://chessbyurl.com/{action}/{ruleset}/{board}/{moves}
```

#### Action
Currently supported action are:
- `play`: Displays a UI for playing the game.
- `svg`: Returns an SVG image file of the board.

#### Ruleset
Rulesets define the rules of the game, including pieces and their movements, but not including the initial board layout.
The ruleset also defines how the `board` and `moves` parameters are interpreted. Rulesets include a version number.

Currently, only one ruleset is supported:
- `c1`: Standard chess.

#### Board
The board string is a sequence of characters representing the *initial* board layout. The final board layout will be
determined by playing `moves` onto this board.

Board strings currently available for ruleset `c1` are:
- `s`: Standard chess starting position.
- `x{string}`: Custom board layout, with the rest of the string representing the layout. (TODO: Describe it)

#### Moves
The moves string represents which moves to play on the initial board. These moves will also be displayed in Play mode.

Moves strings currently available for ruleset `c1` are:
- (empty): No moves. The position will be the initial board layout in `board`.
- `m{string}`: Move sequence, with the rest of the string respresenting the moves. This is calculated by enumerating 
               the legal moves from each position, picking the selected moves, packing them into a byte array, and 
               Base64-encoding the final byte array.

### Parsers
The `Parsers` namespace contains classes for parsing and serialising the URL parameters. Each parser will generally 
examine the first character of the string and decide whether it can parse it; if not, it returns `null`. Parser classes
are also responsible for serialising back to a string. The `ParserCollection` class aggregates all the parsers and
provides methods for parsing/serialising with the first parser that doesn't return `null`.

### Rulesets
The `IRuleset` interface defines the broad rules of the game. This include the players, the board size, the valid 
piece types, and how to decide whether the game is over. Most of the interesting logic is stored within the piece 
types themselves as Behaviours (see below).

### Board state
The only state stored for the board is the position of the pieces and the current player.

There are two other pieces of state that are typically stored with the board: castling rights and en passant 
vulnerability. ChessByURL avoids storing these by instead having distinct piece types for each situation (i.e. both 
`Rook` & `RookWithCastlingRights`, and `Pawn` & `PawnVulnerableToEnPassant`), and swapping between them as needed.

### Moves
Moves consist of a From square, a To square, and optionally a Variant integer. The Variant is used when the From
and To square along aren't enough, and need further input from the user. Currently that's just for Pawn promotion
(to select the piece to promote to). But when Chess960 is implemented, it will also be used to differentiate between
moving the King and castling when this is ambiguous.

### Behaviours
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

## Contributing
Contributions are welcome! If you find a bug or have an idea for a feature, please open an issue. If you'd like to
make a minor change (fix up some styles, add emoji to this readme, etc.), feel free to make a pull request. For larger
changes, please open an issue first so we can discuss the best way to approach it.

## Credits
Chess piece SVGs are from the [Wikimedia Commons](https://commons.wikimedia.org/wiki/Category:SVG_chess_pieces) and are in the public domain.

## Licence
This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.