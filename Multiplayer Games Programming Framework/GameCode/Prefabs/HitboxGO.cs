using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using Multiplayer_Games_Programming_Packet_Library;
using nkast.Aether.Physics2D.Dynamics;
using System.Timers;

namespace Multiplayer_Games_Programming_Framework.GameCode.Prefabs
{
    internal class HitboxGO : GameObject
    {
        public HitboxGO(Scene scene, Transform transform) : base(scene, transform)
        {

        }
    }
}
