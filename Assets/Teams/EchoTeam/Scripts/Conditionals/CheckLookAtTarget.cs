using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using Echo;
using UnityEngine;

namespace Echo
{
    public class CheckLookAtTarget : EchoConditional
    {
        public LookAtTargetType targetType;
        public SharedFloat angleTolerance;
        
        public override TaskStatus OnUpdate()
        {
            switch (targetType)
            {
                case LookAtTargetType.None:
                    return TaskStatus.Failure;
                case LookAtTargetType.EnemyShip:
                    return CheckLookAtEnemyShip();
                case LookAtTargetType.ClosestMine:
                    return CheckLookAtClosestMine();
                default:
                    return TaskStatus.Failure;
            }
        }

        private TaskStatus CheckLookAtEnemyShip()
        {
            return CheckLookAtTargetPosition(_echoData.GetOurSpaceship(), _echoData.GetEnemySpaceship().Position);
        }

        private TaskStatus CheckLookAtClosestMine()
        {
            return CheckLookAtTargetPosition(_echoData.GetOurSpaceship(), _echoData.GetClosestMine().Position);
        }

        private TaskStatus CheckLookAtTargetPosition(SpaceShipView spaceshipView, Vector2 targetPosition)
        {
            // Get angle formed by vector for our ship to mine
            float targetOrientation = EchoMath.OriginToTargetVectorAngle(spaceshipView.Position, targetPosition);
            
            // Compare current ship orientation to normalizedTargetOrientation
            if (Mathf.Abs(spaceshipView.Orientation - targetOrientation) <= angleTolerance.Value)
            {
                // If we are looking at the target we return success
                return TaskStatus.Success;
            }
            
            // Else we return failure
            return TaskStatus.Failure;
        }
    }
}

