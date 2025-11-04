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
        Ship = 0,
        ClosestMine = 1
    }
    
    public class LookAtTarget : EchoAction
    {
        public LookAtTargetType targetType;
        public SharedFloat angleTolerance;
        
        public override TaskStatus OnUpdate()
        {
            switch (targetType)
            {
                case LookAtTargetType.None:
                    return TaskStatus.Success;
                case LookAtTargetType.Ship:
                    return LookAtEnemyShip();
                case LookAtTargetType.ClosestMine:
                    return LookAtClosestMine();
                default:
                    return TaskStatus.Success;
            }
        }
        
        private TaskStatus LookAtEnemyShip()
        {
            // Get both spaceships
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();
            SpaceShipView enemySpaceship = _echoData.GetEnemySpaceship();

            return LookAtTargetPosition(ourSpaceship, enemySpaceship.Position);
        } 
        
        private TaskStatus LookAtClosestMine()
        {
            // Get our spaceship
            SpaceShipView ourSpaceship = _echoData.GetOurSpaceship();
            
            // Get closest mine
            MineView closestMine = _echoData.GetClosestMine();
            
            return LookAtTargetPosition(ourSpaceship,  closestMine.Position);
        }

        private TaskStatus LookAtTargetPosition(SpaceShipView spaceshipView, Vector2 targetPosition)
        {
            // Get angle formed by vector for our ship to mine
            float targetOrientation = EchoMath.OriginToTargetAngle(spaceshipView.Position, targetPosition);
            
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
    } 
    
}

