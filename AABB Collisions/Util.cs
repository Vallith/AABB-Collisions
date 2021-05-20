using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    public static class Util
    {

        public static Texture2D GetColoredCircle(float radius, Color desiredColor)
        {
            int width = (int)radius * 2;
            int height = width;

            Vector2 center = new Vector2(radius, radius);


            Color[] dataColors = new Color[width * height];
            int row = -1; //increased on first iteration to zero!
            int column = 0;
            for (int i = 0; i < dataColors.Length; i++)
            {
                column++;
                if (i % width == 0) //if we reach the right side of the rectangle go to the next row as if we were using a 2D array.
                {
                    row++;
                    column = 0;
                }
                Vector2 point = new Vector2(row, column); //basically the next pixel.
                if (ContainsPoint(point))
                {
                    dataColors[i] = desiredColor; //point lies within the radius. Paint it.
                }
                else
                {
                    dataColors[i] = Color.Transparent; //point lies outside, leave it transparent.
                }

            }

            bool ContainsPoint(Vector2 point)
            {
                return ((point - center).Length() <= radius);
            }

            Texture2D texture = new Texture2D(Game1.instance.GraphicsDevice, width, height);
            texture.SetData(0, new Rectangle(0, 0, width, height), dataColors, 0, width * height);
            return texture;
        }

        public static Texture2D GetColoredSquare(float width, float height, Color desiredColor)
        {

            Color[] dataColors = new Color[(int)width * (int)height];

            for (int i = 0; i < dataColors.Length; i++)
            {
                dataColors[i] = desiredColor;
            }

            Texture2D texture = new Texture2D(Game1.instance.GraphicsDevice, (int)width, (int)height);
            texture.SetData(0, new Rectangle(0, 0, (int)width, (int)height), dataColors, 0, (int)width * (int)height);
            return texture;
        }


        public static void DrawAABB(AABB aabb)
        {
            Game1.instance._spriteBatch.DrawPoint(aabb.max, Color.Red, 10);
            Game1.instance._spriteBatch.DrawPoint(aabb.min, Color.Blue, 10);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.max.X, aabb.min.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.max.X, aabb.min.Y), aabb.max, Color.Red);
            Game1.instance._spriteBatch.DrawLine(aabb.min, new Vector2(aabb.min.X, aabb.max.Y), Color.Red);
            Game1.instance._spriteBatch.DrawLine(new Vector2(aabb.min.X, aabb.max.Y), aabb.max, Color.Red);
        }

    }
}
