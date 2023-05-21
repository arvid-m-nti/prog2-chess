using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProgProjekt3.Form1;
using static ProgProjekt3.Piece;
namespace ProgProjekt3
{
    class Tile
    {
        // PosX & PosY är brickans specifika position, Index är brickans index i listan tiles
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Index { get; set; }
        public bool Occupied { get; set; } // Boolean för om det är en pjäs som står på brickan
        public static List<Tile> tiles = new List<Tile>(); // Lista på alla brickor
        public bool TileIsOccupied()
        {
            // Om en pjäs är på en bricka är brickan ockuperad
            foreach (Piece piece in pieces)
            {
                if (piece.PosX > PosX + 5 && piece.PosX < PosX + 20 &&
                    piece.PosY > PosY + 5 && piece.PosY < PosY + 20)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsLegalMove(Piece piece)
        {
            // Kollar alla lagliga drag som gäller för pjäsen
            List<int> legalMoves = piece.CheckLegalMoves(WhiteTurn);
            if (legalMoves == null) { return false; } // Om det inte finns några lagliga drag returnas false
            // Kollar om det draget man vill göra är med i legalMoves
            foreach (int move in legalMoves)
            {
                if (Index == move)
                {
                    return true;
                }
            }
            return false;
        }
    }
}