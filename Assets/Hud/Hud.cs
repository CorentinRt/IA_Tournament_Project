using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DoNotModify
{

	public class Hud : MonoBehaviour
	{
		const string GAMEOVER_SUFFIX_TEXT = " wins!";

		public Text gameOver;
		public Text countdown;

		public Slider slider1;
		public Slider slider2;
		public Text score1;
		public Text score2;
		public Text scoreBreakdown1;
		public Text scoreBreakdown2;
		public Text playerName1;
		public Text playerName2;

		private int _lastCountDownValue = int.MaxValue;
		private AudioSource _timerAudio;

		void Awake()
		{
			_timerAudio = GetComponent<AudioSource>();

			gameOver.gameObject.SetActive(false);
			string playerName1 = GameManager.Instance.GetPlayerName(0);
			string playerName2 = GameManager.Instance.GetPlayerName(1);
			SetPlayerNames(playerName1, playerName2);
		}

		void SetPlayerNames(string name1, string name2)
		{
			playerName1.text = name1;
			playerName2.text = name2;
		}

		// Update is called once per frame
		void Update()
		{
			GameData gameData = GameManager.Instance.GetGameData();
			score1.text = "" + GameManager.Instance.GetScoreForPlayer(0);
			score2.text = "" + GameManager.Instance.GetScoreForPlayer(1);
			scoreBreakdown1.text = "" + GameManager.Instance.GetWayPointScoreForPlayer(0) + " - " + GameManager.Instance.GetHitScoreForPlayer(0);
			scoreBreakdown2.text = "" + GameManager.Instance.GetWayPointScoreForPlayer(1) + " - " + GameManager.Instance.GetHitScoreForPlayer(1);

			slider1.value = gameData.GetSpaceShipForOwner(0).Energy;
			slider2.value = gameData.GetSpaceShipForOwner(1).Energy;

			int countdownValue = (int)gameData.timeLeft;
			if (countdownValue <= 5 && _lastCountDownValue != countdownValue) {
				_timerAudio.Play();
			}
			_lastCountDownValue = countdownValue;

			countdown.text = countdownValue.ToString();
			if (!gameOver.gameObject.activeSelf && GameManager.Instance.IsGameFinished())
			{
				int winner = GameManager.Instance.GetScoreForPlayer(0) > GameManager.Instance.GetScoreForPlayer(1) ? 0 : 1;
				gameOver.color = winner == 0 ? playerName1.color : playerName2.color;
				gameOver.text = winner == 0 ? GameManager.Instance.GetPlayerName(0) : GameManager.Instance.GetPlayerName(1);
				gameOver.text += GAMEOVER_SUFFIX_TEXT;
				gameOver.gameObject.SetActive(true);
			}
		}
	}

}