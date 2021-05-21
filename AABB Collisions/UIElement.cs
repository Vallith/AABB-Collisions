using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
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

        public abstract void DrawGUI();
        public abstract void InputControl();
        public abstract void AABBRecalculate();
    }

    public class Slider : UIElement
    {
        private float sliderLength;
        private float currentX;

        public float minValue;
        public float maxValue;
        public float step;

        public float dragRadius;
        private float value;
        public float Value { get => value; }

        private Vector2 dragPos => new Vector2(currentX, pos.Y);

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
            aabbs.Add(AABB.CreateCircleAABB(new Vector2(currentX, pos.Y), dragRadius));
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
            Screen.Draw.DrawLine(this.pos, new Vector2(pos.X + sliderLength, pos.Y),Color.White);
            Screen.Draw.DrawCircle(new Vector2(currentX, pos.Y), dragRadius, 20, Color.White);
            if (drawValue == true)
            {
                Screen.Draw.DrawString(Game1.instance.defaultFont, value.ToString(), new Vector2(pos.X, pos.Y - 35), Color.Black);
            }
        }

        public override void AABBRecalculate()
        {
            foreach (var aabb in aabbs)
            {
                aabb.min = new Vector2(currentX - dragRadius, pos.Y - dragRadius);
                aabb.max = new Vector2(currentX + dragRadius, pos.Y + dragRadius);
            }
        }
    }

}
