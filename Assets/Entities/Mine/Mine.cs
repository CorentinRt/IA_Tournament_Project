using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{
	public class MineView
    {
		public MineView(Mine mine) { _mine = mine; }
		Mine _mine;

		// Constants
		public float ActivationTime { get { return _mine.ActivationTime; } }
		public float ExplosionRadius { get { return _mine.ExplosionRadius; } }
		public float BulletHitRadius { get { return _mine.BulletHitRadius; } }

		// Variables
		public Vector2 Position { get { return _mine.Position; } }
		public bool IsActive { get { return _mine.IsActive; } }
	}

	public class Mine : MonoBehaviour
	{
		public MineView view;
        public float ActivationTime { get { return _activationTime; } }
        public float ExplosionRadius { get { return _explosionCollider.radius * Mathf.Abs(_explosionCollider.transform.lossyScale.x); } }
		public float BulletHitRadius { get { return _bulletCollider.radius * Mathf.Abs(_bulletCollider.transform.lossyScale.x); } }
		public Vector2 Position { get { return (Vector2)(transform.position); } }
		public bool IsActive { get { return _isActive; } }


		private const float _activationTime = 1.0f;
		private bool _isActive = false;
		private bool _isHitting = false;

		private CircleCollider2D _explosionCollider = null;
		private CircleCollider2D _bulletCollider = null;

		[SerializeField]
		private Material disabledMaterial = null;
		private List<Material> _defaultMaterialList = new List<Material>();
		private List<MeshRenderer> _meshRendererList = new List<MeshRenderer>();

		private const string ANIMATOR_ON_ACTIVATE = "OnActivate";
		private Animator _animator = null;

		[SerializeField]
		private AudioSource activationAudio;

		void Awake()
		{
			CircleCollider2D[] colliders = GetComponentsInChildren<CircleCollider2D>();
			foreach (CircleCollider2D collider in colliders)
			{
				if (collider.isTrigger) { _explosionCollider = collider; }
				else { _bulletCollider = collider; }
			}

			GetComponentsInChildren<MeshRenderer>(_meshRendererList);
			foreach (MeshRenderer meshRenderer in _meshRendererList)
			{
				_defaultMaterialList.Add(meshRenderer.sharedMaterial);
				if (disabledMaterial != null)
				{
					meshRenderer.material = disabledMaterial;
				}
			}

			_animator = GetComponentInChildren<Animator>();

			view = new MineView(this);
			GameManager.Instance.GetGameData().Mines.Add(view);
		}

		private void OnDestroy()
		{
			GameManager.Instance.GetGameData().Mines.Remove(view);
		}

		void Start()
		{
			StartCoroutine(ActivationCoroutine(_activationTime));

		}

		IEnumerator ActivationCoroutine(float activationTime)
		{
			yield return new WaitForSeconds(activationTime);
			_isActive = true;
			if (disabledMaterial != null)
			{
				int i = 0;
				foreach (MeshRenderer meshRenderer in _meshRendererList)
				{
					meshRenderer.material = _defaultMaterialList[i];
					++i;
				}
			}
			_animator.SetTrigger(ANIMATOR_ON_ACTIVATE);
			activationAudio.Play();
		}

		public bool IsHitting()
		{
			return _isHitting;
		}

		private void OnTriggerStay2D(Collider2D collision)
		{
			if (!_isActive)
				return;

			if (collision.tag == "Player")
			{
				SpaceShip spaceShip = collision.attachedRigidbody.GetComponent<SpaceShip>();
				_isHitting = true;
				spaceShip.OnHitMine(this);
				Destroy(gameObject);
			}
		}
	}

}