using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Echo
{
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

            Vector2 targetPosition = GetTargetPosition();

            List<CellData> path = _navigationGraph.FindPathTo(_ourSpaceShip.Position, targetPosition);

            if (path == null)
                return TaskStatus.Failure;

            if (path.Count <= 1)
                return TaskStatus.Success;

            CellData nextCellData = path[1];

            Vector2 dir = nextCellData.Position - _ourSpaceShip.Position;

            Vector2 normalizedDir = dir.normalized;

            float targetOrientation = AimingHelpers.ComputeSteeringOrient(_ourSpaceShip, _ourSpaceShip.Position + dir, 1.2f);

            Vector2 targetOrientationVector = Vector2.up;

            // Debug computeSteeringOrient
            {
                targetOrientationVector.x = targetOrientationVector.x * Mathf.Cos(targetOrientation * Mathf.Deg2Rad) - targetOrientationVector.y * Mathf.Sin(targetOrientation * Mathf.Deg2Rad);

                targetOrientationVector.y = targetOrientationVector.x * Mathf.Cos(targetOrientation * Mathf.Deg2Rad) + targetOrientationVector.y * Mathf.Sin(targetOrientation * Mathf.Deg2Rad);

                //Debug.DrawLine(_ourSpaceShip.Position, _ourSpaceShip.Position + targetOrientationVector * 5f, Color.red, 2f);
            }

            float currentOrientation = _echoController.GetInputDataByRef().targetOrientation;

            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            float orientationGap = Mathf.Abs(targetOrientation - currentOrientation);

            float thrustPercent = Mathf.Lerp(1f, 0.1f, orientationGap / 360f);

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

        private Vector2 GetTargetPosition()
        {
            switch (_target)
            {
                case MOVETO_TARGET.ENEMY:
                    return _enemySpaceShip.Position;

                case MOVETO_TARGET.NEAREST_POINT:

                    break;
            }

            return Vector2.zero;
        }
    }
}