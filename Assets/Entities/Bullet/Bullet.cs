using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{
	public class BulletView
    {
		public BulletView(Bullet bullet) { _bullet = bullet; }

		// Constants
        public static float Speed { get { return Bullet.Speed; } }

		// Variables
        public Vector2 Position { get { return _bullet.Position; } }
		public Vector2 Velocity { get { return _bullet.Velocity; } }
    
		Bullet _bullet;
    }

    public class Bullet : MonoBehaviour
	{
		public BulletView view;
		public Vector2 Position { get { return (Vector2)(transform.position); } }
		public Vector2 Velocity { get { return (Vector2)(_rigidbody.linearVelocity); } }
		public static float Speed { get { return _bulletSpeed; } }

		private const float _bulletSpeed = 5.0f;
		private bool _isHitting = false;
		private int _owner = -1;
		private Rigidbody2D _rigidbody = null;

		public void Awake()
		{
			_rigidbody = GetComponent<Rigidbody2D>();
			Vector2 direction = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));
			_rigidbody.linearVelocity = direction * _bulletSpeed;

			view = new BulletView(this);
			GameManager.Instance.GetGameData().Bullets.Add(view);
		}

		private void OnDestroy()
		{
			GameManager.Instance.GetGameData().Bullets.Remove(view);
		}

		public void SetOwner(int owner)
		{
			if (_owner != -1)
				return;
			_owner = owner;
		}

		public bool IsHitting()
		{
			return _isHitting;
		}

		// Update is called once per frame
		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (collision.tag == "Player")
			{
				SpaceShip spaceShip = collision.attachedRigidbody.GetComponent<SpaceShip>();
				if (_owner != spaceShip.Owner)
				{
					_isHitting = true;
					spaceShip.OnHitBullet(this);
					Destroy(gameObject);
				}
			}
			else
			{
				if (!collision.isTrigger)
				{
					if (collision.tag == "Mine")
					{
						Mine mine = collision.attachedRigidbody.GetComponent<Mine>();
						Destroy(mine.gameObject);
					}
					Destroy(gameObject);
				}
			}
		}
	}

}