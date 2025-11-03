using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify {


	public class KeyboardController : BaseSpaceShipController
	{
		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			float thrust = (Input.GetAxis("KbVertical") > 0.0f) ? 1.0f : 0.0f;
			float targetOrient = spaceship.Orientation;
			float direction = Input.GetAxis("KbHorizontal");
			if(direction != 0.0f)
			{
				targetOrient -= Mathf.Sign(direction) * 90;
			}
			bool shoot = Input.GetButtonDown("KbFire1");
			bool dropMine = Input.GetButtonDown("KbFire2");
			bool fireShockwave = Input.GetButtonDown("KbFire3");

			return new InputData(thrust, targetOrient, shoot, dropMine, fireShockwave);
		}

	}


}
