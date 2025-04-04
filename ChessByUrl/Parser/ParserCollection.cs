﻿using ChessByUrl.Rules;

namespace ChessByUrl.Parser
{
    public class ParserCollection
    {


        private readonly List<IRulesetParser> _rulesetParsers = new List<IRulesetParser>();
        private readonly List<IBoardParser> _boardParsers = new List<IBoardParser>();
        private readonly List<IMovesParser> _movesParsers = new List<IMovesParser>();

        public void AddRulesetParser(IRulesetParser parser) => _rulesetParsers.Add(parser);
        public void AddBoardParser(IBoardParser parser) => _boardParsers.Add(parser);
        public void AddMovesParser(IMovesParser parser) => _movesParsers.Add(parser);



        public IRuleset? ParseRuleset(string rulesetString)
        {
            foreach (var parser in _rulesetParsers)
            {
                var ruleset = parser.Parse(rulesetString);
                if (ruleset != null)
                    return ruleset;
            }
            return null;
        }

        public Board? ParseBoard(IRuleset ruleset, string boardString)
        {
            foreach (var parser in _boardParsers)
            {
                var board = parser.Parse(ruleset, boardString);
                if (board != null)
                    return board;
            }
            return null;
        }

        public IEnumerable<Move> ParseMoves(IRuleset ruleset, Board board, string moveString)
        {
            foreach (var parser in _movesParsers)
            {
                var moves = parser.Parse(ruleset, board, moveString);
                if (moves != null)
                    return moves;
            }
            return Enumerable.Empty<Move>();
        }

        public string? SerialiseRuleset(IRuleset ruleset)
        {
            foreach (var parser in _rulesetParsers)
            {
                var rulesetString = parser.Serialise(ruleset);
                if (rulesetString != null)
                    return rulesetString;
            }
            return null;
        }

        public string? SerialiseBoard(IRuleset ruleset, Board board)
        {
            foreach (var parser in _boardParsers)
            {
                var boardString = parser.Serialise(ruleset, board);
                if (boardString != null)
                    return boardString;
            }
            return null;
        }

        public string? SerialiseMoves(IRuleset ruleset, Board initialBoard, IEnumerable<Move> moves)
        {
            foreach (var parser in _movesParsers)
            {
                var movesString = parser.Serialise(ruleset, initialBoard, moves);
                if (movesString != null)
                    return movesString;
            }
            return null;
        }


        // TODO: Get rid of this abomination and do proper dependency injection
        // Ultimately we want to include all available parsers
        private static ParserCollection? _instance;
        public static ParserCollection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ParserCollection();
                    _instance.AddRulesetParser(new Orthodox.RulesetParser());
                    _instance.AddBoardParser(new Orthodox.StartBoardParser());
                    _instance.AddBoardParser(new Orthodox.RleBoardParser());
                    _instance.AddMovesParser(new Orthodox.EnumeratedMovesParser());
                }
                return _instance;
            }
        }

    }
}
