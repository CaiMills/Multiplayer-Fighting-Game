﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;
using nkast.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core.Utilities;

namespace Multiplayer_Games_Programming_Framework
{
    internal abstract class Scene
    {
        event Action<float> onStart;
		event Action<float> onDraw;
        event Action<float> onUpdate;
		event Action<float> onLateUpdate;
        event Action onDirtyGameObject;
        public List<GameObject> m_GameObjects;

        protected SceneManager m_Manager;

        public Matrix world;

        public string m_Name { get; private set; }

        public World m_World { get; protected set; }

        public Camera m_Camera { get; protected set; }

        public Scene(SceneManager manager)
        {
            m_Manager = manager;

            m_GameObjects = new List<GameObject>();
            m_Name = SceneName();
            m_World = CreateWorld();
            m_Camera = CreateCamera();
        }

		~Scene()
        {
            m_GameObjects.Clear();
            onStart = null;
            onUpdate = null;
            onDraw = null;
            onLateUpdate = null;
            if (m_World != null)
            {
                m_World.Clear();
                m_World = null;
            }
		}

		protected abstract string SceneName();
        protected abstract World CreateWorld();

        protected abstract Camera CreateCamera();

        public virtual void LoadContent()
        {
		}

        public virtual void Update(float deltaTime)
        {
            onDirtyGameObject?.Invoke();
            onDirtyGameObject = null;

            onStart?.Invoke(deltaTime);
            onStart = null;

            if(m_World != null)
                m_World.Step(deltaTime);

            onUpdate?.Invoke(deltaTime);
            onLateUpdate?.Invoke(deltaTime);

            m_Camera.Update();
        }

        public virtual void Draw(float deltaTime)
        {
            onDraw?.Invoke(deltaTime);
        }

        public void AddGameObject(GameObject gameObject)
        {
            m_GameObjects.Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            m_GameObjects.Remove(gameObject);
        }

        public SpriteBatch GetSpriteBatch()
        {
            return m_Manager.m_SpriteBatch;
        }

        public ContentManager GetContentManager()
        {
            return m_Manager.m_ContentManager;
        }

        public void RegisterGameLoopCall(Action<float> method)
        {
            switch (method.Method.Name)
            {
                case "Start":
                    onStart += method;
                    break;
                case "Draw":
                    onDraw += method;
                    break;
                case "Update":
                    onUpdate += method;
                    break;
                case "LateUpdate":
                    onLateUpdate += method;
                    break;
                default:
                    Debug.Fail("Invalid funcation call");
                    break;
            }
        }

        public void DeregisterGameLoopCall(Action<float> method)
        {
            switch (method.Method.Name)
            {
                case "Start":
					onStart -= method;
					break;
                case "Draw":
                    onDraw -= method;
                    break;
                case "Update":
                    onUpdate -= method;
                    break;
                case "LateUpdate":
                    onLateUpdate -= method;
                    break;
                default:
                    Debug.Fail("Invalid funcation call");
                    break;
            }
        }

        public void RegisterCheckDirtyGameObjectCall(Action method)
        {
            onDirtyGameObject += method;
        }

        public void DeregisterCheckDirtyGameObjectCall(Action method)
        {
            onDirtyGameObject -= method;
        }
	}
}