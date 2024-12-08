using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using Myra.MML;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Vector2 = System.Numerics.Vector2;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class PlayerController : Component
    {
        float m_MovementSpeed;

        StateManager m_StateManager;
        HitboxManager m_HitboxManager;
        Rigidbody m_Rigidbody;
        SpriteRenderer sr;

        public PlayerState m_State;

        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            m_MovementSpeed = 10f;
        }

        protected override void Start(float deltaTime)
        {
            sr = m_GameObject.GetComponent<SpriteRenderer>();
            m_StateManager = m_GameObject.GetComponent<StateManager>();
            m_HitboxManager = m_GameObject.GetComponent<HitboxManager>();
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
        }

        protected override void Update(float deltaTime)
        {
            m_State = m_StateManager.m_State;
            Vector2 input = Vector2.Zero;

            if (m_State != PlayerState.ATTACKING)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    input.X = -1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    input.X = 1;
                }
                m_Rigidbody.m_Body.LinearVelocity = m_Transform.Right * input.X * m_MovementSpeed;

                NetworkManager.m_Instance.TCPSendMessage(new PositionPacket(NetworkManager.m_Instance.m_ID, new Vector2(m_Transform.Position.X, m_Transform.Position.Y)));

                if (Keyboard.GetState().IsKeyDown(Keys.F) && !m_StateManager.cooldownActive)
                {
                    //State Packet
                    m_State = PlayerState.ATTACKING; //this is for sending the packet to the other client
                    m_StateManager.UpdateState(PlayerState.ATTACKING); //whilst this is for setting the correct state for this client
                    m_HitboxManager.UpdateHitboxPosition(new Vector2(m_Transform.Position.X, m_Transform.Position.Y));
                    NetworkManager.m_Instance.TCPSendMessage(new StatePacket(NetworkManager.m_Instance.m_ID, m_State));
                }
            }
        }
    }
}

