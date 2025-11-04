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
        public float angleTolerance;
        
        public override TaskStatus OnUpdate()
        {
            switch (targetType)
            {
                case LookAtTargetType.None:
                    return TaskStatus.Success;
                case LookAtTargetType.Ship:
                    return LookAtShip();
                case LookAtTargetType.ClosestMine:
                    return LookAtClosestMine();
                default:
                    return TaskStatus.Success;
            }
        }
        
        private TaskStatus LookAtShip()
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
            List<MineView> mines = _echoData.GetMines();
            
            if(mines.Count == 0) return TaskStatus.Failure;
            
            MineView closestMine = mines[0];
            float closestDistance = Vector2.Distance(closestMine.Position, ourSpaceship.Position);
            foreach (MineView mine in mines)
            {
                float tempDistance = Vector2.Distance(mine.Position, ourSpaceship.Position);
                if (tempDistance < closestDistance)
                {
                    closestDistance = tempDistance;
                    closestMine = mine;
                }
            }
            
            return LookAtTargetPosition(ourSpaceship,  closestMine.Position);
        }

        private TaskStatus LookAtTargetPosition(SpaceShipView spaceshipView, Vector2 targetPosition)
        {
            // Get angle formed by vector for our ship to mine
            float targetOrientation = OriginToTargetAngle(spaceshipView.Position, targetPosition);
            
            // Compare current ship orientation to normalizedTargetOrientation
            if (Mathf.Abs(spaceshipView.Orientation - targetOrientation) <= angleTolerance)
            {
                // If we are looking at the target we return success
                return TaskStatus.Success;
            }
            
            // Else we set the targetOrientation of our spaceship and return failure
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Returns the angle form by the vector going from origin to target 
        /// </summary>
        private float OriginToTargetAngle(Vector2 origin,Vector2 target)
        {
            // Get angle formed by vector for our ship to mine
            Vector2 ourShipToEnemyShip = target - origin;
            float targetOrientation = Mathf.Atan2(ourShipToEnemyShip.y, ourShipToEnemyShip.x) * Mathf.Rad2Deg;
            
            // Convert angle from range -180/180 to 0/360
            return (targetOrientation + 360) % 360;;
        }
    } 
    
}

