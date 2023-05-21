using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ProgProjekt3.Piece;
using static ProgProjekt3.Tile;
namespace ProgProjekt3
{
    public partial class Form1 : Form
    {
        // Listor för brickorna och pjäsernas PictureBox värden
        public List<PictureBox> tileBoxes = new List<PictureBox>();
        public List<PictureBox> pieceBoxes = new List<PictureBox>();
        public static bool WhiteTurn; // Boolean för vems tur det: true är vit, false är svart
        public Form1()
        {
            InitializeComponent();
            // Kallar ResetGame funktionen
            ResetGame();
            // Sätter AccessibleDescription med vilken färg brickan har så man kan byta färgen fram och tillbaka
            foreach (PictureBox tile in tileBoxes)
            {
                if (tile.BackColor == Color.Peru) { tile.AccessibleDescription = "BrownTile"; }
                else if (tile.BackColor == Color.Cornsilk) { tile.AccessibleDescription = "WhiteTile"; }
            }
            // Om man klickar på något som är en PictureBox kallas PictureBox_Click funktionen
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox)
                {
                    x.Click += PictureBox_Click;
                }
            }
        }
        private void ResetGame()
        {
            // Denna funktionen fyller standardvärden för pjäser och brickor
            tileBoxes.Clear();
            pieceBoxes.Clear();
            pieces.Clear();
            tiles.Clear();
            WhiteTurn = true;
            tileBoxes = IdentifyBoxes("Tile");
            pieceBoxes = IdentifyBoxes("Piece");
            foreach (PictureBox piece in pieceBoxes)
            {
                foreach (PictureBox tile in tileBoxes)
                {
                    int tileIndex = tileBoxes.IndexOf(tile);
                    int tileRow = CheckRowNumber(tileBoxes.IndexOf(tile));
                    tiles.Add(new Tile
                    {
                        PosX = tile.Location.X,
                        PosY = tile.Location.Y,
                        Index = tileIndex,
                        Occupied = false
                    });
                    if (piece.Location.X > tile.Location.X + 5 && piece.Location.X < tile.Location.X + 20 &&
                    piece.Location.Y > tile.Location.Y + 5 && piece.Location.Y < tile.Location.Y + 20)
                    {
                        pieces.Add(new Piece
                        {
                            Name = piece.Name,
                            Type = piece.AccessibleName,
                            PosX = piece.Location.X,
                            PosY = piece.Location.Y,
                            TileIndex = tileIndex,
                            Selected = false,
                            Moves = 0
                        });
                    }
                }
            }
            timer1.Start();
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            // Konverterar 'sender' från Control till PictureBox
            PictureBox selectedBox = sender as PictureBox;
            // Brickornas originella färg appliceras
            foreach (PictureBox tile in tileBoxes)
            {
                if (tile.AccessibleDescription == "BrownTile") { tile.BackColor = Color.Peru; }
                else if (tile.AccessibleDescription == "WhiteTile") { tile.BackColor = Color.Cornsilk; }
            }
            foreach (Piece piece in pieces)
            {
                if (selectedBox.Name == piece.Name) // Om selectedBox är en av pjäserna
                {
                    // Skriver ut alla möjliga drag i MessageBox
                    ViewLegalMoves(piece);

                    // Väljer en vit pjäs om det är vits tur
                    if (piece.Type.Contains("White") && WhiteTurn == true)
                    {
                        foreach (Piece otherPiece in pieces)
                        {
                            otherPiece.Selected = false;
                        }
                        tileBoxes[piece.TileIndex].BackColor = Color.LightGreen;
                        piece.Selected = true;
                    }
                    // Väljer en svart pjäs om det är svarts tur
                    else if (piece.Type.Contains("Black") && WhiteTurn == false)
                    {
                        foreach (Piece otherPiece in pieces)
                        {
                            otherPiece.Selected = false;
                        }
                        tileBoxes[piece.TileIndex].BackColor = Color.LightGreen;
                        piece.Selected = true;
                    }
                }
            }
            foreach (PictureBox tileBox in tileBoxes)
            {
                Tile tile = tiles[tileBoxes.IndexOf(tileBox)];
                if (selectedBox.Name == tileBox.Name) // Om selectedBox är en av brickorna
                {
                    foreach (Piece piece in pieces)
                    {
                        if (piece.Selected && tile.IsLegalMove(piece)) // Om pjäs är vald och om brickan är ett möjligt drag
                        {
                            int pieceRow = CheckRowNumber(piece.TileIndex) + 1;
                            int pieceColumn = 8 - (piece.TileIndex - pieceRow) / 8;
                            int tileRow = CheckRowNumber(tile.Index) + 1;
                            int tileColumn = 8 - (tile.Index - tileRow) / 8;
                            piece.CapturePiece(tile); // Kollar om draget tar någon pjäs
                            // Skriver ut draget i MessageBox
                            MessageBox.Text = $"{piece.Type} tile {ConvertColumn(pieceColumn)}{pieceRow} to {ConvertColumn(tileColumn)}{tileRow}";
                            piece.TileIndex = tile.Index; // Flyttar pjäsen till nya brickan
                            piece.Moves += 1; // Pjäsens Moves ökar med 1
                            piece.Selected = false;
                            WhiteTurn = !WhiteTurn; // Byter värdet på WhiteTurn (true/false)
                        }
                    }
                }
            }
        }
        private List<PictureBox> IdentifyBoxes(string accessName)
        {
            // Denna funktion returnar en lista på alla PictureBoxes med accessName i sitt AccessibleName eller AccessibleDescription
            List<PictureBox> pictureBoxes = new List<PictureBox>();
            foreach (Control x in this.Controls)
            {
                PictureBox pic = x as PictureBox;
                if (pic != null && pic.AccessibleName == accessName)
                {
                    pictureBoxes.Add(pic);
                }
                else if (pic != null && pic.AccessibleDescription == accessName)
                {
                    pictureBoxes.Add(pic);
                }
            }
            pictureBoxes.OrderBy(x => x.Location.X).ThenBy(y => y.Location.Y); // Sorterar listan för deras X och Y värden
            return pictureBoxes;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            foreach (Piece piece in pieces)
            {
                piece.UpdatePieceInfo(pieceBoxes); // Uppdaterar pjäsernas information, ex dess position

                // Om vita kungen dör vinner svart och vice versa
                if (piece.Type == "WhiteKing" && piece.TileIndex < 0)
                {
                    MessageBox.Text = "Black wins";
                    timer1.Stop();
                }
                if (piece.Type == "BlackKing" && piece.TileIndex < 0)
                {
                    MessageBox.Text = "White wins";
                    timer1.Stop();
                }
            }
            foreach (Tile tile in tiles)
            {
                tile.Occupied = tile.TileIsOccupied(); // Uppdaterar brickornas boolean för om det står en pjäs på den
            }
        }
        private void ViewLegalMoves(Piece piece)
        {
            List<int> legalMoves = piece.CheckLegalMoves(WhiteTurn);
            string strMoves = "";
            foreach (int move in legalMoves)
            {
                int row = CheckRowNumber(move) + 1; // Funktion som räknar ut vilken rad pjäsen är på
                int column = 8 - (move - row) / 8;
                string columnLetter = ConvertColumn(column);
                strMoves += $"{columnLetter}{row} ";
            }
            MessageBox.Text = strMoves;
        }
        private string ConvertColumn(int column)
        {
            string columnLetter = "";
            switch (column) // Konverterar kolumnens nummer till en bokstav
            {
                case 1:
                    columnLetter = "A";
                    break;
                case 2:
                    columnLetter = "B";
                    break;
                case 3:
                    columnLetter = "C";
                    break;
                case 4:
                    columnLetter = "D";
                    break;
                case 5:
                    columnLetter = "E";
                    break;
                case 6:
                    columnLetter = "F";
                    break;
                case 7:
                    columnLetter = "G";
                    break;
                case 8:
                    columnLetter = "H";
                    break;

            }
            return columnLetter;
        }
    }
}