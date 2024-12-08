using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using nkast.Aether.Physics2D.Dynamics;
using System.Runtime.InteropServices;
using System.Timers;

namespace Multiplayer_Games_Programming_Framework
{
    internal class PlayerGO : GameObject
	{
        public PlayerGO(Scene scene, Transform transform) : base(scene, transform)
		{
            StateManager stateManager = AddComponent(new StateManager(this));

            HealthManager healthManager = AddComponent(new HealthManager(this, 100));

            HitboxManager hitboxManager = AddComponent(new HitboxManager(this));
        }
    }
}