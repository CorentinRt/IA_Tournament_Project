using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DoNotModify
{

	[CreateAssetMenu(fileName = "Data", menuName = "GameConfiguration/GameCollection", order = 1)]
	public class GameCollection : ScriptableObject
	{
		public List<BaseSpaceShipController> controllers = new List<BaseSpaceShipController>();
		public List<string> levelScenes = new List<string>();
	}

}