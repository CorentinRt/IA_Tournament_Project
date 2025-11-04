using DoNotModify;
using IIM;
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

        public void InitData()
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

        public MineView GetNearestMine()
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<MineView> mines = GetMines();
            
            if(mines.Count == 0) return null;
            
            MineView closestMine = mines[0];
            float closestDistance = Vector2.Distance(closestMine.Position, ourSpaceship.Position);
            foreach (MineView mine in mines)
            {
                float tempDistance = Vector2.Distance(mine.Position, ourSpaceship.Position);
                if (tempDistance < closestDistance)
                {
                    closestDistance = tempDistance;
                    closestMine = mine;
                }
            }
            
            return closestMine;
        }

        public WayPointView GetNearestWayPoint()
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<WayPointView> waypoints = GetWayPoints();

            if (waypoints.Count == 0)
                return null;

            WayPointView nearestWaypoint = waypoints[0];
            float nearestDistance = Vector2.Distance(waypoints[0].Position, ourSpaceship.Position);

            foreach (WayPointView waypoint in waypoints)
            {
                float distance = UnityEngine.Vector3.Distance(ourSpaceship.Position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestWaypoint = waypoint;
                }
            }

            return nearestWaypoint;
        }

        public WayPointView GetNearestEnemyWayPoint()
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<WayPointView> waypoints = GetWayPoints();

            if (waypoints.Count == 0)
                return null;

            WayPointView nearestWaypoint = null;
            float nearestDistance = Mathf.Infinity;

            foreach (WayPointView waypoint in waypoints)
            {
                if (waypoint.Owner == ourSpaceship.Owner ||waypoint.Owner == -1)
                    continue;

                float distance = UnityEngine.Vector3.Distance(ourSpaceship.Position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestWaypoint = waypoint;
                }
            }

            return nearestWaypoint;
        }

        public WayPointView GetNearestNeutralOrEnemyWayPoint()
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<WayPointView> waypoints = GetWayPoints();

            if (waypoints.Count == 0)
                return null;

            WayPointView nearestWaypoint = null;
            float nearestDistance = Mathf.Infinity;

            foreach (WayPointView waypoint in waypoints)
            {
                if (waypoint.Owner == ourSpaceship.Owner)
                    continue;

                float distance = UnityEngine.Vector3.Distance(ourSpaceship.Position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestWaypoint = waypoint;
                }
            }

            return nearestWaypoint;
        }

        public List<BulletView> GetBullets()
        {
            return GetGameData().Bullets;
        }

        public float GetShockwaveRadius()
        {
            return 2.2f;
        }
    }
}


