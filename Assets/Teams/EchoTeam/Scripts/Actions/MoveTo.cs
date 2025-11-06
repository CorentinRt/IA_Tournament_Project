using BehaviorDesigner.Runtime;
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

        private float _minThrustBraking = 0.5f;

        private Vector2 predictedPathPoint;
        private float predictedPathMin = 2f;
        private float predictedPathMax = 5f;
        private float predictedPathFactor = 0.5f;

        private float _deltaFromMaxSpeedConsidered = 0.3f;
        private float _deltaAngleConsideredAligned = 5f;

        private float _slowStartDistance = 0.5f;


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

            _echoDebug.AddCircle("MoveToSuccessCheck", _ourSpaceShip.Position, _echoData.DistanceToleranceToSuccess, Color.green);
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

            _echoDebug.UpdateDebugCirclePosition("MoveToSuccessCheck", targetPosition);

            if (path == null)
                return TaskStatus.Failure;

            if (path.Count <= 1)
                return TaskStatus.Success;

            // Find predicted point path to target
            float currentSpeed = _ourSpaceShip.Velocity.magnitude;
            float predictionDistance = Mathf.Clamp(predictedPathMin + currentSpeed * predictedPathFactor, predictedPathMin, predictedPathMax);

            Vector2 predictedPoint = path[1].Position;
            float currentPredictionDistance = 0f;
            Vector2 previousPredicted = _ourSpaceShip.Position;
            for (int i = 1; i < path.Count; ++i)
            {
                Vector2 tempPredicted = path[i].Position;
                currentPredictionDistance += Vector2.Distance(previousPredicted, tempPredicted);
                previousPredicted = tempPredicted;
                predictedPoint = tempPredicted;

                if (currentPredictionDistance >= predictionDistance)
                    break;
            }

            // Compute dir to Predicted pos
            Vector2 toPredicted = predictedPoint - _ourSpaceShip.Position;
            Vector2 dir = toPredicted.normalized;

            if (Vector2.Distance(_ourSpaceShip.Position, targetPosition) < _echoData.DistanceToleranceToSuccess)
            {
                
                return TaskStatus.Success;
            }

            float targetOrientation = AimingHelpers.ComputeSteeringOrient(_ourSpaceShip, _ourSpaceShip.Position + dir, 1.2f);

            float alignementVelocity = 0f;
            if (_ourSpaceShip.Velocity.sqrMagnitude > 0.001f)
                alignementVelocity = Vector2.Dot(dir, _ourSpaceShip.Velocity.normalized);

            Vector2 shipUp = Vector2.up;
            float alignementShip = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                float tempX = shipUp.x;
                float tempY = shipUp.y;
                shipUp.x = tempX * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) - tempY * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);
                shipUp.y = tempX * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) + tempY * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);
                alignementShip = Vector2.Dot(dir, shipUp);
            }

            float distanceToPredicted = toPredicted.magnitude;

            float distanceToTarget = Vector2.Distance(targetPosition, _ourSpaceShip.Position);

            float thrustPercent = 0f;

            bool hasEnoughSpeedForToward = currentSpeed >= _ourSpaceShip.SpeedMax - _deltaFromMaxSpeedConsidered;

            // case not moving at all -> accelerate
            if (currentSpeed < 0.2f)
            {
                thrustPercent = 1f;
                _echoController.GetInputDataByRef().thrust = thrustPercent;
                _echoController.GetInputDataByRef().targetOrientation = targetOrientation;
                return TaskStatus.Running;
            }
            // Case Velocity aligned to target
            else if (alignementVelocity > Mathf.Cos(_deltaAngleConsideredAligned * Mathf.Deg2Rad))
            {
                // Has enough Speed to continue
                if (hasEnoughSpeedForToward)
                {
                    thrustPercent = _minThrustBraking / 2f;

                    // Case really well Aligned -> don't need thrust
                    if (alignementVelocity > Mathf.Cos(_deltaAngleConsideredAligned / 2f * Mathf.Deg2Rad))
                    {
                        thrustPercent = 0f;
                    }
                }
                else
                {
                    // Case ship aligned with target -> accelerate to match max speed
                    if (alignementShip > Mathf.Cos(_deltaAngleConsideredAligned * Mathf.Deg2Rad))
                    {
                        thrustPercent = 1f;
                    }
                    else
                    {
                        thrustPercent = _minThrustBraking / 2f;
                    }
                }
            }
            // Case velocity not aligned to target
            else
            {
                // Case ship aligned -> accelerate to max
                if (alignementShip > Mathf.Cos(_deltaAngleConsideredAligned * Mathf.Deg2Rad))
                {
                    thrustPercent = 1f;
                }
                // Case ships not aligned
                else
                {
                    // opposite direction
                    if (alignementShip < 0)
                    {
                        thrustPercent = _minThrustBraking / 2f;
                    }
                    // nearly same direction
                    else
                    {
                        alignementShip = Mathf.Clamp01(alignementShip);

                        thrustPercent = Mathf.Lerp(_minThrustBraking, 1f, alignementShip);
                    }
                }
            }

            // case near final target -> start to reduce speed
            if (distanceToTarget < _slowStartDistance)
            {
                float percentClose = Mathf.InverseLerp(0f, 0.7f, distanceToTarget / _slowStartDistance);

                thrustPercent -= percentClose;
                thrustPercent = Mathf.Clamp01(thrustPercent);
            }

            Debug.DrawRay(_ourSpaceShip.Position, shipUp * thrustPercent * 2f, Color.red);

            // Apply to Input data
            _echoController.GetInputDataByRef().thrust = thrustPercent;
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            return TaskStatus.Success;
        }

        private Vector2 GetTargetPosition()
        {
            WayPointView waypointView = null;

            switch (_target)
            {
                case MOVETO_TARGET.ENEMY:
                    ComputePredictionToSpaceship(_ourSpaceShip.Position, Bullet.Speed, _enemySpaceShip.Position, _enemySpaceShip.Velocity, out var predictedPos);
                    return predictedPos;

                case MOVETO_TARGET.NEAREST_WAYPOINT:
                    waypointView = _echoData.GetNearestWayPoint(_ourSpaceShip.Position + _ourSpaceShip.Velocity * _echoData.VelocityModifierFactor);

                    if (waypointView != null)
                        return waypointView.Position;

                    break;

                case MOVETO_TARGET.NEAREST_WAYPOINT_NEUTRAL:
                    waypointView = _echoData.GetNearestNeutralOrEnemyWayPoint(_ourSpaceShip.Position + _ourSpaceShip.Velocity * _echoData.VelocityModifierFactor);

                    if (waypointView != null)
                        return waypointView.Position;

                    break;

                case MOVETO_TARGET.NEAREST_WAYPOINT_ENEMY:
                    waypointView = _echoData.GetNearestEnemyWayPoint(_ourSpaceShip.Position + _ourSpaceShip.Velocity * _echoData.VelocityModifierFactor);

                    if (waypointView != null)
                        return waypointView.Position;

                    waypointView = _echoData.GetNearestNeutralOrEnemyWayPoint(_ourSpaceShip.Position + _ourSpaceShip.Velocity * _echoData.VelocityModifierFactor);

                    if (waypointView != null)
                        return waypointView.Position;

                    break;
            }

            return Vector2.zero;
        }

        /*
        public Vector2 ComputePredictionToSpaceship(Vector2 targetPosition, Vector2 targetVelocity)
        {
            Vector2 dir = targetVelocity.normalized;

            Vector2 resultPosition = targetPosition;

            for (int i = 0; i < _echoData.MaxPonderationLoopPredition; ++i)
            {
                Vector2 tempPosition = targetPosition + dir * i * _echoData.IntervalPrecision;

                if (!AimingHelpers.CanHit(_ourSpaceShip, tempPosition, targetVelocity, _echoData.HitTimeTolerance))
                    continue;

                resultPosition = tempPosition;
                break;
            }

            return resultPosition;
        }
        */

        public bool ComputePredictionToSpaceship(Vector2 shooterPos, float bulletSpeed,
                                    Vector2 targetPos, Vector2 targetVel,
                                    out Vector2 interceptPoint)
        {
            Vector2 delta = targetPos - shooterPos;
            float a = Vector2.Dot(targetVel, targetVel) - bulletSpeed * bulletSpeed;
            float b = 2f * Vector2.Dot(delta, targetVel);
            float c = Vector2.Dot(delta, delta);

            float discriminant = b * b - 4 * a * c;
            if (discriminant < 0)
            {
                interceptPoint = targetPos;
                return false;
            }

            float sqrtD = Mathf.Sqrt(discriminant);
            float t1 = (-b - sqrtD) / (2 * a);
            float t2 = (-b + sqrtD) / (2 * a);

            float t = Mathf.Min(t1, t2);
            if (t < 0) t = Mathf.Max(t1, t2);
            if (t < 0)
            {
                interceptPoint = targetPos;
                return false;
            }

            interceptPoint = targetPos + targetVel * t;
            return true;
        }
    }
}