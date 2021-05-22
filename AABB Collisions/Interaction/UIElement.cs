using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace AABB_Collisions
{
    public abstract class UIElement
    {
        protected Vector2 pos;
        protected Vector2 Pos { get => pos; }

        public List<AABB> aabbs = new List<AABB>();

        public UIElement(Vector2 pos)
        {
            this.pos = pos;
            Game1.instance.uiElements.Add(this);
        }

        // Draws the elements GUI
        public abstract void DrawGUI();
        // Handles user input
        public abstract void InputControl();
        // If a UIElement has anything which moves
        // the AABB will need to be recalculated to
        // match it's new position
        public abstract void AABBRecalculate();
    }

    public class RadioButton : UIElement
    {
        public float radius;
        public float lineWidth;
        public float circleSize;

        public Texture2D texture;
        public bool isTicked = false;

        public RadioButton(Vector2 pos, float radius, float lineWidth = 1, bool isTicked = false) : base(pos)
        {
            this.radius = radius;
            this.lineWidth = lineWidth;
            this.isTicked = isTicked;
            circleSize = radius - lineWidth - (radius * 0.2f);
            texture = Util.GetColoredCircle(circleSize, Color.White);
            aabbs.Add(AABB.CreateCircleAABB(pos, radius));
        }

        public override void AABBRecalculate()
        {
            aabbs[0].Pos = pos;
        }

        public override void DrawGUI()
        {
            Screen.Draw.DrawCircle(Pos, radius, 30, Color.White, lineWidth);
            if (isTicked)
            {
                Screen.Draw.Draw(texture, new Rectangle((int)pos.X - texture.Width / 2, (int)pos.Y - texture.Height / 2, texture.Width, texture.Height), Color.White);
            }
        }

        public override void InputControl()
        {
            if (Input.IsPressed(MouseButton.LeftButton) && AABB.InsideSquare(aabbs[0].TopLeft, aabbs[0].BottomRight, Input.mouse.Position.ToVector2()))
            {
                isTicked = !isTicked;
            }
        }
    }


    public class Slider : UIElement
    {

        // Position of the current position of the circle
        private float currentX;

        // Values for the slider
        public float minValue, maxValue, value;
        public float step;

        // Size of inner circle
        public float circleSize;
        // Size of outer circle
        public float dragRadius;
        // Length of slider in pixels
        private float sliderLength;
        public float Value { get => value; }
        private Vector2 dragPos => new Vector2(currentX, pos.Y);

        public Texture2D texture;

        bool dragging = false;

        bool drawValue;

        public delegate void SliderChanged(Slider slider, float value);

        public SliderChanged onSliderChanged;

        public Slider(Vector2 pos, float radius, float length, float min, float max, bool drawValue = false) : base(pos)
        {
            currentX = pos.X;
            value = min;
            dragRadius = radius;
            sliderLength = length;
            minValue = min;
            maxValue = max;
            this.drawValue = drawValue;
            circleSize = radius - 1 - (radius * 0.2f);
            texture = Util.GetColoredCircle(circleSize, Color.White);
            aabbs.Add(AABB.CreateCircleAABB(new Vector2(currentX, pos.Y), radius));
        }

        public override void InputControl() {
            if (Input.IsPressed(MouseButton.LeftButton) || dragging)
            {
                Vector2 mouse = Input.mouse.Position.ToVector2();
                if (dragging || AABB.InsideCircle(mouse, dragPos, dragRadius))
                {
                    dragging = true;
                    currentX = Math.Clamp(mouse.X, pos.X, pos.X + sliderLength);
                    value = Util.Remap(currentX, pos.X, pos.X + sliderLength, minValue, maxValue);
                    if (onSliderChanged != null)
                    {
                        onSliderChanged(this, value);
                    }
                    AABBRecalculate();
                }
            }
            if (!Input.IsHeld(MouseButton.LeftButton))
            {
                dragging = false;
            }
        }

        public override void DrawGUI()
        {
            Screen.Draw.DrawLine(pos, new Vector2(pos.X + sliderLength, pos.Y),Color.White);
            Screen.Draw.DrawCircle(new Vector2(currentX, pos.Y), dragRadius, 20, Color.White);
            Screen.Draw.Draw(texture, new Rectangle((int)currentX - (int)texture.Width / 2, (int)pos.Y - (int)texture.Height / 2, texture.Width, texture.Height), Color.White);
            if (drawValue == true)
            {
                Screen.Draw.DrawString(Game1.instance.defaultFont, value.ToString(), new Vector2(pos.X, pos.Y - 35), Color.Black);
            }
        }

        public override void AABBRecalculate()
        {
            foreach (var aabb in aabbs)
            {
                aabb.Pos = new Vector2(currentX, pos.Y);
            }
        }
    }

}
