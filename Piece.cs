using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ChessEngine
{
    abstract class Piece
    {
        //Position/Movement Squares
        //Board piece is on
        public Board gameBoard { get; protected set; }
        //Piece location
        public Square currentSquare {get; protected set;}
        //Legal squares for piece to move to
        public List<Square> legalSquares = new List<Square>();
        //Accessible Squares - squares that the piece could move to on an empty board
        public List<Square> accessibleSquares = new List<Square>();


        //First Move
        protected bool firstMove = true; //is this the piece's first move?

        //Piece Value/type
        public int value {get; protected set;}
        public string pieceType {get; protected set;}

        //Piece Color (label + brush)
        protected SolidBrush b;
        public bool isLight { get; protected set; } //Team bool "is on light team"

        //Piece Constructor
        public Piece(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard)
        {
            //Passes in parameters
            this.currentSquare = startingSquare;
            this.b = b;
            this.isLight = isLight;
            this.gameBoard = gameBoard;
            //Generates a new set of legal moves
            generateLegalMoves();
        }
        //Draws the piece on its current square
        public void Draw(Graphics g, Square currentSquare)
        {
            this.currentSquare = currentSquare;
            g.FillEllipse(b, currentSquare.shape);
        }
        //Move Method
        public void Move(Square target)
        {
            //Stores old square
            Square temp = currentSquare;
            //Updates currect square
            currentSquare = target;
            //Vacates old square
            temp.Empty();
            //Enters new square
            currentSquare.Fill(this);
            //Marks piece as having moved
            firstMove = false;
        }
        //Unmove Method - used for reversing moves
        public void Unmove(Square target)
        {
            //Stores old square
            Square temp = currentSquare;
            //Updates currect square
            currentSquare = target;
            //Vacates old square
            temp.Empty();
            //Enters new square
            currentSquare.Fill(this);
            //Marks piece as having moved
            firstMove = true;
        }

        //Return list of legal moves - UNTESTED
        public List<Square> returnLegalMoves()
        {
            //generate a list of legal moves
            generateLegalMoves();
            return legalSquares; //return it
        }

        //Check if Move is legal
        abstract public bool checkLegalMove(Square target);

        //Generate Legal moves
        abstract public void generateLegalMoves();

        //Redraws the Piece
        abstract public void Draw(Graphics g);
    }
    class Pawn : Piece
    {
        
        //Constructor - ADD VALUE/TYPE
        public Pawn(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard) 
        {
            value = 1;
            pieceType = "pawn";
        }
        //Draws the piece
        override public void Draw(Graphics g)
        {
            g.FillEllipse(b, currentSquare.shape); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Gernerates the list of legal moves
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();
            
            //Check for piece color
            int lightToggle = 1;
            if (isLight)
            {
                lightToggle = -1; //set direction toggle
            }
            //Stores the candidate for one square movement
            Square candidateOne = gameBoard.boardSquares[currentSquare.r + lightToggle * gameBoard.up, currentSquare.c ];
            
               
            //Check one square movement
            if (!candidateOne.occupied)
            {
                legalSquares.Add(candidateOne);
                //Check two square movement only if one square movement isn't blocked and it's the first move
                if (firstMove)
                {
                    //Stores the candidate for two square movement
                    Square candidateTwo = gameBoard.boardSquares[currentSquare.r + 2 * lightToggle * gameBoard.up, currentSquare.c];
                    if (!candidateTwo.occupied)
                    {
                        legalSquares.Add(candidateTwo);
                    }
                }
            }
            
            //Check for captures
            //set R, C to hold coords of current square
            int R = currentSquare.r;
            int C = currentSquare.c;

            //create a list of the possible spaces the knight can move relative to the current square
            List<KeyValuePair<int, int>> coords = new List<KeyValuePair<int, int>>();
            coords.Add(new KeyValuePair<int, int>(1, 1*lightToggle));
            coords.Add(new KeyValuePair<int, int>(-1, 1*lightToggle));


            //Iterate over the list
            foreach (KeyValuePair<int, int> p in coords)
            {
                //Translate the values so they are relative to the current square
                C = currentSquare.c + p.Key;
                R = currentSquare.r + p.Value;

                //Check the values within the board to see if they are accessible
                if (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
                {
                    Square candidate = gameBoard.boardSquares[R, C];
                    if (candidate.occupied)
                    {
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                }
            }

        }
    }
    class Rook : Piece
    {
        //Constructor - ADD VALUE/TYPE
        public Rook(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard) 
        {
            value = 5;
            pieceType = "rook";
        }

        //Draws the piece
        override public void Draw(Graphics g)
        {
            g.FillRectangle(b, currentSquare.shape); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Generates the list of legal moves - FIX (REMOVE BREAK STATEMENTS IN LOOPS)
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();

            int i = currentSquare.r; //start at the current row

            //For each square in the same column going up, check if move is legal
            while (i < gameBoard.c - 1)
            {
                i++;
                //Stores the candidate 
                Square candidate = gameBoard.boardSquares[i, currentSquare.c];
                //If there is a piece in the candidate square
                if (candidate.occupied)
                {
                    //Break loop
                    i = gameBoard.c;
                    //If the piece is the opposite color, allow captures
                    if (candidate.occupant.isLight != this.isLight)
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                else
                {
                    //Add square to list of legal moves
                    legalSquares.Add(candidate);
                }
            }
            
            //For each square in the same column going down, check if move is legal
            i = currentSquare.r; //start at the current row
            while (i > 0)
            {
                i--;
                //Stores the candidate 
                Square candidate = gameBoard.boardSquares[i, currentSquare.c];
                //If there is a piece in the candidate square
                if (candidate.occupied)
                {
                    //Break loop
                    i = -1;
                    //If the piece is the opposite color, allow captures
                    if (candidate.occupant.isLight != this.isLight)
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                else
                {
                    //Add square to list of legal moves
                    legalSquares.Add(candidate);
                }
            }
            
            //For each square in the same row going right, check if move is legal
            i = currentSquare.c; //start at the current column
            while (i < gameBoard.r - 1)
            {
                i++;
                //Stores the candidate 
                Square candidate = gameBoard.boardSquares[currentSquare.r, i];
                //If there is a piece in the candidate square
                if (candidate.occupied)
                {
                    //Break loop
                    i = gameBoard.r;
                    //If the piece is the opposite color, allow captures
                    if (candidate.occupant.isLight != this.isLight)
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                else
                {
                    //Add square to list of legal moves
                    legalSquares.Add(candidate);
                }
            }
            //For each square in the same row going left, check if move is legal
            i = currentSquare.c; //start at the current column
            while (i > 0)
            {
                i--;
                //Stores the candidate 
                Square candidate = gameBoard.boardSquares[currentSquare.r, i];
                //If there is a piece in the candidate square
                if (candidate.occupied)
                {
                    //Break loop
                    i = -1;
                    //If the piece is the opposite color, allow captures
                    if (candidate.occupant.isLight != this.isLight)
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                else
                {
                    //Add square to list of legal moves
                    legalSquares.Add(candidate);
                }
            }
        }
    }
    
    class Bishop : Piece
    {
        //Constructor - ADD VALUE/TYPE
        public Bishop(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard)
        {
            value = 3;
            pieceType = "bishop";
        }

        //Draws the piece
        override public void Draw(Graphics g)
        {
            Rectangle r = currentSquare.shape;
            //Alter the shape of the piece so that its not identical to the rook 
            //CONSIDER STORING SOMEWHERE INSTEAD OF RECALCULATING EACH TIME
            r.Width -= 8;
            r.Height -= 8;
            r.X += 4;
            r.Y += 4;

            g.FillRectangle(b, r); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Generates the list of legal moves
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();

            //Start at current square
            int R = currentSquare.r;
            int C = currentSquare.c;

            //For each square in a diagonal row going up and to the left, check if move is legal
            while (0 <= R && R < gameBoard.r  && 0 <= C && C < gameBoard.c)
            {
                Square candidate = gameBoard.boardSquares[R,C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        R = -1;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R--;
                C--;
            }

            //For each square in a diagonal row going up and to the right, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        R = -1;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R--;
                C++;
            }
            //For each square in a diagonal row going down and to the right, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        R = -1;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R++;
                C++;
            }
            //For each square in a diagonal row going down and to the left, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        R = -1;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R++;
                C--;
            }
        }
    }
    
    class Queen : Piece
    {
        //Constructor - ADD VALUE/TYPE
        public Queen(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard)
        {
            value = 9;
            pieceType = "queen";
        }

        //Draws the piece
        override public void Draw(Graphics g)
        {
            Rectangle r = currentSquare.shape;
            //Alter the shape of the piece so that its not identical to the rook 
            //CONSIDER STORING SOMEWHERE INSTEAD OF RECALCULATING EACH TIME
            r.Width -= 8;
            r.Height -= 8;
            r.X += 4;
            r.Y += 4;

            g.FillEllipse(b, r); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Generates the list of legal moves - FIX, QUEEN SOMETIME CAN JUMP OVER BISHOP WITHIN 1 square
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();


            /*
             * ROOk SQUARE LOGIC  - FIX, ACTING AS BISHOP LOGIC CURRENTLY
             */
            bool searchFlag = true;

            //Start at current square
            int R = currentSquare.r;
            int C = currentSquare.c;
            searchFlag = true; //set search flag to true
            //For each square in a row going to the left, check if move is legal
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                C--;
            }
            
            //For each square in a row going right, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                C++;
            }
            
            //For each square in a column going up, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R++;
            }
            //For each square in a column going down, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R--;
            }
            
            /*
             * BISHOP SQUARE LOGIC
             */

            //Start at current square
            R = currentSquare.r;
            C = currentSquare.c;
            searchFlag = true; //set search flag to true
            //For each square in a diagonal row going up and to the left, check if move is legal
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R--;
                C--;
            }

            //For each square in a diagonal row going up and to the right, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R--;
                C++;
            }
            //For each square in a diagonal row going down and to the right, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R++;
                C++;
            }
            //For each square in a diagonal row going down and to the left, check if move is legal
            R = currentSquare.r; //start again at current square
            C = currentSquare.c; //start again at current square
            searchFlag = true; //set search flag to true
            while (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c && searchFlag)
            {
                Square candidate = gameBoard.boardSquares[R, C];
                if (candidate != currentSquare)
                {
                    if (candidate.occupied)
                    {
                        //Break loop
                        searchFlag = false;
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
                R++;
                C--;
            }
        }
    }
    
    
    class King : Piece
    {
        //Constructor - ADD VALUE/TYPE
        public King(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard)
        {
            value = -1;
            pieceType = "king";
        }

        //Draws the piece
        override public void Draw(Graphics g)
        {
            Rectangle r = currentSquare.shape;
            //Alter the shape of the piece so that its not identical to the rook 
            //CONSIDER STORING SOMEWHERE INSTEAD OF RECALCULATING EACH TIME
            r.Width -= 16;
            r.Height -= 16;
            r.X += 8;
            r.Y += 8;

            //points for cool crown icon
            PointF TopLef = new PointF(r.X, r.Y);
            PointF MidLef = new PointF(r.X + r.Width * 1 / 4, r.Y + r.Height / 2);
            PointF TopMid = new PointF(r.X + r.Width/2, r.Y);
            PointF MidRig = new PointF(r.X + r.Width * 3 / 4, r.Y + r.Height/2);
            PointF TopRig = new PointF(r.X + r.Width, r.Y);
            PointF BotRig = new PointF(r.X + r.Width, r.Y + r.Height);
            PointF BotLef = new PointF(r.X, r.Y + r.Height);
            PointF[] points = { TopLef, MidLef, TopMid, MidRig, TopRig, BotRig, BotLef };

            g.FillPolygon(b, points); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Generates the list of legal moves - FIX, QUEEN SOMETIME CAN JUMP OVER BISHOP WITHIN 1 square
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();

            //set R, C to hold coords of current square
            int R = currentSquare.r;
            int C = currentSquare.c;

            //create a list of the possible spaces the knight can move relative to the current square
            List<KeyValuePair<int, int>> coords = new List<KeyValuePair<int, int>>();
            coords.Add(new KeyValuePair<int, int>(1, 0));
            coords.Add(new KeyValuePair<int, int>(-1, 0));
            coords.Add(new KeyValuePair<int, int>(0, 1));
            coords.Add(new KeyValuePair<int, int>(0, -1));
            coords.Add(new KeyValuePair<int, int>(-1, 1));
            coords.Add(new KeyValuePair<int, int>(1, -1));
            coords.Add(new KeyValuePair<int, int>(-1, -1));
            coords.Add(new KeyValuePair<int, int>(1, 1));

            //Iterate over the list
            foreach (KeyValuePair<int, int> p in coords)
            {
                //Translate the values so they are relative to the current square
                C = currentSquare.c + p.Key;
                R = currentSquare.r + p.Value;

                //Check the values within the board to see if they are accessible
                if (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
                {
                    Square candidate = gameBoard.boardSquares[R, C];
                    if (candidate.occupied)
                    {
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
            }
        }
    }


    class Knight : Piece
    {
        //Constructor - ADD VALUE/TYPE
        public Knight(SolidBrush b, bool isLight, Square startingSquare, Board gameBoard) : base(b, isLight, startingSquare, gameBoard)
        {
            value = 3;
            pieceType = "knight";
        }

        //Draws the piece
        override public void Draw(Graphics g)
        {
            Rectangle r = currentSquare.shape;
            //Alter the shape of the piece so that its not identical to the rook 
            //CONSIDER STORING SOMEWHERE INSTEAD OF RECALCULATING EACH TIME
            r.Width -= 8;
            r.Height -= 50;
            r.X += 4;
            r.Y += 25;

            g.FillEllipse(b, r); //FIX
        }

        //Checks if given square is a legal move
        override public bool checkLegalMove(Square target)
        {
            if (legalSquares.Contains(target))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Generates the list of legal moves
        override public void generateLegalMoves()
        {
            //Clear list of legal squares
            legalSquares.Clear();
            
            //set R, C to hold coords of current square
            int R = currentSquare.r;
            int C = currentSquare.c;

            //create a list of the possible spaces the knight can move relative to the current square
            List<KeyValuePair<int,int>> coords = new List<KeyValuePair<int, int>>();
            coords.Add(new KeyValuePair<int, int>(1, 2));
            coords.Add(new KeyValuePair<int, int>(2, 1));
            coords.Add(new KeyValuePair<int, int>(-1, 2));
            coords.Add(new KeyValuePair<int, int>(-2, 1));
            coords.Add(new KeyValuePair<int, int>(1, -2));
            coords.Add(new KeyValuePair<int, int>(2, -1));
            coords.Add(new KeyValuePair<int, int>(-1, -2));
            coords.Add(new KeyValuePair<int, int>(-2, -1));
            
            //Iterate over the list
            foreach(KeyValuePair<int,int> p in coords)
            {
                //Translate the values so they are relative to the current square
                C = currentSquare.c + p.Key;
                R = currentSquare.r + p.Value;

                //Check the values within the board to see if they are accessible
                if (0 <= R && R < gameBoard.r && 0 <= C && C < gameBoard.c)
                {
                    Square candidate = gameBoard.boardSquares[R, C];
                    if (candidate.occupied)
                    {
                        //If the piece is the opposite color, allow captures
                        if (candidate.occupant.isLight != this.isLight)
                        {
                            //Add square to list of legal moves
                            legalSquares.Add(candidate);
                        }
                    }
                    else
                    {
                        //Add square to list of legal moves
                        legalSquares.Add(candidate);
                    }
                }
            }
        }
    } 
}
