using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	public class Shockwave : MonoBehaviour
	{
		private int _owner = -1;
		private List<GameObject> _hitList = new List<GameObject>();
		private const float _power = 4.0f;
		private Animator _animator = null;

		public void Awake()
		{
			_animator = GetComponentInChildren<Animator>();
		}

		public void Update()
		{
			if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
			{
				Destroy(gameObject);
			}
		}

		public void SetOwner(int owner)
		{
			if (_owner != -1)
				return;
			_owner = owner;
		}

		private void OnTriggerEnter2D(Collider2D collision)
		{
			if (_hitList.Contains(collision.attachedRigidbody.gameObject))
				return;

			bool isApplied = false;
			if (collision.tag == "Player")
			{
				SpaceShip spaceShip = collision.attachedRigidbody.GetComponent<SpaceShip>();
				if (_owner != spaceShip.Owner)
				{
					spaceShip.OnApplyShockwave(this);
					isApplied = true;
				}
			}
			if (collision.tag == "Mine" || collision.tag == "Bullet")
			{
				Destroy(collision.attachedRigidbody.gameObject);
			}
			if (isApplied)
			{
				Vector2 direction = (collision.attachedRigidbody.transform.position - transform.position);
				direction.Normalize();
				collision.attachedRigidbody.linearVelocity = direction * _power;
			}
		}
	}

}