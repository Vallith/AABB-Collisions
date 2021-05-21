using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace AABB_Collisions
{
    static class ScreenTools
    {
    }

    public class Slider : ScreenTool
    {
        private float sliderLength;
        private float currentX;

        public float minValue;
        public float maxValue;
        public float step;

        public float dragRadius;
        private float value;
        float Value { get => value; }

        private Vector2 dragPos => new Vector2(currentX, pos.Y);

        bool dragging = false;

        public delegate void SliderChanged(Slider slider, float value);

        public SliderChanged onSliderChanged;

        public Slider(Vector2 pos, float radius, float length, float min, float max)
        {
            this.pos = pos;
            currentX = pos.X;
            this.sliderLength = length;
            this.minValue = min;
            this.maxValue = max;
            this.dragRadius = radius;
        }

        public override void InputControl() {
            if (Input.IsHeld(MouseButton.LeftButton))
            {
                Vector2 mouse = Input.mouse.Position.ToVector2();
                if (dragging || AABB.InsideCircle(mouse, dragPos, dragRadius))
                {
                    dragging = true;
                    currentX = Math.Clamp(mouse.X, pos.X, pos.X + sliderLength);
                    value = Util.Remap(currentX, pos.X, pos.X + sliderLength, minValue, maxValue);
                    onSliderChanged(this, value);
                }
            }
            else
            {
                dragging = false;
            }
        }

        public override void DrawGUI()
        {
            Screen.Draw.DrawLine(this.pos, new Vector2(pos.X + sliderLength, pos.Y),Color.White);
            Screen.Draw.DrawCircle(new Vector2(currentX, pos.Y), dragRadius, 20, Color.White);
        }

        public override void AABBRecalculate()
        {
            
        }
    }

    public abstract class ScreenTool
    {
        protected Vector2 pos;
        protected Vector2 Pos { get => pos; }

        protected AABB aabb;


        public abstract void DrawGUI();
        public abstract void InputControl();
        public abstract void AABBRecalculate();
    }
}
