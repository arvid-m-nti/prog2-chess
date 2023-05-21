using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ProgProjekt3.Tile;
using static ProgProjekt3.Form1;
using System.Windows.Forms;
using System.Drawing;
namespace ProgProjekt3
{
    class Piece
    {
        public string Name { get; set; } // (Name) för representerande PictureBox
        public string Type { get; set; } // Vit/svart & vilken sorts pjäs, ex. WhiteKing
        // PosX & PosY är pjäsens specifika position och TileIndex är pjäsens position på en bricka
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int TileIndex { get; set; }
        public bool Selected { get; set; } // Om man tryckt på pjäsen så är denna true
        public int Moves { get; set; } // Räknar antal drag en pjäs har gjort, används för 2 steg regeln för bönder
        public static List<Piece> pieces = new List<Piece>(); // Lista på alla pjäser i spelet
        public void UpdatePieceInfo(List<PictureBox> pieceBoxes)
        {
            // Ändrar PictureBoxens positionsvärden så att den är samma som Piecens värde
            foreach (PictureBox pieceBox in pieceBoxes)
            {
                if (Name == pieceBox.Name)
                {
                    if (TileIndex >= 0)
                    {
                        PosX = tiles[TileIndex].PosX + 10;
                        PosY = tiles[TileIndex].PosY + 10;
                    }
                    pieceBox.Location = new Point(PosX, PosY);
                }
            }
        }
        public void CapturePiece(Tile tile)
        {
            // Kallas när en pjäs har tagits
            foreach (Piece otherPiece in pieces)
            {
                if (tile.Index == otherPiece.TileIndex)
                {
                    // Flyttar pjäsen som tagits utanför spelytan
                    otherPiece.PosX = -100;
                    otherPiece.PosY = -100;
                    otherPiece.TileIndex = -1;
                }
            }
        }
        public static int CheckRowNumber(int index)
        {
            // Räknar ut raden för en pjäs/bricka 
            int rowNumber = 0;
            int i = index;
            if (index >= 8)
            {
                while (i >= 8)
                {
                    i -= 8;
                    if (i < 8)
                    {
                        rowNumber = i;
                    }
                }
                return rowNumber;
            }
            else
            {
                return index;
            }
        }
        public List<int> CheckLegalMoves(bool WhiteTurn)
        {
            List<int> legalMoves = new List<int>(); // Lista för pjäsens möjliga drag
            int row = CheckRowNumber(TileIndex); // Kallar en funktion som räknar ut pjäsens RAD
            int column = (TileIndex - row) / 8; // Pjäsens KOLUMN
            /* KNIGHT & KING fungerar ganska dåligt, kanske PAWN också */
            if (WhiteTurn) // Vit tur
            {
                if (Type.Contains("Pawn")) // Vita bönder
                {
                    int up1 = 8 * (column) + (row + 1);
                    int up2 = 8 * (column) + (row + 2);
                    int upLeft1 = 8 * (column + 1) + (row + 1);
                    int upRight1 = 8 * (column - 1) + (row + 1);
                    if (up1 >= 0 && up1 < 64 && !tiles[up1].Occupied) // 1 steg upp till tom bricka
                    {
                        legalMoves.Add(up1);
                    }
                    if (up2 >= 0 && up2 < 64 && Moves == 0) // 2 steg upp till tom bricka (gäller bara första steget)
                    {
                        legalMoves.Add(up2);
                    }
                    foreach (Piece otherPiece in pieces)
                    {
                        if (otherPiece.Type.Contains("Black"))
                        {
                            if (upLeft1 >= 0 && upLeft1 < 64 && otherPiece.TileIndex == upLeft1) // Ta svart pjäs diagonalt upp-vänster
                            {
                                legalMoves.Add(upLeft1);
                            }
                            if (upRight1 >= 0 && upRight1 < 64 && otherPiece.TileIndex == upRight1) // Ta svart pjäs diagonalt upp-höger
                            {
                                legalMoves.Add(upRight1);
                            }
                        }
                    }

                }
                else if (Type.Contains("Rook")) // Vita torn
                {
                    for (int i = row + 1; i < 8; i++) // Uppåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = row - 1; i >= 0; i--) // Nedåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta ner till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column + 1; i < 8; i++) // Vänster
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta vänster till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column - 1; i >= 0; i--) // Höger
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta höger till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (Type.Contains("Knight")) // Vita springare (hästar)
                {
                    int upLeft = 8 * (column + 1) + (row + 2); // 2 upp, 1 vänster
                    int upRight = 8 * (column - 1) + (row + 2); // 2 upp, 1 höger
                    int rightUp = 8 * (column - 2) + (row + 1); // 2 höger, 1 upp
                    int rightDown = 8 * (column - 2) + (row - 1); // 2 höger, 1 ner
                    int downRight = 8 * (column - 1) + (row - 2); // 2 ner, 1 höger
                    int downLeft = 8 * (column + 1) + (row - 2); // 2 ner, 1 vänster
                    int leftDown = 8 * (column + 2) + (row - 1); // 2 vänster, 1 ner
                    int leftUp = 8 * (column + 2) + (row + 1); // 2 vänster, 1 upp
                    List<int> knightMoves = new List<int> { upLeft, upRight, rightUp, rightDown, downRight, downLeft, leftDown, leftUp };
                    foreach (int move in knightMoves)
                    {
                        if (move >= 0 && move < 64 && !tiles[move].Occupied) // Flytta till tom ruta
                        {
                            legalMoves.Add(move);
                        }
                        foreach (Piece otherPiece in pieces)
                        {
                            if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Flytta till ruta med svart pjäs
                            {
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
                else if (Type.Contains("Bishop")) // Vita löpare
                {
                    for (int i = 1; i < 8; i++) // Upp-vänster
                    {
                        if (row + i >= 8) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-höger
                    {
                        if (row + i >= 8) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-höger
                    {
                        if (row - i < 0) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-vänster
                    {
                        if (row - i < 0) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (Type.Contains("King")) // Vita kungen
                {
                    int up = 8 * (column) + (row + 1);
                    int upRight = 8 * (column - 1) + (row + 1);
                    int right = 8 * (column - 1) + (row);
                    int downRight = 8 * (column - 1) + (row - 1);
                    int down = 8 * (column) + (row - 1);
                    int downLeft = 8 * (column + 1) + (row - 1);
                    int left = 8 * (column + 1) + (row);
                    int upLeft = 8 * (column + 1) + (row + 1);
                    List<int> kingMoves = new List<int> { up, upRight, right, downRight, down, downLeft, left, upLeft };
                    foreach (int move in kingMoves)
                    {
                        if (move >= 0 && move < 64 && !tiles[move].Occupied) // Flytta till tom ruta
                        {
                            legalMoves.Add(move);
                        }
                        foreach (Piece otherPiece in pieces)
                        {
                            if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Flytta till ruta med svart pjäs
                            {
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
                else if (Type.Contains("Queen")) // Vita drottningen
                {
                    for (int i = row + 1; i < 8; i++) // Uppåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = row - 1; i >= 0; i--) // Nedåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta ner till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column + 1; i < 8; i++) // Vänster
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta vänster till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column - 1; i >= 0; i--) // Höger
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta höger till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-vänster
                    {
                        if (row + i >= 8) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-höger
                    {
                        if (row + i >= 8) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs upp-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-höger
                    {
                        if (row - i < 0) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-vänster
                    {
                        if (row - i < 0) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("Black") && otherPiece.TileIndex == move) // Ta svart pjäs ner-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            else // Annars svart tur
            {
                if (Type.Contains("Pawn")) // Svarta bönder
                {
                    int down1 = 8 * (column) + (row - 1);
                    int down2 = 8 * (column) + (row - 2);
                    int downRight1 = 8 * (column - 1) + (row - 1);
                    int downLeft1 = 8 * (column + 1) + (row - 1);
                    if (down1 >= 0 && down1 < 64 && !tiles[down1].Occupied) // Ett steg fram
                    {
                        legalMoves.Add(down1);
                    }
                    if (down2 >= 0 && down2 < 64 && !tiles[down2].Occupied && Moves == 0) // Två steg fram (gäller bara första steget)
                    {
                        legalMoves.Add(down2);
                    }
                    foreach (Piece otherPiece in pieces)
                    {
                        if (otherPiece.Type.Contains("White"))
                        {
                            if (downRight1 >= 0 && downRight1 < 64 && otherPiece.TileIndex == downRight1) // Ta svart pjäs diagonalt upp-vänster
                            {
                                legalMoves.Add(downRight1);
                            }
                            if (downLeft1 >= 0 && downLeft1 < 64 && otherPiece.TileIndex == downLeft1) // Ta svart pjäs diagonalt upp-höger
                            {
                                legalMoves.Add(downLeft1);
                            }
                        }
                    }
                }
                else if (Type.Contains("Rook")) // Svarta torn
                {
                    for (int i = row + 1; i < 8; i++) // Uppåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = row - 1; i >= 0; i--) // Nedåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta ner till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column + 1; i < 8; i++) // Vänster
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta vänster till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column - 1; i >= 0; i--) // Höger
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta höger till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (Type.Contains("Knight")) // Svarta springare (hästar)
                {
                    int upLeft = 8 * (column + 1) + (row + 2); // 2 upp, 1 vänster
                    int upRight = 8 * (column - 1) + (row + 2); // 2 upp, 1 höger
                    int rightUp = 8 * (column - 2) + (row + 1); // 2 höger, 1 upp
                    int rightDown = 8 * (column - 2) + (row - 1); // 2 höger, 1 ner
                    int downRight = 8 * (column - 1) + (row - 2); // 2 ner, 1 höger
                    int downLeft = 8 * (column + 1) + (row - 2); // 2 ner, 1 vänster
                    int leftDown = 8 * (column + 2) + (row - 1); // 2 vänster, 1 ner
                    int leftUp = 8 * (column + 2) + (row + 1); // 2 vänster, 1 upp
                    List<int> knightMoves = new List<int> { upLeft, upRight, rightUp, rightDown, downRight, downLeft, leftDown, leftUp };
                    foreach (int move in knightMoves)
                    {
                        if (move >= 0 && move < 64 && !tiles[move].Occupied) // Flytta till tom ruta
                        {
                            legalMoves.Add(move);
                        }
                        foreach (Piece otherPiece in pieces)
                        {
                            if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Flytta till ruta med vit pjäs
                            {
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
                else if (Type.Contains("Bishop")) // Svarta löpare
                {
                    for (int i = 1; i < 8; i++) // Upp-vänster
                    {
                        if (row + i >= 8) { break; }
                        if (column + i >= 8) { break; }

                        int move = 8 * (column + i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-höger
                    {
                        if (row + i >= 8) { break; }
                        if (column - i < 0) { break; }

                        int move = 8 * (column - i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-höger
                    {
                        if (row - i < 0) { break; }
                        if (column - i < 0) { break; }

                        int move = 8 * (column - i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-vänster
                    {
                        if (row - i < 0) { break; }
                        if (column + i >= 8) { break; }

                        int move = 8 * (column + i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
                else if (Type.Contains("King")) // Svarta kungen
                {
                    int up = 8 * (column) + (row + 1);
                    int upRight = 8 * (column - 1) + (row + 1);
                    int right = 8 * (column - 1) + (row);
                    int downRight = 8 * (column - 1) + (row - 1);
                    int down = 8 * (column) + (row - 1);
                    int downLeft = 8 * (column + 1) + (row - 1);
                    int left = 8 * (column + 1) + (row);
                    int upLeft = 8 * (column + 1) + (row + 1);
                    List<int> kingMoves = new List<int> { up, upRight, right, downRight, down, downLeft, left, upLeft };
                    foreach (int move in kingMoves)
                    {
                        if (move >= 0 && move < 64 && !tiles[move].Occupied) // Flytta till tom ruta
                        {
                            legalMoves.Add(move);
                        }
                        foreach (Piece otherPiece in pieces)
                        {
                            if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Flytta till ruta med vit pjäs
                            {
                                legalMoves.Add(move);
                            }
                        }
                    }
                }
                else if (Type.Contains("Queen")) // Svarta drottningen
                {
                    for (int i = row + 1; i < 8; i++) // Uppåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = row - 1; i >= 0; i--) // Nedåt
                    {
                        int move = 8 * (column) + (i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta ner till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column + 1; i < 8; i++) // Vänster
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta vänster till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = column - 1; i >= 0; i--) // Höger
                    {
                        int move = 8 * (i) + (row);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta höger till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-vänster
                    {
                        if (row + i >= 8) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Upp-höger
                    {
                        if (row + i >= 8) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row + i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs upp-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-höger
                    {
                        if (row - i < 0) { break; }
                        if (column - i < 0) { break; }
                        int move = 8 * (column - i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner-höger
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    for (int i = 1; i < 8; i++) // Ner-vänster
                    {
                        if (row - i < 0) { break; }
                        if (column + i >= 8) { break; }
                        int move = 8 * (column + i) + (row - i);
                        if (move >= 0 && move < 64)
                        {
                            if (!tiles[move].Occupied) // Flytta upp till tom bricka
                            {
                                legalMoves.Add(move);
                            }
                            else if (tiles[move].Occupied)
                            {
                                foreach (Piece otherPiece in pieces)
                                {
                                    if (otherPiece.Type.Contains("White") && otherPiece.TileIndex == move) // Ta vit pjäs ner-vänster
                                    {
                                        legalMoves.Add(move);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return legalMoves; // Returnar alla drag som går för pjäsen
        }
    }
}