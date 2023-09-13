using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDelta.Helpers
{
    public static class SpriteBatchExtensions
    {
        public static Texture2D Pixel { get; set; }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            Pixel = new Texture2D(graphicsDevice, 1, 1);
            Pixel.SetData<Color>(new Color[] { Color.White });
        }

        public static void DrawHollowRect(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            //  Calculate the rectangle corner positions
            Point topLeft = new Point(rect.Left, rect.Top);
            Point topRight = new Point(rect.Right, rect.Top);
            Point bottomRight = new Point(rect.Right, rect.Bottom);
            Point bottomLeft = new Point(rect.Left, rect.Bottom);

            //  Draw edges corner to corner
            spriteBatch.DrawLine(topLeft, topRight, color);
            spriteBatch.DrawLine(topRight, bottomRight, color);
            spriteBatch.DrawLine(bottomRight, bottomLeft, color);
            spriteBatch.DrawLine(bottomLeft, topLeft, color);
        }

        public static void DrawFilledRect(this SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            // Create a 1x1 pixel texture filled with the specified color
            Texture2D filledPixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            filledPixel.SetData(new[] { color });

            // Draw the filled rectangle
            spriteBatch.Draw(filledPixel, rect, Color.White);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Point start, Point end, Color color)
        {
            //  Calculate the angle of the line
            float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);

            //  Calculate distance
            float p1 = start.X - end.X;
            float p2 = start.Y - end.Y;
            float distance = MathF.Sqrt((p1 * p1) + (p2 * p2));

            //  Draw the line
            spriteBatch.Draw(texture: Pixel,
                             position: start.ToVector2(),
                             sourceRectangle: null,
                             color: color,
                             rotation: angle,
                             origin: Vector2.Zero,
                             scale: new Vector2(distance, 1.0f),
                             effects: SpriteEffects.None,
                             layerDepth: 1f);
        }
    }

}
