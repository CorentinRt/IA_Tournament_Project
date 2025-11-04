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

        private SpaceShipView _ourSpaceship;
        private SpaceShipView _enemySpaceship;
        private List<WayPointView> _waypoints = new List<WayPointView>();
        private List<MineView> _mines = new List<MineView>();
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

            float distance = 0f;
            bool isNear = false;

            switch (targetToCheck)
            {
                case TARGET.ENEMY:
                    distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, _enemySpaceship.Position);
                    isNear = distance <= checkDistance.Value;
                    break;

                case TARGET.WAYPOINT:
                    foreach (WayPointView waypoint in _waypoints)
                    {
                        distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, waypoint.Position);
                        if (distance <= checkDistance.Value)
                        {
                            isNear = true;
                            break;
                        }
                    }
                    break;

                case TARGET.MINE:
                    foreach (MineView mine in _mines)
                    {
                        distance = UnityEngine.Vector3.Distance(_ourSpaceship.Position, mine.Position);
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

