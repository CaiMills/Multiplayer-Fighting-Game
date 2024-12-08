using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class PlayerNetworkController : Component
    {
        StateManager m_StateManager;
        HitboxManager m_HitboxManager;
        Rigidbody m_Rigidbody;
        SpriteRenderer sr;

        int m_Index;

        bool m_PosIsDirty = false;
        Vector2 m_NewPosition;

        bool m_StateIsDirty = false;
        PlayerState m_NewState;

        public PlayerNetworkController(GameObject gameObject, int index) : base(gameObject)
        {
            m_Index = index;
        }

        protected override void Start(float deltaTime)
        {
            m_StateManager = m_GameObject.GetComponent<StateManager>();
            m_HitboxManager = m_GameObject.GetComponent<HitboxManager>();
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
            sr = m_GameObject.GetComponent<SpriteRenderer>();
            NetworkManager.m_Instance.m_PlayerPositions.Add(m_Index, UpdatePosition);
            NetworkManager.m_Instance.m_PlayerStates.Add(m_Index, UpdateState);
        }

        public override void Destroy()
        {
            NetworkManager.m_Instance.m_PlayerPositions.Remove(m_Index);
            NetworkManager.m_Instance.m_PlayerStates.Remove(m_Index);
            base.Destroy();
        }

        protected override void Update(float deltaTime)
        {
            if(m_PosIsDirty)
            {
                m_Rigidbody.UpdatePosition(m_NewPosition);
                m_HitboxManager.UpdateHitboxPosition(m_NewPosition);
                m_PosIsDirty = false;
            }
            if (m_StateIsDirty)
            {
                m_StateManager.UpdateState(m_NewState);
                m_StateIsDirty = false;
            }
        }

        private void UpdatePosition(Vector2 position)
        {
            m_NewPosition = position;
            m_PosIsDirty = true;
        }

        private void UpdateState(PlayerState state)
        {
            m_NewState = state;
            m_StateIsDirty = true;
        }
    }
}


