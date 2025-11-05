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


        private int _ourSpaceshipID = 0;
        private int _enemySpaceshipID = 1;

        private GameManager _gameManager;
        // ----- FIELDS ----- //

        public float HitTimeTolerance => _hitTimeTolerance;
        public float HitToleranceAngle => _hitToleranceAngle;

        public int MaxPonderationLoopPredition => _maxPonderationLoopPredition;
        public float IntervalPrecision => _intervalPrecision;
        public float DistanceToleranceToSuccess => _distanceToleranceToSuccess;

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
                    nearestDistance = distance;
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
                    nearestDistance = distance;
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

                //Compute intersection between bullet line and ship line, if no intersection we continue to next bullet
                if (!AimingHelpers.ComputeIntersection(bullet.Position, bullet.Velocity.normalized, ourSpaceship.Position, ourSpaceship.Velocity.normalized, out Vector2 intersection))
                    continue;

                //Compute Time to intersection point for ship and bullet
                float shipDistanceToIntersection = Vector2.Distance(intersection, ourSpaceship.Position);
                float shipTimeToIntersection = ourSpaceship.Velocity.magnitude / shipDistanceToIntersection;

                float bulletDistanceToIntersection = Vector2.Distance(intersection, bullet.Position);
                float bulletTimeToIntersection = bullet.Velocity.magnitude / bulletDistanceToIntersection;

                //Check if time for bullet to arrive to point = time for ship to arrive at same point (with tolerance)
                if (Mathf.Abs(shipTimeToIntersection - bulletTimeToIntersection) <= tolerance)
                    dangerBullets.Add(bullet);
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
    }
}


