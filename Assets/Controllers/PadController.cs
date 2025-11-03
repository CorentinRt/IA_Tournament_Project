using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify {


	public class PadController : BaseSpaceShipController
	{
		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			Vector2 axis = new Vector2(Input.GetAxis("PadHorizontal"), Input.GetAxis("PadVertical"));
			float targetOrient = spaceship.Orientation;
			float thrust = 0.0f;
			if (axis.SqrMagnitude() > 0)
			{
				targetOrient = Vector2.SignedAngle(Vector2.right, axis);
				thrust = axis.magnitude;
			}			
			bool shoot = Input.GetButtonDown("PadFire1");
			bool dropMine = Input.GetButtonDown("PadFire2");
			bool fireShockwave = Input.GetButtonDown("PadFire3");

			return new InputData(thrust, targetOrient, shoot, dropMine, fireShockwave);
		}

	}


}
