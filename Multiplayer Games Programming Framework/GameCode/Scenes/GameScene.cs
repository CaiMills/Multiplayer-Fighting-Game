using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using nkast.Aether.Physics2D.Dynamics;
using Multiplayer_Games_Programming_Framework.Core;
using System.Diagnostics;
using Multiplayer_Games_Programming_Framework.GameCode.Components;
using Microsoft.Xna.Framework.Graphics;
using Multiplayer_Games_Programming_Framework.Core.Utilities;
using Multiplayer_Games_Programming_Framework.GameCode;
using Multiplayer_Games_Programming_Framework.GameCode.Prefabs;
using Microsoft.VisualBasic;
using Multiplayer_Games_Programming_Packet_Library;
using Myra.MML;

namespace Multiplayer_Games_Programming_Framework
{
    internal class GameScene : Scene
	{
		Player1GO m_Player1;
		Player2GO m_RemotePlayer2;
		Player2GO m_Player2;
		Player1GO m_RemotePlayer1;

        readonly PlayerController m_PlayerController;

		Random m_Random = new Random();
		
		GameModeState m_GameModeState;

		float m_GameTimer;
		float m_CameraMovSpeed = 50;
		float m_CameraRotSpeed = 1;
		float m_CameraZoomSpeed = 1;

        public GameScene(SceneManager manager) : base(manager)
		{
			m_GameModeState = GameModeState.AWAKE;
		}

		public override void LoadContent()
		{
			base.LoadContent();

            //float screenWidth = Constants.m_ScreenWidth;
            //float screenHeight = Constants.m_ScreenHeight;

            //this only really works for upwards of two people total, if I want more playing simulatiously, change this
            if (NetworkManager.m_Instance.m_ID == 0)
            {
                m_Player1 = GameObject.Instantiate<Player1GO>(this, new Transform(new Vector2(-120, 7), new Vector2(6, 6), 0));
                m_Player1.AddComponent(new PlayerController(m_Player1));

                //screenWidth - 100, 500
                m_RemotePlayer2 = GameObject.Instantiate<Player2GO>(this, new Transform(new Vector2(120, 7), new Vector2(6, 6), 0));
                m_RemotePlayer2.AddComponent(new PlayerNetworkController(m_RemotePlayer2, 1));
            }
			else
			{
                m_Player2 = GameObject.Instantiate<Player2GO>(this, new Transform(new Vector2(120, 7), new Vector2(6, 6), 0));
                m_Player2.AddComponent(new PlayerController(m_Player2));

                //screenWidth - 100, 500
                m_RemotePlayer1 = GameObject.Instantiate<Player1GO>(this, new Transform(new Vector2(-120, 7), new Vector2(6, 6), 0));
                m_RemotePlayer1.AddComponent(new PlayerNetworkController(m_RemotePlayer1, 0));
            }

        //Border
        Vector2[] wallPos = new Vector2[]
			{
				new Vector2(0, -276), //top
				new Vector2(495, 0), //right
				new Vector2(0, 276), //bottom
				new Vector2(-495, 0) //left
			};

			Vector2[] wallScales = new Vector2[]
			{
				new Vector2(100, 1), //top
				new Vector2(1, 56.25f), //right
				new Vector2(100, 35), //bottom
				new Vector2(1, 56.25f) //left
			};

			for (int i = 0; i < 4; i++)
			{
				GameObject go = GameObject.Instantiate<GameObject>(this, new Transform(wallPos[i], wallScales[i], 0));
				SpriteRenderer sr = go.AddComponent(new SpriteRenderer(go, "Square(10x10)"));
				Rigidbody rb = go.AddComponent(new Rigidbody(go, BodyType.Static, 10, sr.m_Size / 2));
				rb.CreateRectangle(sr.m_Size.X, sr.m_Size.Y, 0.0f, 1.0f, Vector2.Zero, Physics.GetCategoryByName("Wall"), Physics.GetCategoryByName("All"));
				sr.m_Color = new Color(0,0.2f, 1);
				go.AddComponent(new ChangeColourOnCollision(go, Color.Red));
				m_GameObjects.Add(go);
            }
		}



        protected override string SceneName()
		{
			return "GameScene";
		}

		protected override World CreateWorld()
		{
			return new World(Physics.m_Gravity);
		}

        protected override Camera CreateCamera()
        {
			return new Camera(Vector2.Zero);
        }

        public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			//Update and move camera
			Vector2 camPos = m_Camera.m_Position;
			float camRot = m_Camera.m_Rotation;
			float camZoom = m_Camera.m_ZoomFactor;

            m_Camera.SetPosition(camPos);
			m_Camera.SetRotation(camRot);
			m_Camera.SetZoom(camZoom);

            //m_GameTimer += deltaTime;


			switch (m_GameModeState)
			{
				case GameModeState.AWAKE:
					m_GameModeState = GameModeState.STARTING;
					break;

				case GameModeState.STARTING:
					
					m_GameModeState = GameModeState.PLAYING;

					break;

				case GameModeState.PLAYING:

					/*if(m_GameTimer > 60)
					{
						m_Ball.Destroy();
						m_GameModeState = GameModeState.ENDING;
					}*/

					break;

				case GameModeState.ENDING:

					Debug.WriteLine("Game Over");
					break;
				default:
					break;
			}
		}
	}
}