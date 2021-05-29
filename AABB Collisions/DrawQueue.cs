using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace AABB_Collisions
{
    static class DrawQueue
    {
        /// <summary>
        /// A list that contains queued drawing commands, is does the draw only once, it will need to be reinserted before the next frame to be drawn again.
        /// </summary>
        static List<IQueueDraw> delayedDraws = new List<IQueueDraw>();
        /// <summary>
        /// Contains permanet draws these will last for the entire program unless manually deleted.
        /// </summary>
        static List<IQueueDraw> permanentDraws = new List<IQueueDraw>();

        /// <summary>
        /// Adds an IQueueDraw to the Queued Drawing System, useful if need to draw in a scope which can't have batched rendering enabled.
        /// </summary>
        public static void Add(IQueueDraw draw, bool permanent = false)
        {
            if (permanent)
            {
                permanentDraws.Add(draw);
            }
            else
            {
                delayedDraws.Add(draw);
            }
        }

        /// <summary>
        /// Begins the drawing of the queued drawings. (requires batch rendering to be active)
        /// </summary>
        public static void DrawAll()
        {
            foreach (var item in delayedDraws)
            {
                item.Draw();
            }

            foreach (var item in permanentDraws)
            {
                item.Draw();
            }

            delayedDraws.Clear();
        }
    }

    interface IQueueDraw
    {
        void Draw();
    }

    struct DrawString : IQueueDraw
    {
        string text;
        Vector2 position;
        Color color;
        SpriteFont spriteFont;

        public static DrawString Create(string text, Vector2 position, Color color, SpriteFont spriteFont = null)
        {
            if (spriteFont == null)
            {
                spriteFont = Game1.instance.defaultFont;
            }
            return new DrawString() { text = text, position = position, color = color, spriteFont = spriteFont };
        }

        void IQueueDraw.Draw()
        {
            Screen.Draw.DrawString(spriteFont, text, position, color);
        }
    }

    struct DrawRect : IQueueDraw
    {
        Vector2 position;
        int width, height, thick;
       
        Color color;

        public static DrawRect Create(Vector2 position, int width, int height, Color color, int thick = 2)
        {
            return new DrawRect() { position = position, width = width, height = height, thick=thick, color = color };
        }

        void IQueueDraw.Draw()
        {
            Screen.Draw.DrawRect(position,width, height, color, thick);
        }
    }

    struct DrawCircle : IQueueDraw
    {
        Vector2 position;
        float radius;
        int thick;

        Color color;


        public static DrawCircle Create(Vector2 position, float radius, Color color, int thick = 2)
        {
            return new DrawCircle() { position = position, thick=thick, radius = radius, color = color };
        }

        void IQueueDraw.Draw()
        {
            Screen.Draw.DrawCircle(position, radius, 20, color, thick);
        }
    }

    struct DrawPoint : IQueueDraw
    {
        Vector2 position;
        int size;

        Color color;

        public static DrawPoint Create(Vector2 position, Color color, int thick = 5)
        {
            return new DrawPoint() { position = position, size = thick,  color = color };
        }

        void IQueueDraw.Draw()
        {
            Screen.Draw.DrawPoint(position, color, size);
        }
    }

    struct DrawLine : IQueueDraw
    {
        Vector2 from;
        Vector2 to;
        int thick;

        Color color;

        public static DrawLine Create(Vector2 from, Vector2 to, Color color, int thick = 2)
        {
            return new DrawLine() {from = from, to = to, thick = thick, color = color };
        }

        void IQueueDraw.Draw()
        {
            Screen.Draw.DrawLine(from, to, color, thick);
        }
    }
}
