using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [TaskCategory("Echo")]
    public class MoveTo : EchoAction
    {
        private enum MOVETO_TARGET
        {
            ENEMY = 0,
            NEAREST_POINT = 1
        }

        [SerializeField] private MOVETO_TARGET _target;

        private PathfindingNavigationGraph _navigationGraph;

        private SpaceShipView _ourSpaceShip;
        private SpaceShipView _enemySpaceShip;

        public override void OnAwake()
        {
            base.OnAwake();

            _ourSpaceShip = _echoData.GetOurSpaceship();

            if (_ourSpaceShip == null)
                Debug.LogError("No our spaceShip found in gameData");

            _enemySpaceShip = _echoData.GetEnemySpaceship();

            if (_enemySpaceShip == null)
                Debug.LogError("No enemy spaceShip found in gameData");

            _navigationGraph = GetComponent<PathfindingNavigationGraph>();

            if (_navigationGraph == null)
                Debug.LogError("Error : no navigation graph found");
        }

        public override void OnStart()
        {
            base.OnStart();

        }

        public override TaskStatus OnUpdate()
        {
            if (_ourSpaceShip == null || _enemySpaceShip == null || _navigationGraph == null)
                return TaskStatus.Failure;

            Vector2 targetPosition = Vector2.zero;

            switch (_target)
            {
                case MOVETO_TARGET.ENEMY:
                    targetPosition = _enemySpaceShip.Position;
                    break;

                case MOVETO_TARGET.NEAREST_POINT:

                    break;
            }

            List<CellData> path = _navigationGraph.FindPathTo(_ourSpaceShip.Position, targetPosition);

            if (path == null || path.Count <= 1)
            {
                if (path == null)
                {
                    return TaskStatus.Failure;
                }

                if (path.Count <= 1)
                //Debug.LogWarning("Warning : Try find path not existing !");
                return TaskStatus.Success;
            }

            CellData nextCellData = path[1];

            Vector2 dir = nextCellData.Position - _ourSpaceShip.Position;

            Vector2 normalizedDir = dir.normalized;

            float targetOrientation = AimingHelpers.ComputeSteeringOrient(_ourSpaceShip, _ourSpaceShip.Position + dir, 1.2f); ;
            //float targetOrientation = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;

            float currentOrientation = _echoController.GetInputDataByRef().targetOrientation;

            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            float orientationGap = Mathf.Abs(targetOrientation - currentOrientation);

            //float thrustPercent = Mathf.Lerp(1f, 0.1f, orientationGap / 360f);
            float thrustPercent = 1f;

            Vector2 normalizedVelocity = _ourSpaceShip.Velocity.normalized;

            Vector2 differenceVelocityDir = normalizedVelocity - normalizedDir;
            differenceVelocityDir.Normalize();

            if (Mathf.Abs(Mathf.Atan2(differenceVelocityDir.y, differenceVelocityDir.x) * Mathf.Rad2Deg) < 5f && _ourSpaceShip.Velocity.magnitude >= _ourSpaceShip.SpeedMax)
            {
                thrustPercent = 0f;
            }

            _echoController.GetInputDataByRef().thrust = thrustPercent;

            return TaskStatus.Running;
        }
    }
}