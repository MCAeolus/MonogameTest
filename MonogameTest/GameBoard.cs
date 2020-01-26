using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonogameTest.blocks;
using System;
using System.Collections.Generic;

namespace MonogameTest
{
    /*
    * bugs found:
    * - four piece laying sideways was detected as not being placed but bottom right corner had space for the piece. 
    * - more instances occuring.... SPACE MATCHING DOES ___NOT___ WORK. Either use less efficient method or re-consider the problem.
    * 
    * - even with naive searching still finding error.... may be way vectors are beign searched (ex. ignoring row or column...)
    * 
    */ 

    class GameBoard
    {

        private static Vector2 boardSize = new Vector2(10, 10);
        private static Vector2 pieceSize = new Vector2(32, 32);

        private BoardPiece[,] board = new BoardPiece[(int)boardSize.X, (int)boardSize.Y];
        private Texture2D gridBack;
        private Texture2D blockTexture;
        private Texture2D trayBackground;

        private int spacing = 0;

        private Vector2 topLeftBoardPosition = new Vector2(20, 20);

        private Vector2 topLeftTrayPosition = new Vector2(500, 20);

        private List<TrayPiece> trayPieces = new List<TrayPiece>();
        private int trayCount;

        private TrayPiece trayPieceMoving = null; //for the moving function

        public GameBoard(ContentManager content)
        {
            gridBack = content.Load<Texture2D>("gridback");
            blockTexture = content.Load<Texture2D>("block");
            trayBackground = content.Load<Texture2D>("trayBackground");

            NewTrayPieces(); //initiate the tray pieces.

        }

        private void NewTrayPieces() //TODO center tray pieces. make tray prettier in general... but fully functioning.
        {
            BlockCollection.Blocks[] enums = (BlockCollection.Blocks[])Enum.GetValues(typeof(BlockCollection.Blocks));
            Vector2 topVector = topLeftTrayPosition + new Vector2(50, 20);


            for(int i = 0; i < 3; i++)
            {
                TrayPiece trayPiece = BlockCollection.collectionDictionary[enums[Game1.random.Next(0, enums.Length)]].getTrayPiece();
                trayPiece.trayPosition = trayPiece.position = topVector + new Vector2(0, 120)*i;
                trayPieces.Add(trayPiece);
            }

            trayCount = 3;
        }

        public void MoveTrayPiece(MouseState mouseState) //this contains all board operational logic.
        {

            if (Game1.gameover) return;

            //if (mouseState.LeftButton != ButtonState.Pressed) return;

            Vector2 mousePosition = mouseState.Position.ToVector2();

            if (trayPieceMoving != null)
            {
                TrayPiece piece = trayPieceMoving;

                if (mouseState.LeftButton == ButtonState.Pressed) //moving piece
                {
                    piece.position = mousePosition + piece.offsetMouse;
                }
                else //piece dropped
                {
                    piece.isMoving = false;
                    Vector2 relativePosition = getApproximateRelativePosition(piece.position, doRound: true);
                    //Console.WriteLine(relativePosition);


                    if (addBlockPieces(piece, relativePosition))
                    {

                        trayPieces.Remove(piece); //remove tray piece.

                        if (--trayCount <= 0) NewTrayPieces();

                        DoGameUpdate();
                        //logic should be performed here to make sure the player can place the pieces.
                        //this is a relatively large time complex check, but is also sparsely run.
                        TrayPiece smallestPiece = trayPieces[0];
                        int smallestPieceSize = smallestPiece.layoutInstance.GetLength(0) * smallestPiece.layoutInstance.GetLength(1);

                        for(int i = 1; i < trayPieces.Count; i++) //start at index 1 because 0 is done above.
                        {
                            TrayPiece tp = trayPieces[i];
                            int len = tp.layoutInstance.GetLength(0) * tp.layoutInstance.GetLength(1);
                            if (len < smallestPieceSize)
                            {
                                smallestPiece = tp;
                                smallestPieceSize = len;
                            }
                        }

                        bool[,] searchedBoard = new bool[board.GetLength(0), board.GetLength(1)]; //default populated false

                        bool matchFound = false;
                        Vector2[] vectorsToMatch = makeLayoutVectors(smallestPiece.layoutInstance);//relative to top left
                        for (int i = 0; i < board.GetLength(0) - smallestPiece.layoutInstance.GetLength(0); i++)
                        {
                            for (int j = 0; j < board.GetLength(1) - smallestPiece.layoutInstance.GetLength(1); j++)
                            {
                                //if (searchedBoard[i, j]) continue;

                                //if(board[i,j] == null)
                                {
                                    if (matchLayout(vectorsToMatch, new Vector2(i, j), searchedBoard))
                                    {
                                        matchFound = true;
                                        break;
                                    }
                                  
                                }
                                searchedBoard[i, j] = true;

                            }
                        }

                        if (!matchFound) //game over!
                        {
                            Console.WriteLine("game over!");
                            Game1.setGameOver();
                        }
                            

                    } else
                        piece.position = piece.trayPosition;

                    trayPieceMoving = null;

                }
            }
            else if(mouseState.LeftButton == ButtonState.Pressed)
            {

                foreach (TrayPiece piece in trayPieces)
                {
                    if (piece == null) continue;

                    foreach (BoardPiece boardBlock in piece.pieceInstance)
                    {

                        if (boardBlock == null) continue;

                        Point leftCorner = (piece.position + boardBlock.position * 32).ToPoint();
                        if (new Rectangle(leftCorner, new Point(32, 32)).Contains(mousePosition))
                        {

                            piece.offsetMouse = (piece.position - mousePosition);
                            piece.isMoving = true;
                            trayPieceMoving = piece;
                            break;

                        }
                    }
                }
            }

        }

        private bool matchLayout(Vector2[] vectorsMatch, Vector2 initCoords, bool[,] searchedBoard)
        {

            foreach(Vector2 vec in vectorsMatch)
            {
                int i = (int)(vec.X + initCoords.X);
                int j = (int)(vec.Y + initCoords.Y);

                //Console.WriteLine("loc" + i + "," + j);
                
                searchedBoard[i, j] = true;

                if(board[i,j] != null) return false;
                
            }

            return true;

        }

        private Vector2[] makeLayoutVectors(bool[,] layout)
        {
            List<Vector2> vectors = new List<Vector2>();

            //Console.WriteLine("layout len" + layout.GetLength(0) + "," + layout.GetLength(1));

            for(int i = 0; i < layout.GetLength(0); i++ )
            {
                for(int j = 0; j < layout.GetLength(1); j++)
                {
                    if(layout[i,j])
                    {
                        vectors.Add(new Vector2(i, j));
                    }
                }
            }

            return vectors.ToArray();
        }

        public void DrawBoard(SpriteBatch sb)
        {

            sb.Draw(trayBackground, topLeftTrayPosition, Color.White);

            for (int i = 0; i < (int)boardSize.X; i++)
            {
                for (int j = 0; j < (int)boardSize.Y; j++)
                {
                    BoardPiece piece = board[i,j];
                    Vector2 topLeft = topLeftBoardPosition + new Vector2(i * (pieceSize.X + spacing), j * (pieceSize.Y + spacing));

                    if (piece == null) sb.Draw(gridBack, topLeft, Color.White);
                    else sb.Draw(blockTexture, topLeft, piece.tint);
                }
            }

            foreach (TrayPiece piece in trayPieces)
            {
                if (piece != null)
                {

                    piece.drawPieces(sb, blockTexture);

                }
            }

        }

        public void DoGameUpdate()
        {

            //perform check for rows that should be cleared.
            
            //check cols
            for( int i = 0; i < board.GetLength(0); i++ )
            {
                bool colFilled = true;
                for (int j = 0; j < board.GetLength(1) && colFilled; j++ )
                {

                    if (board[i, j] == null)
                        colFilled = false;
                    
                }

                if(colFilled)
                {
                    for (int j = 0; j < board.GetLength(1); j++)
                    {

                        board[i, j] = null;

                    }
                }

            }

            //check rows
            for( int j = 0; j < board.GetLength(1); j++)
            {
                bool rowFilled = true;
                for (int i = 0; i < board.GetLength(0) && rowFilled; i++)
                {

                    if (board[i, j] == null)
                        rowFilled = false;

                }

                if (rowFilled)
                {
                    for (int i = 0; i < board.GetLength(0); i++)
                    {

                        board[i, j] = null;

                    }
                }
                
            }

        }

        public Vector2 getApproximateRelativePosition(Vector2 absoluteWindowPosition, bool doRound = false)
        {
            Vector2 relative = absoluteWindowPosition - topLeftBoardPosition;
            relative = relative * new Vector2(1 / (pieceSize.X + spacing), 1 / (pieceSize.Y + spacing));

            if (doRound) return new Vector2((float)Math.Round(relative.X), (float)Math.Round(relative.Y));
            else return new Vector2((float)Math.Floor(relative.X), (float)Math.Floor(relative.Y));
        }

        public Boolean addBlockPieces(TrayPiece trayPiece, Vector2 topLeftRel)
        {
            Boolean[,] layout = trayPiece.layoutInstance;
            for (int i = 0; i < layout.GetLength(0); i++)
            {
                for (int j = 0; j < layout.GetLength(1); j++)
                {
                    Vector2 relativeLocation = new Vector2(topLeftRel.X + i, topLeftRel.Y + j);

                    if (relativeLocation.X >= boardSize.X || relativeLocation.Y >= boardSize.Y || relativeLocation.X < 0 || relativeLocation.Y < 0) return false;

                    if(layout[i, j])
                    {
                        if (board[(int)(relativeLocation.X), (int)(relativeLocation.Y)] != null) return false;
                    }

                }
            }

            foreach(BoardPiece piece in trayPiece.pieceInstance)
            {
                if (piece == null) continue;
                board[(int)(piece.position.X + topLeftRel.X), (int)(piece.position.Y + topLeftRel.Y)] = piece;
            }

            return true;
        }

    }
}