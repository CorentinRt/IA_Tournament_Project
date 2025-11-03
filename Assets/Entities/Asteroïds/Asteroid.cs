using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{
	public class AsteroidView
    {
		public AsteroidView (Asteroid asteroid) { _asteroid = asteroid; }

		Asteroid _asteroid;

        // Constants
        public float Radius { get { return _asteroid.Radius; } }

        // Variables
        public Vector2 Position { get { return _asteroid.Position; } }
	}

	public class Asteroid : MonoBehaviour
	{
		public AsteroidView view;

		public Vector2 Position { get { return (Vector2)(transform.position); } }
		public float Radius { get { return _collider.radius * Mathf.Abs(_collider.transform.lossyScale.x); } }

		private CircleCollider2D _collider = null;

		void Awake()
		{
			_collider = GetComponentInChildren<CircleCollider2D>();

			view = new AsteroidView(this);
			GameManager.Instance.GetGameData().Asteroids.Add(view);
		}

		private void OnDestroy()
		{
			GameManager.Instance.GetGameData().Asteroids.Remove(view);
		}
	}

}