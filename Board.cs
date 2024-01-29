using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Board
    {
        //Board Orientataion
        public int up { get; protected set; } //Indicates which direction is up 1 for white on bottom, -1 for black on bottom
        
        //Board Size/Pos Parameters
        private int x = 0;
        private int y = 0;
        public int r { get; protected set; }
        public int c { get; protected set; }
        //Square size
        private int squareLength = 20;
        
        //Lists
        //Array of Squares
        public Square[,] boardSquares { get; protected set; }
        //Lists of Pieces
        List<Piece> lightPieces = new List<Piece>();
        List<Piece> darkPieces = new List<Piece>();

        //List of accesible squares by piece color
        List<Square> lightAccessibleSquares = new List<Square>();
        List<Square> darkAccessibleSquares = new List<Square>();

        //Brushes - Light/Dark Squares/Pieces
        SolidBrush lightSquaresBrush = new SolidBrush(System.Drawing.Color.Tan);
        SolidBrush darkSquaresBrush = new SolidBrush(System.Drawing.Color.Brown);
        SolidBrush lightPiecesBrush = new SolidBrush(System.Drawing.Color.White);
        SolidBrush darkPiecesBrush = new SolidBrush(System.Drawing.Color.Black);

        //Game values
        int lightScore = 0;
        int darkScore = 0;

        //Constructor
        public Board(int x, int y, int r, int c, int squareLength, Color light, Color dark)
        {
            //Board orientation
            up = 1; //default value
            //Dimensions
            //Defines board position
            this.x = x;
            this.y = y;
            //Defines Square width/length
            this.squareLength = squareLength;
            //Defines Board Rows/Columns
            this.r = r;
            this.c = c;

            //Passes in Colors
            lightSquaresBrush.Color = light;
            darkSquaresBrush.Color = dark;

            //Creates Squares array and fills it
            boardSquares = new Square[r, c];
            //temp pos variables for square position offset
            int tx = x;
            int ty = y;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    //Creates the board with checkerboard pattern (alternating color by row and column)
                    if ((i + j) % 2 == 0)
                    {
                        //Light Squares
                        boardSquares[i, j] = new Square(lightSquaresBrush, squareLength, tx, ty, i, j);
                    }
                    else
                    {
                        //Dark Squares
                        boardSquares[i, j] = new Square(darkSquaresBrush, squareLength, tx, ty, i, j);
                    }
                    //Horizontal Offset
                    tx += squareLength;
                }
                //Vertical Offset
                ty += squareLength;
                //Resets Horizontal Offset
                tx = x;
            }
        }
        //Drawing Method - Draws Board with given square size on given graphics at given pos (INCOMPLETE - DOESN'T ADJUST CHILD POSITIONS)
        public void Draw(Graphics g, int squareLength, int x, int y)
        {
            foreach (Square s in boardSquares)
            {
                s.Draw(g);
            }
            //Reassigns length/pos variabes
            this.squareLength = squareLength;
            this.x = x;
            this.y = y;
        }
        //Drawing Method (Unchanged) - Redraws board
        public void Draw(Graphics g)
        {
            foreach (Square s in boardSquares)
            {
                s.Draw(g);
            }
            foreach (Piece p in lightPieces)
            {
                p.Draw(g);
            }
            foreach (Piece p in darkPieces)
            {
                p.Draw(g);
            }
        }
        //Add Piece
        public void AddPiece(bool isLight, int r, int c, string pieceType = "")
        {
            //Creates temporary Brush/List references
            SolidBrush tempBrush = null;
            List<Piece> tempList = null;
            //Creates temp piece
            Piece newPiece;

            //Directs references to appropriate color's list/brush
            if (isLight == true)
            {
                tempBrush = lightPiecesBrush;
                tempList = lightPieces;
            }
            else if (isLight == false)
            {
                tempBrush = darkPiecesBrush;
                tempList = darkPieces;
            }
            
            //Creates correct Piece type and adds it to the appropriate Piece list - make invalid string a pawn
            if (pieceType == "pawn")
            {
                newPiece = new Pawn(tempBrush, isLight, boardSquares[r, c], this);
            }
            else if (pieceType == "rook")
            {
                newPiece = new Rook(tempBrush, isLight, boardSquares[r, c], this);
            }
            else if (pieceType == "bishop")
            {
                newPiece = new Bishop(tempBrush, isLight, boardSquares[r, c], this);
            }
            else if (pieceType == "queen")
            {
                newPiece = new Queen(tempBrush, isLight, boardSquares[r, c], this);
            }
            else if (pieceType == "knight")
            {
                newPiece = new Knight(tempBrush, isLight, boardSquares[r, c], this);
            }
            else if (pieceType == "king")
            {
                newPiece = new King(tempBrush, isLight, boardSquares[r, c], this);
            }
            else
            {
                //Creates Pawn
                newPiece = new Pawn(tempBrush, isLight, boardSquares[r, c], this);
            }
            //Adds piece to its teams list and updates the occupant of its square
            tempList.Add(newPiece);
            boardSquares[r, c].Fill(newPiece);
            
            //INCOMPLETE - Other piece types needed
        }

        //Board Capture method - removes a piece from the gameboard
        public void Capture (Piece target)
        {
            //remove the piece from the correct list and update the score
            if (target.isLight)
            {
                lightPieces.Remove(target);
                darkScore += target.value;
            }
            else
            {
                darkPieces.Remove(target);
                lightScore += target.value;
            }
        }
        //Board Uncapture method - returns a piece to the gameboard
        public void Uncapture(Piece target)
        {
            //remove the piece from the correct list and update the score
            if (target.isLight)
            {
                lightPieces.Add(target);
                darkScore -= target.value;
            }
            else
            {
                darkPieces.Add(target);
                lightScore -= target.value;
            }
        }

        //Board Click Method - Returns square clicked or null
        public Square Click(int mx, int my)
        {
            //Finds clicked square
            foreach (Square s in boardSquares)
            {
                if (s.shape.Contains(mx, my))
                {
                    return s;
                }
            }
            //Returns null if no square clicked
            return null;
        }

        //Board Covered Squares Method - Returns a list of squares covered by light or dark - INCOMPLETE
        public List<Square> CoveredSquares(bool isLight)
        {
            //Create a list for returning
            List<Square> temp = new List<Square>();
            temp.Clear();
            
            //if light
            if (isLight)
            {
                //create a list of squares covered by light piecess
                foreach (Piece p in lightPieces)
                {
                    temp.AddRange(p.returnLegalMoves());
                }
                lightAccessibleSquares = temp; //store the list of squares
            }
            else
            {
                //create a list of squares covered by dark piecess
                foreach (Piece p in darkPieces)
                {
                    temp.AddRange(p.returnLegalMoves());
                }
                darkAccessibleSquares = temp; //store the list of squares
            }
            //return the list
            return temp;
        } 
        //Check Check - checks if the king of a certain color is in check
        public bool CheckCheck(bool isLight)
        {
            CoveredSquares(!isLight); //generates the list of squares covered by the opposition
            bool ans = false; //answer variable
            Piece king = null; //create pointer to piece
            //if checking for the light pieces
            if (isLight)
            {
                //find the king in the list of pieces
                foreach (Piece p in lightPieces)
                {
                    if (p.pieceType == "king")
                    {
                        king = p; //store it once found
                    }
                }
                //if the king was found and its current square is accessible by the other color
                if (king != null && darkAccessibleSquares.Contains(king.currentSquare))
                {
                    ans = true; //it is in check
                }
            }
            //if checking for the dark pieces
            else
            {
                //find the king in the list of pieces
                foreach (Piece p in darkPieces)
                {
                    if (p.pieceType == "king")
                    {
                        king = p; //store it once found
                    }
                }
                //if the king was found and its current square is accessible by the other color
                if (king != null && lightAccessibleSquares.Contains(king.currentSquare))
                {
                    ans = true; //it is in check
                }
            }
            return ans; //return the answer
        }

        //Check Check - checks if the king of {isLight} is in checkmate - INCOMPLETE: (NEED A MORE ROBUST WAY TO MARK MOVES MADE ILLEGAL BY CHECKS) FIND THE PIECES CHECKING THE KING, if >1, only check king moves, if >0, check blocks too
        public bool CheckCheckMate(bool isLight)
        {
            /*
            CoveredSquares(!isLight); //generates the list of squares covered by the opposition
            bool inCheck = false; //is the king in check?
            bool noMoves = false; //does the king have a way to get out of check
            Piece king = null; //create pointer to piece
            //if checking for the light pieces
            if (isLight)
            {
                //find the king in the list of pieces
                foreach (Piece p in lightPieces)
                {
                    if (p.pieceType == "king")
                    {
                        king = p; //store it once found
                    }
                }
                //if the king was found and its current square is accessible by the other color
                if (king != null && darkAccessibleSquares.Contains(king.currentSquare))
                {
                    inCheck = true; //it is in check
                }
                if (inCheck)
                {
                    foreach(Piece p in darkPieces)
                    {
                        if()
                    }
                }
            }
            //if checking for the dark pieces
            else
            {
                //find the king in the list of pieces
                foreach (Piece p in darkPieces)
                {
                    if (p.pieceType == "king")
                    {
                        king = p; //store it once found
                    }
                }
                //if the king was found and its current square is accessible by the other color
                if (king != null && lightAccessibleSquares.Contains(king.currentSquare))
                {
                    inCheck = true; //it is in check
                }

            }
            return ans; //return the answer
            */
            return false;
        }
        //Check Stale mate - checks for stalemate for the {isLight} team
        public bool CheckStaleMate (bool isLight)
        {
            return false;
        }

    }
}

