using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	public class StateMachine : MonoBehaviour
	{
		public State CurrentState { get; set; } = State.ChaseWayPoint;
		public SpaceShip SpaceShip { get; set; } = null;
		public GameData Data { get; set; } = null;
		public float ManaDropMine = 0.5f;
		public StateMachine Instance;

		//public StateMachine(SpaceShip spaceShip, GameData data)
		//{

		//}

		private void Awake()
		{
			Instance = this;
		}

		private void Update()
		{
			switch (CurrentState)
			{
				case State.IsHit:
					break;
				case State.HasShot:
					break;
				case State.WaypointCaptured:
					if (SpaceShip.Energy > SpaceShip.MineEnergyCost && SpaceShip.Energy > ManaDropMine)
					{
						SpaceShip.DropMine();
					}
					break;
				case State.ChaseShip:
					break;
				case State.ChaseWayPoint:
					break;
			}
		}

		void OnCapturedWayPoint(WayPoint wayPointTargeted)
		{
			if (wayPointTargeted?.Owner == SpaceShip.Owner)
			{
				CurrentState = State.WaypointCaptured;
			}
		}

		void OnHitSpaceShip()
		{
			CurrentState = SpaceShip.IsHit() ? State.IsHit : CurrentState;
		}
	}

}