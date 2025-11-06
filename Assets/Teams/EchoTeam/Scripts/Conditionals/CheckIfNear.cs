using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckIfNear : EchoConditional
    {
        public enum TARGET
        {
            ENEMY = 0,
            WAYPOINT = 1,
            MINE = 2,
            BULLET = 3
        }

        // ----- FIELDS ----- //
        [Tooltip("Target to check radius")]
        public TARGET targetToCheck = TARGET.ENEMY;

        [Tooltip("Max distance to check target")]
        public SharedFloat checkDistance = 0.4f;

        private SpaceShipView _ourSpaceship;
        private SpaceShipView _enemySpaceship;

        private string _debugCircleName;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _ourSpaceship = _echoData.GetOurSpaceship();
            _enemySpaceship = _echoData.GetEnemySpaceship();

            _debugCircleName = _echoDebug.AddCircle("CheckIfNearRadius", _ourSpaceship.Position, checkDistance.Value, UnityEngine.Color.blue, true);
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check if near.");
                return TaskStatus.Failure;
            }

            _echoDebug.UpdateDebugCirclePosition(_debugCircleName, _ourSpaceship.Position);

            float distance = 0f;
            bool isNear = false;

            switch (targetToCheck)
            {
                case TARGET.ENEMY:
                    distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, _enemySpaceship.Position);
                    isNear = distance <= checkDistance.Value;
                    break;

                case TARGET.WAYPOINT:
                    WayPointView nearestWaypoint = _echoData.GetNearestEnemyWayPoint(_ourSpaceship.Position);
                    if (nearestWaypoint == null) return TaskStatus.Failure;
                    distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, nearestWaypoint.Position);
                    isNear = distance <= checkDistance.Value;
                    break;

                case TARGET.MINE:
                    MineView nearestMine = _echoData.GetNearestMine();
                    if (nearestMine == null) return TaskStatus.Failure;
                    distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, nearestMine.Position);
                    isNear = distance <= checkDistance.Value;
                    break;

                case TARGET.BULLET:
                    BulletView nearestBullet = _echoData.GetNearestBullet();
                    if (nearestBullet == null) return TaskStatus.Failure;
                    distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, nearestBullet.Position);
                    isNear = distance <= checkDistance.Value;
                    break;
            }

            return isNear ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}

