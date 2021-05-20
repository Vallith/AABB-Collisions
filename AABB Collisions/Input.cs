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


        static KeyboardState current;
        static KeyboardState previous;

        static MouseState currentMouse;
        static MouseState previousMouse;

        static Input()
        {
            current = Keyboard.GetState();
            currentMouse = Mouse.GetState();
        }

        public static void Process()
        {
            previous = current;
            previousMouse = currentMouse;
            current = Keyboard.GetState();
            currentMouse = Mouse.GetState();
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
                    return currentMouse.LeftButton == ButtonState.Pressed;

                case MouseButton.RightButton:
                    return currentMouse.RightButton == ButtonState.Pressed;

                case MouseButton.MiddleButton:
                    return currentMouse.MiddleButton == ButtonState.Pressed;
                default:
                    return false;
            }
        }

        public static bool IsPressed(MouseButton mouseButton)
        {
            switch (mouseButton)
            {
                case MouseButton.LeftButton:
                    return currentMouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed;

                case MouseButton.RightButton:
                    return currentMouse.RightButton == ButtonState.Pressed && previousMouse.RightButton != ButtonState.Pressed;

                case MouseButton.MiddleButton:
                    return currentMouse.MiddleButton == ButtonState.Pressed && previousMouse.MiddleButton != ButtonState.Pressed;
                default:
                    return false;
            }
        }

    }
}
