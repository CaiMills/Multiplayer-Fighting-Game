using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using nkast.Aether.Physics2D.Dynamics;
using System.Timers;

namespace Multiplayer_Games_Programming_Framework.GameCode.Prefabs
{
    internal class Player2GO : PlayerGO
    {
        public Player2GO(Scene scene, Transform transform) : base(scene, transform)
        {
            SpriteRenderer sr = AddComponent(new SpriteRenderer(this, "playerBlackIdleFlipped"));
            sr.m_DepthLayer = 1;

            Rigidbody rb = AddComponent(new Rigidbody(this, BodyType.Dynamic, 1, sr.m_Size / 2));
            rb.m_Body.Tag = this;
            rb.m_Body.IgnoreGravity = true;
            rb.m_Body.FixedRotation = true;
            rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 0.0f, Vector2.Zero, Physics.GetCategoryByName("Player2"), 
                Physics.GetCategoryByName("Player1") | Physics.GetCategoryByName("WorldObject") | Physics.GetCategoryByName("Wall") | Physics.GetCategoryByName("Hitbox"));

            m_PlayerNumber = 2;
        }
    }
}
