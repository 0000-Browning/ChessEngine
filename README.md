# ChessEngine
A chess game made in C# with windows forms. Built in two phases, about a year apart. Missing certain features, such as en passent, castling, promotion, and check/stalemate. The structure of the program is generally flawed, as encapsulation is not used properly, meaning that adding these features would likely require a total redesign.

File guide:

Board.cs - Class file for chess board
ChessEngine.sln - VS solution file for the whole project
ChessGameController.cs - Class file for the main game controller
Form1.Designer.cs - .NET design file for main form
Form1.cs - Top level code for the whole project
Piece.cs - Class file for the pieces
Program.cs - Boilerplate; initializes the main form
Square.cs - Class file for the board square

It is easiest to parse the code starting from Form1.cs and working downward. 
