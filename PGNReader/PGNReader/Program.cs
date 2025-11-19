using System;
using Chess;

using System.Windows.Forms;
namespace PGNReader
{

    public class Reader
    {
        static void Main()  
        {

        }
        public static (string fen, int maxTurns) Read(string pgn, int currentTurn)
        {
            var board = new ChessBoard() { AutoEndgameRules = AutoEndgameRules.All };
            board = ChessBoard.LoadFromPgn(pgn);
            string fen;
            int maxTurns = board.ExecutedMoves.Count;
            board.First();
            board.Previous();
            fen = board.ToFen();
            while (currentTurn != board.MoveIndex + 1)
            {
                board.Next();
                fen = board.ToFen();
            }
            return (fen, maxTurns);
        }
    }
}
