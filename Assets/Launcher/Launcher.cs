using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DoNotModify
{

	public class Launcher : MonoBehaviour
	{
		public Text redPlayerText;
		public Text bluePlayerText;
		public Text levelText;

		public ControllerButton blueButton;
		public ControllerButton redButton;
		public LevelButton levelButton;
		public RectTransform bluePanel;
		public RectTransform redPanel;
		public RectTransform levelPanel;

		public GameCollection collection;
		public GameConfiguration gameConfiguration;

		void Start()
		{
			if(GameConfiguration.Instance != null) {
				gameConfiguration = GameConfiguration.Instance;				
			} else {
				gameConfiguration = new GameConfiguration();
            }

			foreach (BaseSpaceShipController controller in collection.controllers)
			{
				ControllerButton newBlueButton = Instantiate<ControllerButton>(blueButton);
				newBlueButton.controller = controller;
				newBlueButton.transform.SetParent(bluePanel);

				ControllerButton newRedButton = Instantiate<ControllerButton>(redButton);
				newRedButton.controller = controller;
				newRedButton.transform.SetParent(redPanel);
			}

			foreach (string levelScene in collection.levelScenes)
			{
				LevelButton newLevelButton = Instantiate<LevelButton>(levelButton);
				newLevelButton.levelName = levelScene;
				newLevelButton.transform.SetParent(levelPanel);
			}

			if (collection.controllers.Count >= 1)
			{
				SetController(collection.controllers[0], 0);
				SetController(collection.controllers[0], 1);
			}
			if (collection.controllers.Count >= 2)
			{
				SetController(collection.controllers[1], 1);
			}
			if (collection.levelScenes.Count >= 1)
			{
				SetLevel(collection.levelScenes[0]);
			}
		}

		public void SetController(BaseSpaceShipController controller, int playerId)
		{
			if (playerId == 0)
			{
				gameConfiguration.controller1 = controller;
				redPlayerText.text = controller.name;
			}
			else
			{
				gameConfiguration.controller2 = controller;
				bluePlayerText.text = controller.name;
			}
		}

		public void SetLevel(string levelName)
		{
			gameConfiguration.levelName = levelName;
			levelText.text = levelName;
		}

		public void Launch()
		{
			SceneManager.LoadScene(gameConfiguration.levelName);
		}
	}

}