using DoNotModify;
using UnityEngine;

namespace Echo
{
    public class EchoData : MonoBehaviour
    {
        // ----- FIELDS ----- //
        public static EchoData Instance = null;

        private int _ourSpaceshipID = 0;
        private int _enemySpaceshipID = 1;

        private GameManager _gameManager;
        // ----- FIELDS ----- //

        private void Awake()
        {
            Instance = this;
        }

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
    }
}


