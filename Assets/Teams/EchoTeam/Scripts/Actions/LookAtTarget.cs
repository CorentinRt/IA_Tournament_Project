using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using UnityEngine;

namespace Echo
{
    public enum LookAtTargetType
    {
        None = -1,
        EnemyShip = 0,
        NearestMine = 1
    }
    
    public class LookAtTarget : EchoAction
    {
        public LookAtTargetType targetType;
        public SharedFloat angleTolerance;
        private SpaceShipView _ourSpaceShip;

        public override void OnAwake()
        {
            base.OnAwake();

            _ourSpaceShip = _echoData.GetOurSpaceship();

            if (_ourSpaceShip == null)
                Debug.LogError("No our spaceShip found in gameData");
        }

        public override TaskStatus OnUpdate()
        {
            switch (targetType)
            {
                case LookAtTargetType.None:
                    return TaskStatus.Success;
                case LookAtTargetType.EnemyShip:
                    return LookAtEnemyShip();
                case LookAtTargetType.NearestMine:
                    return LookAtNearestMine();
                default:
                    return TaskStatus.Success;
            }
        }
        
        private TaskStatus LookAtEnemyShip()
        {
            // Get both spaceships
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();
            SpaceShipView enemySpaceship = _echoData.GetEnemySpaceship();

            Vector2 targetPos = ComputePredictionToSpaceship(enemySpaceship.Position, enemySpaceship.Velocity);

            return LookAtTargetPosition(ourSpaceship, targetPos);
        } 
        
        private TaskStatus LookAtNearestMine()
        {
            // Get our spaceship
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();
            
            // Get closest mine
            MineView closestMine = _echoData.GetNearestMine();
            
            return LookAtTargetPosition(ourSpaceship,  closestMine.Position);
        }

        private TaskStatus LookAtTargetPosition(SpaceShipView spaceshipView, Vector2 targetPosition)
        {
            // Get angle formed by vector for our ship to mine
            float targetOrientation = EchoMath.OriginToTargetVectorAngle(spaceshipView.Position, targetPosition);
            
            // Compare current ship orientation to normalizedTargetOrientation
            if (Mathf.Abs(spaceshipView.Orientation - targetOrientation) <= angleTolerance.Value)
            {
                // If we are looking at the target we return success
                return TaskStatus.Success;
            }
            
            // Else we set the targetOrientation of our spaceship and return failure
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;
            return TaskStatus.Failure;
        }

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
    } 
    
}

