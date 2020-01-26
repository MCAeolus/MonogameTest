using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTest.blocks
{
    abstract class BlockCollection
    {

        public enum Blocks { ThreeThree, TwoTwo, BigL, SmallL, ZShape, TShape, TwoStick, ThreeStick, FourStick, FiveStick, Dot };
        public static Dictionary<Blocks, BlockCollection> collectionDictionary = new Dictionary<Blocks, BlockCollection>()
        {
            { Blocks.ThreeThree, new ThreeThreeShape() },
            { Blocks.TwoTwo, new TwoTwoShape() },
            { Blocks.BigL, new BigLShape() },
            { Blocks.SmallL, new SmallLShape() },
            { Blocks.ZShape, new ZShape() },
            { Blocks.TShape, new TShape() },
            { Blocks.TwoStick, new TwoStick() },
            { Blocks.ThreeStick, new ThreeStick() },
            { Blocks.FourStick, new FourStick() },
            { Blocks.FiveStick, new FiveStick() },
            { Blocks.Dot, new Dot() }
        };

        public Color tint
        {
            private set;
            get;
        }
        public Boolean[,] layout
        {
            get;
            private set;
        }
        
        public BlockCollection(Color tint, Boolean[,] layout)
        {
            this.layout = layout;
            this.tint = tint;
        }

        public TrayPiece getTrayPiece()
        {
            return new TrayPiece(this, getRotatedLayout(Game1.random.Next(0, 3)));
        }

        public static BoardPiece[,] getPieces(bool[,] layout, Color tint)
        {
            BoardPiece[,] pieces = new BoardPiece[layout.GetLength(0), layout.GetLength(1)];

            for (int i = 0; i < layout.GetLength(0); i++)
            {
                for (int j = 0; j < layout.GetLength(1); j++)
                {

                    if (layout[i, j])
                    {
                        pieces[i, j] = new BoardPiece(new Vector2(i, j), tint);
                    }


                }
            }

            return pieces;
        }

        public BoardPiece[,] getDefaultPieces()
        {

            BoardPiece[,] pieces = new BoardPiece[layout.GetLength(0), layout.GetLength(1)];

            for (int i = 0; i < layout.GetLength(0); i++)
            {
                for (int j = 0; j < layout.GetLength(1); j++)
                {

                    if (layout[i, j])
                    {
                        pieces[i, j] = new BoardPiece(new Vector2(i, j), tint);
                    }


                }
            }

            return pieces;
        }

        public bool[,] getRotatedLayout(int times)
        {
            bool[,] layout = (bool[,])this.layout.Clone();
            times = Math.Abs(times % 4);

            while (times-- > 0) //naive clockwise rotation... could be made more efficient but for this game the method doesn't need to be very fast (and matrices are small so fast anyways)
            {

                bool[,] rotatedMat = new bool[layout.GetLength(1), layout.GetLength(0)];

                for (int i = 0; i < layout.GetLength(0); i++ ) //perform a transpose and reverse the row.
                {
                    for ( int j = 0; j < layout.GetLength(1); j++ )
                    {

                        rotatedMat[j, i] = layout[layout.GetLength(0) - i - 1, j];
                          
                    }
                }

                layout = rotatedMat;


            }

            return layout;

        }

    }
    class ThreeThreeShape : BlockCollection
    {

        public ThreeThreeShape() : base(Color.Blue, new bool[,]{    { true, true, true },
                                                                    { true, true, true },
                                                                    { true, true, true } })
        {
        }
       
    }
   
    class TwoTwoShape : BlockCollection
    {
        public TwoTwoShape() : base(Color.Green, new bool[,] { { true, true }, 
                                                               { true, true } })
        {
        }
    }

    class BigLShape : BlockCollection
    {
        public BigLShape() : base(Color.Yellow, new bool[,] { { true, false, false },
                                                              { true, false, false },
                                                              { true, true,  true } })
        {

        }
    }

    class SmallLShape : BlockCollection
    {

        public SmallLShape() : base(Color.Orange, new bool[,] { { true, false },
                                                                { true, true  } })
        {

        }

    }
    class ZShape : BlockCollection
    {
        public ZShape() : base(Color.Purple, new bool[,] { { true, true,  false },
                                                           { false, true, true  } })
        {

        }
    }

    class TShape : BlockCollection
    {
        public TShape() : base(Color.Red, new bool[,] { { true, true, true },
                                                        { false, true, false  }})
        {

        }
    }
    
    class TwoStick : BlockCollection
    {
        public TwoStick() : base(Color.Orange, new bool[,] { { true, true} })
        {

        }
    }

    class ThreeStick : BlockCollection
    {
        public ThreeStick() : base(Color.Pink, new bool[,] { { true, true, true } })
        {
        }
    }

    class FourStick : BlockCollection
    {
        public FourStick() : base(Color.Magenta, new bool[,] { {true, true, true, true}})
        {

        }
    }

    class FiveStick : BlockCollection
    {
        public FiveStick() : base(Color.Purple, new bool[,] { { true, true, true, true, true} })
        {

        }
    }

    class Dot : BlockCollection
    {
        public Dot() : base(Color.White, new bool[,] { {true} })
        {

        }
    }

    class TrayPiece
    {

        public BlockCollection collection
        {
            private set;
            get;
        }
        public BoardPiece[,] pieceInstance
        {
            private set;
            get;
        }
        public bool[,] layoutInstance
        {
            private set;
            get;
        }
        public Vector2 position = new Vector2(0, 0);
        public Vector2 trayPosition = new Vector2(0, 0);
        public Boolean isMoving = false;
        public Vector2 offsetMouse = new Vector2(0, 0);

        public TrayPiece(BlockCollection collection, bool[,] instanceLayout)
        {
            this.collection = collection;
            this.layoutInstance = instanceLayout;
            this.pieceInstance = BlockCollection.getPieces(instanceLayout, collection.tint);
        }
        

        public void drawPieces(SpriteBatch sb, Texture2D blockTexture)
        {
            foreach(BoardPiece piece in pieceInstance)
            {
                if (piece == null) continue;

                sb.Draw(blockTexture, position + piece.position * 32, piece.tint);

            }
        }
    }
}

