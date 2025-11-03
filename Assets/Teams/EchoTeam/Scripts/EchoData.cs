using DoNotModify;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class EchoData : MonoBehaviour
    {
        // ----- FIELDS ----- //
        private int _ourSpaceshipID = 0;
        private int _enemySpaceshipID = 1;

        private GameManager _gameManager;
        // ----- FIELDS ----- //

        private void Start()
        {
            _gameManager = GameManager.Instance;

            GetSpaceshipsID();
        }

        private void GetSpaceshipsID()
        {
            EchoController echoController = GetComponent<EchoController>(); 
            _ourSpaceshipID = _gameManager.GetSpaceShipForController(echoController).Owner;
            _enemySpaceshipID = (_ourSpaceshipID == 0) ? 1 : 0;
        }

        public GameData GetGameData()
        {
            return _gameManager.GetGameData();
        }

        public SpaceShipView GetOurSpaceship()
        {
            return GetGameData().GetSpaceShipForOwner(_ourSpaceshipID);
        }

        public SpaceShipView GetEnemySpaceship()
        {
            return GetGameData().GetSpaceShipForOwner(_enemySpaceshipID);
        }

        public List<WayPointView> GetWayPoints()
        {
            return GetGameData().WayPoints;
        }

        public List<AsteroidView> GetAsteroids()
        {
            return GetGameData().Asteroids;
        }

        public List<MineView> GetMines()
        {
            return GetGameData().Mines;
        }
    }
}


