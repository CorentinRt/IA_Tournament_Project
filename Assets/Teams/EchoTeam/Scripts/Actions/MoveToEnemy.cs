using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Echo
{
    [TaskCategory("Echo")]
    public class MoveToEnemy : Action
    {
        private EchoController _controller;

        private PathfindingNavigationGraph _navigationGraph;

        private EchoData _data;

        private SpaceShipView _ourSpaceShip;
        private SpaceShipView _enemySpaceShip;

        public override void OnAwake()
        {
            base.OnAwake();

            _controller = GetComponent<EchoController>();

            if (_controller == null)
                Debug.LogError("Error : no controller found");

            _data = GetComponent<EchoData>();

            if (_data == null)
                Debug.LogError("Error : no data found on controller");

            _ourSpaceShip = _data.GetOurSpaceship();

            if (_ourSpaceShip == null)
                Debug.LogError("No our spaceShip found in gameData");

            _enemySpaceShip = _data.GetEnemySpaceship();

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

            List<CellData> path = _navigationGraph.FindPathTo(_ourSpaceShip.Position, _enemySpaceShip.Position);

            if (path == null || path.Count <= 1)
            {
                if (path == null)
                {
                    return TaskStatus.Failure;
                }

                if (path.Count <= 1)
                Debug.LogWarning("Warning : Try find path not existing !");
                return TaskStatus.Success;
            }

            CellData nextCellData = path[1];

            Vector2 dir = nextCellData.Position - _ourSpaceShip.Position;

            Vector2 normalizedDir = dir.normalized;

            float targetOrientation = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;

            float currentOrientation = _controller.GetInputDataByRef().targetOrientation;

            _controller.GetInputDataByRef().targetOrientation = targetOrientation;

            float orientationGap = Mathf.Abs(targetOrientation - currentOrientation);

            float thrustPercent = Mathf.Lerp(1f, 0.1f, orientationGap / 360f);

            Vector2 normalizedVelocity = _ourSpaceShip.Velocity.normalized;

            Vector2 differenceVelocityDir = normalizedVelocity - normalizedDir;
            differenceVelocityDir.Normalize();

            if (Mathf.Abs(Mathf.Atan2(differenceVelocityDir.y, differenceVelocityDir.x) * Mathf.Rad2Deg) < 5f && _ourSpaceShip.Velocity.magnitude >= _ourSpaceShip.SpeedMax)
            {
                thrustPercent = 0f;
            }

            AimingHelpers.ComputeSteeringOrient(_ourSpaceShip, _ourSpaceShip.Position + dir, 2f);

            _controller.GetInputDataByRef().thrust = thrustPercent;

            return TaskStatus.Running;

            return base.OnUpdate();
        }
    }
}