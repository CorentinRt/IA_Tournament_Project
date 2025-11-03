using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	public struct InputData
	{
		public float thrust;
		public float targetOrientation;
		public bool shoot;
		public bool dropMine;
		public bool fireShockwave;
		public InputData(float thrust, float targetOrient, bool shoot, bool dropMine, bool fireShockwave)
		{
			this.thrust = thrust; this.targetOrientation = targetOrient; this.shoot = shoot; this.dropMine = dropMine; this.fireShockwave = fireShockwave;
		}
	}


	public abstract class BaseSpaceShipController : MonoBehaviour
	{
		public virtual void Initialize(SpaceShipView spaceship, GameData data)
		{

		}

		public virtual InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			return new InputData(0.0f, 0.0f, false, false, false);
		}
	}

}