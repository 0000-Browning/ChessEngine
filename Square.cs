using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace ChessEngine
{
    class Square
    {
        //Position
        int x = 0;
        int y = 0;
        //Relative position
        public int r { get; protected set; } //row
        public int c { get; protected set; } //column
        //Dimensions
        int length = 20;
        //Square's shape
        public Rectangle shape;
        //Brush
        SolidBrush b;
        //Piece on square
        public Piece occupant { get; protected set; }
        public bool occupied {get; protected set;}

        //Constructor
        public Square(SolidBrush b, int length, int x, int y, int r, int c)
        {
            //Sets emptyness status
            occupied = false;
            //Passes position in 
            this.x = x;
            this.y = y;
            this.r = r;
            this.c = c;
            //Passes color in
            this.b = b;
            //Passes square size
            this.length = length;
            //Creates rectangle for the square
            shape = new Rectangle(x, y, length, length);
        }
        //Drawaing Method - Draws square of given size on given surface
        public void Draw(Graphics g, int length, int x, int y)
        {
            //Reassigns length/pos variabes
            this.length = length;
            this.x = x;
            this.y = y;
            //Redefines Square
            shape.X = x;
            shape.Y = y;
            shape.Width = length;
            shape.Height = length;
            //Draws the square
            g.FillRectangle(b, shape);
        }
        //Drawing Method (Unchanged) - Redraws the sqaure
        public void Draw(Graphics g)
        {
            //Draws the square
            g.FillRectangle(b, shape);
        }
        //Update occupant
        public void Fill(Piece target)
        {
            occupant = target;
            occupied = true;
        }
        //Updates occupant
        public void Empty()
        {
            occupant = null;
            occupied = false;
        }
    }
}
