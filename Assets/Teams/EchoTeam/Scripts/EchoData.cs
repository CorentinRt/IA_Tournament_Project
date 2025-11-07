using BehaviorDesigner.Runtime;
using DoNotModify;
using IIM;
using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    public class EchoData : MonoBehaviour
    {
        // ----- FIELDS ----- //
        [Header("Hit")]
        [SerializeField] private float _hitTimeTolerance = 0.15f;
        [SerializeField] private float _hitToleranceAngle = 10f;

        [Header("MoveTo")]
        [SerializeField] private int _maxPonderationLoopPredition = 20;
        [SerializeField] private float _intervalPrecision = 0.1f;
        [SerializeField] private float _distanceToleranceToSuccess = 0.8f;
        [SerializeField] private float _velocityModifierFactor = 0.5f;


        private int _ourSpaceshipID = 0;
        private int _enemySpaceshipID = 1;

        private GameManager _gameManager;
        // ----- FIELDS ----- //

        public float HitTimeTolerance => _hitTimeTolerance;
        public float HitToleranceAngle => _hitToleranceAngle;

        public int MaxPonderationLoopPredition => _maxPonderationLoopPredition;
        public float IntervalPrecision => _intervalPrecision;
        public float DistanceToleranceToSuccess => _distanceToleranceToSuccess;
        public float VelocityModifierFactor => _velocityModifierFactor;

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

        public WayPointView GetNearestWayPoint(Vector2 position)
        {
            List<WayPointView> waypoints = GetWayPoints();

            if (waypoints.Count == 0)
                return null;

            WayPointView nearestWaypoint = null;
            float nearestDistance = Mathf.Infinity;

            foreach (WayPointView waypoint in waypoints)
            {
                float distance = UnityEngine.Vector3.Distance(position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWaypoint = waypoint;
                }
            }

            return nearestWaypoint;
        }

        public WayPointView GetNearestEnemyWayPoint(Vector2 position)
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

                float distance = UnityEngine.Vector3.Distance(position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
                    nearestWaypoint = waypoint;
                }
            }

            return nearestWaypoint;
        }

        public WayPointView GetNearestNeutralOrEnemyWayPoint(Vector2 position)
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

                float distance = UnityEngine.Vector3.Distance(position, waypoint.Position);
                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
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

        public BulletView GetNearestBullet()
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<BulletView> bullets = GetBullets();

            if (bullets.Count == 0)
                return null;

            BulletView nearestBullet = null;
            float nearestDistance = Mathf.Infinity;

            foreach (BulletView bullet in bullets)
            {
                float distance = UnityEngine.Vector3.Distance(ourSpaceship.Position, bullet.Position);
                if (distance <= nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBullet = bullet;
                }
            }

            return nearestBullet;
        }

        public BulletView GetNearestBulletDanger(float radius = 0.3f, float tolerance = 0.3f)
        {
            SpaceShipView ourSpaceship = GetOurSpaceship();
            List<BulletView> bullets = GetBullets();

            List<BulletView> dangerBullets = new List<BulletView>();

            foreach (BulletView bullet in bullets)
            {
                if (Vector2.Distance(bullet.Position, ourSpaceship.Position) > radius)
                    continue;

                float oritentationBullet = EchoMath.OriginToTargetVectorAngle(Vector2.right, bullet.Velocity.normalized);

                if (EchoMath.CanHit(oritentationBullet, bullet.Position, ourSpaceship.Position, ourSpaceship.Velocity, tolerance))
                {
                    dangerBullets.Add(bullet);
                }
            }

            if (dangerBullets.Count <= 0)
            {
                return null;
            }

            BulletView nearestBullet = null;
            float nearestDistance = Mathf.Infinity;

            foreach (BulletView bullet in dangerBullets)
            {
                float distance = Vector2.Distance(bullet.Position, ourSpaceship.Position);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestBullet = bullet;
                }
            }

            return nearestBullet;
        }

        public int GetOurSpaceshipScore()
        {
            return _gameManager.GetScoreForPlayer(_ourSpaceshipID);
        }

        public int GetEnemySpaceshipScore()
        {
            return _gameManager.GetScoreForPlayer(_enemySpaceshipID);
        }
    }
}


