using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	public class GameConfiguration
	{
		public static GameConfiguration Instance;

		public BaseSpaceShipController controller1;
		public BaseSpaceShipController controller2;
		public string levelName;

		public GameConfiguration()
        {
			Instance = this;
		}
	}

}