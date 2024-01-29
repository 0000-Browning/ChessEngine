using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ChessEngine
{
    class ChessGameController
    {
        //Graphics
        Graphics g;
        Pen highlight = new Pen(Color.Blue, 2);

        //Game Objects
        Board gameBoard;
        List<Piece> lightPieces;
        List<Piece> darkPieces;
        Piece selectedPiece;

        //Game states
        bool lightToMove = true; //Indicates player to move, true -> light, false -> black
        int result; //Indicates game result, 0 -> tie, 1 -> white wins, -1 -> black wins
        bool inProgress = true; //Indicates if game is finished
        int up = 1; //Indicates which direction is up 1 for white on bottom, -1 for black on bottom

        //Constructor
        public ChessGameController(Graphics g, int x, int y, int r, int c, int squareLength, Color light, Color dark)
        {
            //Passes in graphics surface
            this.g = g;

            //Creates gameboard
            gameBoard = new Board(x, y, r, c, squareLength, light, dark);

            //Adds pieces - Move to constructor of Board?
            //Add Pawns
            for (int i = 0; i < 8; i++)
            {
                gameBoard.AddPiece(true, 6, i, "pawn");
                gameBoard.AddPiece(false, 1, i, "pawn");
            }
            
            //Add Rooks
            gameBoard.AddPiece(false, 0, 0, "rook");
            gameBoard.AddPiece(false, 0, 7, "rook");
            gameBoard.AddPiece(true, 7, 0, "rook");
            gameBoard.AddPiece(true, 7, 7, "rook");

            //Add bishops
            gameBoard.AddPiece(false, 0, 2, "bishop");
            gameBoard.AddPiece(false, 0, 5, "bishop");
            gameBoard.AddPiece(true, 7, 2, "bishop");
            gameBoard.AddPiece(true, 7, 5, "bishop");
            
            //Add knights
            gameBoard.AddPiece(false, 0, 1, "knight");
            gameBoard.AddPiece(false, 0, 6, "knight");
            gameBoard.AddPiece(true, 7, 1, "knight");
            gameBoard.AddPiece(true, 7, 6, "knight");

            //Add queen
            gameBoard.AddPiece(false, 0, 3, "queen");
            gameBoard.AddPiece(true, 7, 3, "queen");

            //Add king
            gameBoard.AddPiece(false, 0, 4, "king");
            gameBoard.AddPiece(true, 7, 4, "king");

        }

        //Click method - on board (configured: select pieces) - INCOMPLETE - may have extra checks
        public void Click(int mx, int my)
        {
            //Temp square to store result of board.click
            Square clickedSquare;
            //Returns clicked square or null
            clickedSquare = gameBoard.Click(mx, my);
            //End function if no square clicked - MAY ADD OTHER FUCNTIONALITY LATER
            if (clickedSquare == null)
            {
                return;
            }

            //Checks result and acts based on previous selections (or lack thereof) - INCOMPLETE - extraneous checks
            if (selectedPiece == null) //If no selected piece and piece clicked -> select piece
            {
                if (clickedSquare.occupant != null && clickedSquare.occupant.isLight == lightToMove)
                {
                    //Selects piece
                    selectedPiece = clickedSquare.occupant;
                }
            }
            //If selected piece -> check for legal move or piece of same color to select
            else if (selectedPiece != null) 
            {
                //Checks if square: is empty and a legal destination for the piece
                if (clickedSquare.occupant == null)
                {
                    if (selectedPiece.checkLegalMove(clickedSquare))
                    {
                        //Store original square
                        Square temp = selectedPiece.currentSquare;
                        //Moves piece
                        selectedPiece.Move(clickedSquare);
                        //If the king is now in check, undo the move
                        if (gameBoard.CheckCheck(lightToMove))
                        {
                            selectedPiece.Unmove(temp); //return piece to its original square
                        }
                        else //update the turn otherwise
                        {
                            lightToMove ^= true; //toggles color to move
                        }
                    } 
                }
                //Checks if square: is occupied by piece of opposite color and a legal destination for the piece
                else if (clickedSquare.occupant.isLight != lightToMove && selectedPiece.checkLegalMove(clickedSquare))
                {
                    //Store original square
                    Square tempS = selectedPiece.currentSquare;
                    //Capture piece and store it
                    Piece tempP = Capture(selectedPiece, clickedSquare);
                    //If the king is now in check, undo the move
                    if (gameBoard.CheckCheck(lightToMove))
                    {
                        selectedPiece.Unmove(tempS); //return attacking piece to its original square
                        gameBoard.Uncapture(tempP); //return captured peice to the gameboard
                        clickedSquare.Fill(tempP); //return captured piece to its original square
                    }
                    else //update the turn otherwise
                    {
                        lightToMove ^= true; //toggles color to move
                    }
                }
                //Deselects piece
                selectedPiece = null;
                
                //Selects new piece if same color piece clicked
                if (clickedSquare.occupant != null && clickedSquare.occupant.isLight == lightToMove)
                {
                    //Selects new piece
                    selectedPiece = clickedSquare.occupant;
                }
            }
            else
            {
                //Do nothing
            }
            Update();
        }
        
        //Capture method - takes a piece and a square as input, moves the piece to the square and captures the current occupant
        public Piece Capture(Piece attacker, Square target)
        {
            Piece temp = target.occupant; //store the current occupant of the target square
            gameBoard.Capture(target.occupant); //remove it from the board
            target.Empty(); //update the square
            attacker.Move(target); //move the attacker into the square
            return temp; //return the captured piece
        }


        //Selected Piece Method - shows potential moves/highlights piece
        public void Select(Piece target)
        {
            g.DrawRectangle(highlight, target.currentSquare.shape);
            target.generateLegalMoves();
        }
        
        //Update - game tick
        public void Update()
        {
            //Clears Graphics
            g.Clear(Color.White);
            
            //Redraws Board
            gameBoard.Draw(g);

            //Highlights selected Piece
            if (selectedPiece != null)
            {
                Select(selectedPiece);
            }
            //Check win conditions - INCOMPLETE
            gameBoard.CheckCheckMate(lightToMove);
            gameBoard.CheckStaleMate(lightToMove);
        }
    }
}
