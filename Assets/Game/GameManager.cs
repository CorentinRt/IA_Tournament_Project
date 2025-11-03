using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoNotModify
{

	public class GameData
	{
		public List<SpaceShipView> SpaceShips = new List<SpaceShipView>();
		public List<WayPointView> WayPoints = new List<WayPointView>();
		public List<AsteroidView> Asteroids = new List<AsteroidView>();
		public List<MineView> Mines = new List<MineView>();
		public List<BulletView> Bullets = new List<BulletView>();

		public float timeLeft = 0.0f;

		public SpaceShipView GetSpaceShipForOwner(int owner)
		{
			return SpaceShips.Find(x => x.Owner == owner);
		}
	}

	public class GameManager : MonoBehaviour
	{
		public enum GameState
		{
			RUNNING,
			ENDED,
		}

		public class Player {
			public SpaceShip spaceShip;
			public BaseSpaceShipController controller;
		}

		public static GameManager Instance = null;

		public List<SpaceShip> spaceShips;
		[SerializeField]
		private DebugControllersSO _debugControllerSO;
		[SerializeField]
		private float _gameDuration = 60.0f;

		private List<Player> _players = new List<Player>();

		private GameState _gameState = GameState.RUNNING;

		private GameData _gameData = new GameData();

		void Awake()
		{
			Time.timeScale = 1.0f;
			Instance = this;
			_gameData.timeLeft = _gameDuration;
			foreach(SpaceShip ship in spaceShips)
			{
				Player player = new Player();
				player.spaceShip = ship;
				_players.Add(player);
			}

			List<BaseSpaceShipController> controllers = new List<BaseSpaceShipController>();
			if (GameConfiguration.Instance != null)
			{
				controllers.Add(GameConfiguration.Instance.controller1);
				controllers.Add(GameConfiguration.Instance.controller2);
			} else
			{
				controllers.Add(_debugControllerSO.p1.GetComponent<BaseSpaceShipController>());
				controllers.Add(_debugControllerSO.p2.GetComponent<BaseSpaceShipController>());
			}

			for (int i = 0; i < controllers.Count; i++)
			{
				Player player = _players[i];
				player.controller = Instantiate<BaseSpaceShipController>(controllers[i]);
				player.controller.name = controllers[i].name;
				player.spaceShip.Initialize(player.controller, i);
			}
		}


		void Start()
		{
			foreach (Player player in _players)
			{
				if (player.controller != null)
				{
					player.controller.Initialize(player.spaceShip.view, GetGameData());
				}
			}
		}


		void Update()
		{
			if (_gameState == GameState.RUNNING) {
				_gameData.timeLeft -= Time.deltaTime;
				if (_gameData.timeLeft <= 0.0f) {
					_gameData.timeLeft = 0.0f;
					_gameState = GameState.ENDED;
					foreach (Player player in _players) {
						if (player.controller != null) {
							player.controller.enabled = false;
						}
					}
					Time.timeScale = 0.0f;
				}
			}

			if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) {
				if (GameConfiguration.Instance == null) {
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
				} else {
					SceneManager.LoadScene("Launcher");
				}
				enabled = false;
			}
		}


		public GameData GetGameData()
		{
			return _gameData;
		}


		public int GetScoreForPlayer(int id)
		{
			int score = 0;
			foreach (WayPointView wayPoint in _gameData.WayPoints)
			{
				if (wayPoint.Owner == id)
				{
					score++;
				}
			}
			foreach (Player player in _players)
			{
				if (player.spaceShip == null || player.spaceShip.Owner == id)
					continue;
				score += player.spaceShip.HitCount;
			}

			return score;
		}


		public int GetWayPointScoreForPlayer(int id)
		{
			int score = 0;
			foreach (WayPointView wayPoint in _gameData.WayPoints)
			{
				if (wayPoint.Owner == id)
				{
					score++;
				}
			}
			return score;
		}


		public int GetHitScoreForPlayer(int id)
		{
			int score = 0;
			foreach (Player player in _players)
			{
				if (player.spaceShip == null || player.spaceShip.Owner == id)
					continue;
				score += player.spaceShip.HitCount;
			}
			return score;
		}


		public bool IsGameFinished()
		{
			return _gameState == GameState.ENDED;
		}


		public SpaceShip GetSpaceShipForController(BaseSpaceShipController controller)
		{
			if (!_players.Exists(x => x.controller == controller))
				return null;
			return _players.Find(x => x.controller == controller).spaceShip;
		}


		public string GetPlayerName(int id)
		{
			if (_players.Count <= id || _players[id].controller == null)
				return "";
			return _players[id].controller.name;
		}
	}

}