using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTest
{
    class BoardPiece
    {

        public Vector2 position
        {
            get;
        }

        public Color tint
        {
            get;
        }

        public BoardPiece(Vector2 position, Color tint)
        {
            this.position = position;
            this.tint = tint;
        }
    }
}
