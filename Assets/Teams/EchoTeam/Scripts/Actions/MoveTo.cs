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
            NEAREST_WAYPOINT = 1,
            NEAREST_WAYPOINT_NEUTRAL = 2,
            NEAREST_WAYPOINT_ENEMY = 3
        }

        [SerializeField] private MOVETO_TARGET _target;

        private PathfindingNavigationGraph _navigationGraph;

        private SpaceShipView _ourSpaceShip;
        private SpaceShipView _enemySpaceShip;

        private float _minThrustBraking = 0.1f;

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

            // Compute dir to next cellData
            Vector2 dir = path[1].Position - _ourSpaceShip.Position;
            Vector2 normalizedDir = dir.normalized;

            float targetOrientation = AimingHelpers.ComputeSteeringOrient(_ourSpaceShip, _ourSpaceShip.Position + dir, 1.2f);

            Vector2 velocity = _ourSpaceShip.Velocity;

            Vector2 velocityDiffAngleVector = normalizedDir - velocity.normalized;
            velocityDiffAngleVector.Normalize();

            //float velocityDiffAngle = Mathf.Abs(Mathf.Atan2(velocityDiffAngleVector.y, velocityDiffAngleVector.x) * Mathf.Rad2Deg);
            float velocityDiffAngle = EchoMath.OriginToTargetVectorAngle(velocity.normalized, normalizedDir);

            //Debug.DrawRay(_ourSpaceShip.Position, normalizedDir * 2f, Color.red);
            //Debug.DrawRay(_ourSpaceShip.Position, velocity.normalized * 2f, Color.green);

            float thrustPercent = 0f;

            float dotProduct = velocity.x * dir.y + velocity.y * dir.x;

            // Determine if need to use thrust
            bool canThrust = velocityDiffAngle > 10f || velocity.magnitude < _ourSpaceShip.SpeedMax - 0.5f || dotProduct < 0;

            if (canThrust)
            {
                thrustPercent = Mathf.Lerp(1f, _minThrustBraking, velocityDiffAngle / 180f);

                // Adjust thrust percent if facing right dir but velocity is wrong direction
            }
            
            // Apply to data
            _echoController.GetInputDataByRef().thrust = thrustPercent;
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            return TaskStatus.Running;
        }

        private Vector2 GetTargetPosition()
        {
            WayPointView waypointView = null;

            switch (_target)
            {
                case MOVETO_TARGET.ENEMY:
                    return _enemySpaceShip.Position;

                case MOVETO_TARGET.NEAREST_WAYPOINT:
                    waypointView = _echoData.GetNearestWayPoint();

                    if (waypointView != null)
                        return waypointView.Position;

                    break;

                case MOVETO_TARGET.NEAREST_WAYPOINT_NEUTRAL:
                    waypointView = _echoData.GetNearestNeutralOrEnemyWayPoint();

                    if (waypointView != null)
                        return waypointView.Position;

                    break;

                case MOVETO_TARGET.NEAREST_WAYPOINT_ENEMY:
                    waypointView = _echoData.GetNearestEnemyWayPoint();

                    if (waypointView != null)
                        return waypointView.Position;

                    waypointView = _echoData.GetNearestNeutralOrEnemyWayPoint();

                    if (waypointView != null)
                        return waypointView.Position;

                    break;
            }

            return Vector2.zero;
        }
    }
}