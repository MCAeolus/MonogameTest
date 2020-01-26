using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonogameTest
{
    class DrawingUtility
    {

        private Texture2D drawingTexture;

        public DrawingUtility(ContentManager content)
        {

            drawingTexture = content.Load<Texture2D>("drawTexture");

        }


        public void DrawLine(SpriteBatch sb, Vector2 startPosition, Vector2 endPosition, Color tint, int width)
        {

            Vector2 line = endPosition - startPosition;

            float angle = (float)Math.Atan2(line.Y, line.X);
            Rectangle lineRectangle = new Rectangle((int)startPosition.X, (int)startPosition.Y, width, (int)line.Length()); //test how the width effects this....

            sb.Draw(drawingTexture, lineRectangle, null, tint, angle, new Vector2(0, 0), SpriteEffects.None, 0);
        }

        public static Color ColorFromHex(String hex)
        {
            hex = hex.Replace("#", "");

            int r = Convert.ToInt32(hex.Substring(0, 2), 16);
            int g = Convert.ToInt32(hex.Substring(2, 2), 16);
            int b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return new Color(r, g, b);
        }
    }
}
