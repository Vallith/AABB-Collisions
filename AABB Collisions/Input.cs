using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AABB_Collisions
{
    public enum MouseButton
    {
        LeftButton,
        RightButton,
        MiddleButton
    }
    public static class Input
    {


        public static KeyboardState current;
        public static KeyboardState previous;

        public static MouseState mouse;
        public static MouseState previousMouse;

        static Input()
        {
            current = Keyboard.GetState();
            mouse = Mouse.GetState();
        }

        public static void Process()
        {
            previous = current;
            previousMouse = mouse;
            current = Keyboard.GetState();
            mouse = Mouse.GetState();
        }

        public static bool IsHeld(Keys key)
        {
            return current.IsKeyDown(key);
        }

        public static bool IsPressed(Keys key)
        {
            return current.IsKeyDown(key) && !previous.IsKeyDown(key);
        }

        public static bool IsHeld(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftButton:
                    return mouse.LeftButton == ButtonState.Pressed;

                case MouseButton.RightButton:
                    return mouse.RightButton == ButtonState.Pressed;

                case MouseButton.MiddleButton:
                    return mouse.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public static bool IsPressed(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftButton:
                    return mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed;

                case MouseButton.RightButton:
                    return mouse.RightButton == ButtonState.Pressed && previousMouse.RightButton != ButtonState.Pressed;

                case MouseButton.MiddleButton:
                    return mouse.MiddleButton == ButtonState.Pressed && previousMouse.MiddleButton != ButtonState.Pressed;
                default:
                    return false;
            }
        }

    }
}
