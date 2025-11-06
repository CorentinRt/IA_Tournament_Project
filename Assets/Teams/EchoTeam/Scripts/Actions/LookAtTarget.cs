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

            bool foundPrediction = ComputePredictionToSpaceship(_ourSpaceShip.Position, Bullet.Speed,
            enemySpaceship.Position, enemySpaceship.Velocity, out var predictedPos);

            //Debug.Log($"Found = {foundPrediction}, Target pos = {enemySpaceship.Position}, predicted pos {predictedPos}");

            Debug.DrawLine(ourSpaceship.Position, predictedPos, Color.yellow);

            return LookAtTargetPosition(ourSpaceship, predictedPos);
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
            return TaskStatus.Running;
        }

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

