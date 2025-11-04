using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckIfNear : Conditional
    {
        public enum TARGET
        {
            ENEMY = 0,
            WAYPOINT = 1,
            MINE = 2,
        }

        // ----- FIELDS ----- //
        [Tooltip("Target to check radius")]
        public TARGET targetToCheck = TARGET.ENEMY;

        [Tooltip("Max distance to check target")]
        public SharedFloat checkDistance = 0.4f;

        private EchoData _echoData;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _echoData = GetComponent<EchoData>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check if near.");
                return TaskStatus.Failure;
            }

            SpaceShipView ourSpaceShip = _echoData.GetOurSpaceship();
            SpaceShipView enemySpaceShip = _echoData.GetEnemySpaceship();
            List<WayPointView> wayPoints = _echoData.GetWayPoints();
            List<MineView> mines = _echoData.GetMines();

            float distance = 0f;
            bool isNear = false;

            switch (targetToCheck)
            {
                case TARGET.ENEMY:
                    distance = UnityEngine.Vector3.Distance(ourSpaceShip.Position, enemySpaceShip.Position);
                    isNear = distance <= checkDistance.Value;
                    break;

                case TARGET.WAYPOINT:
                    foreach (WayPointView waypoint in wayPoints)
                    {
                        distance = UnityEngine.Vector3.Distance(ourSpaceShip.Position, waypoint.Position);
                        if (distance <= checkDistance.Value)
                        {
                            isNear = true;
                            break;
                        }
                    }
                    break;

                case TARGET.MINE:
                    foreach (MineView mine in mines)
                    {
                        distance = UnityEngine.Vector3.Distance(ourSpaceShip.Position, mine.Position);
                        if (distance <= checkDistance.Value)
                        {
                            isNear = true;
                            break;
                        }
                    }
                    break;
            }

            return isNear ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}

