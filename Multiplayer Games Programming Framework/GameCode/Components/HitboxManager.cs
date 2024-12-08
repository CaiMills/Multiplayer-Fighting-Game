using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Multiplayer_Games_Programming_Packet_Library;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
    internal class HitboxManager : Component
    {
        HitboxGO m_Hitbox;
        HitboxGO m_Hitbox2;
        Rigidbody m_Hitbox1Rigidbody;
        Rigidbody m_Hitbox2Rigidbody;
        Vector2 m_BufferedHitboxPositionUpdate;
        bool m_BufferedHitboxPositionIsDirty = false;

        public HitboxManager(GameObject gameObject) : base(gameObject)
        {
        }

        protected override void Update(float deltaTime)
        {
            if (m_BufferedHitboxPositionIsDirty)
            {
                PushBufferedHitboxPositionUpdate();
            }
        }

            public void UpdateHitboxPosition(Vector2 pos)
        {
            m_BufferedHitboxPositionUpdate = pos;
            m_BufferedHitboxPositionIsDirty = true;
        }

        private void PushBufferedHitboxPositionUpdate()
        {
            if (m_Hitbox1Rigidbody != null)
            {
                m_Hitbox1Rigidbody.m_Body.Position = new Vector2(Physics.ScreenToPhysics(m_BufferedHitboxPositionUpdate.X), Physics.ScreenToPhysics(m_BufferedHitboxPositionUpdate.Y));
            }
            else if (m_Hitbox2Rigidbody != null)
            {
                m_Hitbox2Rigidbody.m_Body.Position = new Vector2(Physics.ScreenToPhysics(m_BufferedHitboxPositionUpdate.X), Physics.ScreenToPhysics(m_BufferedHitboxPositionUpdate.Y));
            }

            m_BufferedHitboxPositionUpdate = Vector2.Zero;
            m_BufferedHitboxPositionIsDirty = false;
        }

        public void CreateHitbox()
        {
            if (m_GameObject.m_PlayerNumber == 1)
            {
                m_Hitbox = GameObject.Instantiate<HitboxGO>(m_GameObject.m_Scene, new Transform(m_BufferedHitboxPositionUpdate, new Vector2(14, 3), 0));
                SpriteRenderer sr = m_Hitbox.AddComponent(new SpriteRenderer(m_Hitbox, "Square(10x10)"));
                Rigidbody rb = m_Hitbox.AddComponent(new Rigidbody(m_Hitbox, BodyType.Static, 10, sr.m_Size / 2));
                rb.CreateHitbox(sr.m_Size.X, sr.m_Size.Y, 0.0f, 1.0f, Vector2.Zero, Physics.GetCategoryByName("Hitbox"), Physics.GetCategoryByName("Player2"));
                m_Hitbox1Rigidbody = rb;

                sr.m_Color = new Color(255, 255, 255);
                m_Hitbox.AddComponent(new ChangeColourOnCollision(m_Hitbox, Color.Red));
                m_GameObject.m_Scene.m_GameObjects.Add(m_Hitbox);
            }
            if (m_GameObject.m_PlayerNumber == 2)
            {
                m_Hitbox2 = GameObject.Instantiate<HitboxGO>(m_GameObject.m_Scene, new Transform(m_BufferedHitboxPositionUpdate, new Vector2(14, 3), 0));
                SpriteRenderer sr = m_Hitbox2.AddComponent(new SpriteRenderer(m_Hitbox2, "Square(10x10)"));
                Rigidbody rb = m_Hitbox2.AddComponent(new Rigidbody(m_Hitbox2, BodyType.Static, 10, sr.m_Size / 2));
                rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 1.0f, Vector2.Zero, Physics.GetCategoryByName("Hitbox"), Physics.GetCategoryByName("Player1"));
                m_Hitbox2Rigidbody = rb;

                sr.m_Color = new Color(255, 255, 255);
                m_Hitbox2.AddComponent(new ChangeColourOnCollision(m_Hitbox2, Color.Red));
                m_GameObject.m_Scene.m_GameObjects.Add(m_Hitbox2);
            }
        }

        public void DeleteHitbox()
        {
            if (m_Hitbox != null)
            {
                m_Hitbox.Destroy();
            }
            if (m_Hitbox2 != null)
            {
                m_Hitbox2.Destroy();
            }
        }

        protected override void OnCollisionEnter(Fixture sender, Fixture other, Contact contact)
        {
            //Doesnt Work Currently

            //GameObject collidedObject = (GameObject)other.Tag;

            // Check if the collided object is a player 1
            if (other.Tag is Player1GO player1)
            {
                // Access the player's HealthManager
                HealthManager healthManager = player1.GetComponent<HealthManager>();

                if (healthManager != null)
                {
                    // Call the TakingDamage method
                    healthManager.TakingDamage(player1, 20);

                    Packet packet = new MessagePacket("Player hit!");
                    NetworkManager.m_Instance.TCPSendMessage(packet);
                }
            }
            // Check if the collided object is a player 2
            if (other.Tag is Player1GO player2)
            {
                // Access the player's HealthManager
                HealthManager healthManager = player2.GetComponent<HealthManager>();

                if (healthManager != null)
                {
                    // Call the TakingDamage method
                    healthManager.TakingDamage(player2, 20);

                    Packet packet = new MessagePacket("Player hit!");
                    NetworkManager.m_Instance.TCPSendMessage(packet);
                }
            }

            DeleteHitbox();
        }
    }
}
