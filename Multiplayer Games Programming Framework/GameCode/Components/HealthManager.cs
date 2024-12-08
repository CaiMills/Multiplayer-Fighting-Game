using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Packet_Library;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class HealthManager : Component
    {
        int m_MaxHealth;
        int m_CurrentHealth;

        StateManager m_StateManager;

        public HealthManager(GameObject gameObject, int startingHealth) : base(gameObject)
        {
            m_MaxHealth = startingHealth;
        }

        protected override void Start(float deltaTime)
        {
            m_CurrentHealth = m_MaxHealth;
            m_StateManager = m_GameObject.GetComponent<StateManager>();
        }

        protected override void Update(float deltaTime)
        {
            if (m_CurrentHealth <= 0)
            {
                m_StateManager.UpdateState(PlayerState.DEAD);
            }
        }

        public void TakingDamage(GameObject gameObject, int damage)
        {
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.m_Color = new Color(255, 0, 0);

            m_CurrentHealth =- damage;
        }
    }
}
