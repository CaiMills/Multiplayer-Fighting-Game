using Microsoft.Xna.Framework;
using nkast.Aether.Physics2D.Dynamics;
using System.Diagnostics;

namespace Multiplayer_Games_Programming_Framework.Core.Utilities
{
    internal static class Physics
    {
        public static Vector2 m_Gravity = new Vector2(0, 9.81f);

        public static float m_PhysicsWidth = ScreenToPhysics(Graphics.GraphicsDevice.Viewport.Width);
        public static float m_PhysicsHeight = ScreenToPhysics(Graphics.GraphicsDevice.Viewport.Height);

        public static float ScreenToPhysics(float value)
        {
            return value * 0.02f;
        }

        public static Vector2 ScreenToPhysics(Vector2 value)
        {
            return new Vector2(ScreenToPhysics(value.X), ScreenToPhysics(value.Y));
        }

        public static float PhysicstoScreen(float value)
        {
            return value * 50.0f;
        }

        public static Vector2 PhysicstoScreen(Vector2 value)
        {
            return new Vector2(PhysicstoScreen(value.X), PhysicstoScreen(value.Y));
        }

        public static Category GetCategoryByName(string name)
        {
            switch (name)
            {
                case "All":
                    return Category.All;
                case "None":
                    return Category.None;
                case "Player1":
                    return Category.Cat1;
                case "Player2":
                    return Category.Cat2;
                case "WorldObject":
                    return Category.Cat3;
                case "Wall":
                    return Category.Cat4;
                case "Hitbox":
                    return Category.Cat5;
            }

            Debug.WriteLine("Category not found: " + name + ". Returning Category.None");
            return Category.None;
        }
    }
}
