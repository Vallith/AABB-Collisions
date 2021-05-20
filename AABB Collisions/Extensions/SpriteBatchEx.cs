using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AABB_Collisions
{
    public static class SpriteBatchEx
    {
        public static Texture2D px1;
        static SpriteBatchEx()
        {
            px1 = new Texture2D(Game1.instance.GraphicsDevice, 1, 1);
            Color[] dataColors = new Color[1] { Color.White };
            px1.SetData(0, new Rectangle(0, 0, 1, 1), dataColors, 0, 1);
        }
        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(px1, r, null, color, angle, Vector2.Zero, SpriteEffects.None, 0);
        }

        /// <summary>
        /// Draw a circle
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="radius">The radius of the circle</param>
        /// <param name="sides">The number of sides to generate</param>
        /// <param name="color">The color of the circle</param>
        /// <param name="thickness">The thickness of the lines used</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0)
        {
            DrawPolygon(spriteBatch, center, CreateCircle(radius, sides), color, thickness, layerDepth);
        }

        /// <summary>
        ///     Draws a closed polygon from an array of points
        /// </summary>
        /// <param name="spriteBatch">The destination drawing surface</param>
        /// ///
        /// <param name="offset">Where to offset the points</param>
        /// <param name="points">The points to connect with lines</param>
        /// <param name="color">The color to use</param>
        /// <param name="thickness">The thickness of the lines</param>
        /// <param name="layerDepth">The depth of the layer of this shape</param>
        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float layerDepth = 0)
        {
            if (points.Count == 0)
                return;

            if (points.Count == 1)
            {
                DrawPoint(spriteBatch, points[0], color, (int)thickness);
                return;
            }

            var texture = GetTexture(spriteBatch);

            for (var i = 0; i < points.Count - 1; i++)
                DrawPolygonEdge(spriteBatch, texture, points[i] + offset, points[i + 1] + offset, color, thickness, layerDepth);

            DrawPolygonEdge(spriteBatch, texture, points[points.Count - 1] + offset, points[0] + offset, color, thickness, layerDepth);
        }

        /// <summary>
        /// Draws a point at the specified position. The center of the point will be at the position.
        /// </summary>
        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f, float layerDepth = 0)
        {
            var scale = Vector2.One * size;
            var offset = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), position + offset, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_whitePixelTexture == null)
            {
                _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _whitePixelTexture.SetData(new[] { Color.White });
                spriteBatch.Disposing += (sender, args) =>
                {
                    _whitePixelTexture?.Dispose();
                    _whitePixelTexture = null;
                };
            }

            return _whitePixelTexture;
        }

        private static void DrawPolygonEdge(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth)
        {
            var length = Vector2.Distance(point1, point2);
            var angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            var scale = new Vector2(length, thickness);
            spriteBatch.Draw(texture, point1, null, color, angle, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
        }

        private static Vector2[] CreateCircle(double radius, int sides)
        {
            const double max = 2.0 * Math.PI;
            var points = new Vector2[sides];
            var step = max / sides;
            var theta = 0.0;

            for (var i = 0; i < sides; i++)
            {
                points[i] = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
                theta += step;
            }

            return points;
        }

    }
}
