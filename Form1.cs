using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessEngine
{
    /*
    Author: Jake Browning
    Program: Chess Engine v0.9

    INCOMPLETE/missing features:
        Checks in click event for Controller
        Promotion
        Castling
        Stale/Checkmate - Will probably require total redesign/something clever

    Design Notes:
        Resizable Board
            Fix Board.Draw(stuff) - Doesn't adjust piece/square sizes
            Configure other stuff
        Add Pieces
            Pawn  - paritally - NEED en passent, promotion  
            Rook - partially - NEED castling
            Bishop - done
            Queen - done
            Knight - done
            King - partially - NEED checkmate, castling
            Piece Designs - King sprite created (inefficient method)
        Add Piece movement - DONE
            Configure Click Events - DONE
            Allow Piece Captures - DONE
            Add square highlights (where a piece can move) (?)
            Enforce movement rules
                Blocked Pieces - DONE
                Check - Done (Needs improvement)
                Pawn Double move - DONE
                En Passent - NEED
                Castling - NEED
                Promotion - NEED
                Update Square Highlights to reflect restrictions 
        Import/Design Piece Graphics
            Add Options (?) - Partially
        Configure controller:
            Alternate turns - DONE
            Clock (?)
            Piece value counter (?) - Implemented, not displayed
            Game End Conditions
     */

    public partial class Form1 : Form
    {
        //Graphics surface
        Graphics g; //INCON - Create Graphics surface
        //Board object
        ChessGameController test;

        //Instantializes Form 
        public Form1()
        {
            InitializeComponent();
        }

        //Instantializes Graphics object
        private void Form1_Load(object sender, EventArgs e)
        {
            g = panel1.CreateGraphics();
            
        }

        //First load
        private void Form1_Shown(object sender, EventArgs e)
        {
            test = new ChessGameController(g, 10, 10, 8, 8, 40, Color.Tan, Color.Brown);
            test.Update();
        }
        
        //Click Event Handler
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            test.Click(e.X, e.Y);

        }
    }
}
