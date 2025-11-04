using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DoNotModify;

namespace Echo
{
	public class EchoController : BaseSpaceShipController
	{
		public InputData InputData { get; set; }
		
		public override void Initialize(SpaceShipView spaceship, GameData data)
		{
		}

		public override InputData UpdateInput(SpaceShipView spaceship, GameData data)
		{
			return InputData;
		}
	}

}
