using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using nkast.Aether.Physics2D.Collision;
using nkast.Aether.Physics2D.Dynamics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class StateManager : Component
    {
        Rigidbody m_Rigidbody;
        SpriteRenderer sr;

        public PlayerState m_State;

        Timer attackTimer;
        public bool cooldownActive = false;
        bool canAttack = true;

        HitboxManager m_HitboxManager;

        public StateManager(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void Start(float deltaTime)
        {
            sr = m_GameObject.GetComponent<SpriteRenderer>();
            m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
            m_State = PlayerState.IDLE;
            m_HitboxManager = m_GameObject.GetComponent<HitboxManager>();
        }

        protected override void Update(float deltaTime)
        {
            if (!canAttack && !cooldownActive)
            {
                cooldownActive = true;
                Timer(800, "Cooldown");
            }
        }

        public void UpdateState(PlayerState state)
        {
            m_State = state;

            switch (m_State)
            {
                case PlayerState.IDLE:
                    break;
                case PlayerState.ATTACKING:
                    if (canAttack)
                    {
                        AttackingCoroutine();
                    }
                    break;
            }
        }

        private void AttackingCoroutine()
        {
            m_GameObject.RemoveComponent(sr);

            if (m_GameObject.m_PlayerNumber == 1)
            {
                sr = m_GameObject.AddComponent(new SpriteRenderer(m_GameObject, "playerBlackPunch"));
            }
            if (m_GameObject.m_PlayerNumber == 2)
            {
                sr = m_GameObject.AddComponent(new SpriteRenderer(m_GameObject, "playerBlackPunchFlipped"));
            }

            m_Rigidbody.m_Body.LinearVelocity = m_Transform.Right * 0;
            m_HitboxManager.CreateHitbox();
            Timer(500, "Attack");
        }

        private void Timer(int seconds, string timerType)
        {
            attackTimer = new Timer();
            attackTimer.AutoReset = true;

            switch (timerType)
            {
                case "Attack":
                    attackTimer.Elapsed += new ElapsedEventHandler(OnAttackEnd);
                    break;
                case "Cooldown":
                    attackTimer.Elapsed += new ElapsedEventHandler(OnCooldownEnd);
                    break;
            }

            attackTimer.Interval = seconds; // ~ seconds
            attackTimer.Enabled = true;
        }

        private void OnAttackEnd(object source, ElapsedEventArgs e)
        {
            attackTimer.Dispose();
            m_HitboxManager.DeleteHitbox();

            m_GameObject.RemoveComponent(sr);

            if (m_GameObject.m_PlayerNumber == 1)
            {
                sr = m_GameObject.AddComponent(new SpriteRenderer(m_GameObject, "playerBlackIdle"));
            }
            if (m_GameObject.m_PlayerNumber == 2)
            {
                sr = m_GameObject.AddComponent(new SpriteRenderer(m_GameObject, "playerBlackIdleFlipped"));
            }

            canAttack = false;

            UpdateState(PlayerState.IDLE); //For current client
            NetworkManager.m_Instance.TCPSendMessage(new StatePacket(NetworkManager.m_Instance.m_ID, PlayerState.IDLE));
        }

        private void OnCooldownEnd(object source, ElapsedEventArgs e)
        {
            attackTimer.Dispose();
            canAttack = true;
            cooldownActive = false;
        }
    }
}
