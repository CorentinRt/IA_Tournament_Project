using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoNotModify
{

	public class LevelButton : MonoBehaviour
	{
		private Launcher launcher;
		[System.NonSerialized]
		public string levelName;

		void Start()
		{
			launcher = GetComponentInParent<Launcher>();
			GetComponentInChildren<Text>().text = levelName;
		}

		public void OnPress()
		{
			launcher.SetLevel(levelName);
		}
	}

}