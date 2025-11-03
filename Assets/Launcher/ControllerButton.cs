using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoNotModify
{

	public class ControllerButton : MonoBehaviour
	{
		public int playerId = 0;
		[System.NonSerialized]
		public BaseSpaceShipController controller;
		private Launcher launcher;

		void Start()
		{
			launcher = GetComponentInParent<Launcher>();
			GetComponentInChildren<Text>().text = controller.name;
		}

		public void OnPress()
		{
			launcher.SetController(controller, playerId);
		}
	}

}