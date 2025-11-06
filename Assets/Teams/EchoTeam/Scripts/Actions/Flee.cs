using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using UnityEngine;

namespace Echo
{
    public class Flee : EchoAction
    {
        public SharedFloat checkRadius = 5f;
        public SharedFloat tolerance = 0.6f;
        public SharedFloat angleNeededInvertDirectionFront = 30f;
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
            if (_ourSpaceShip == null)
                return TaskStatus.Failure;

            BulletView dangerBullet = _echoData.GetNearestBulletDanger(checkRadius.Value, tolerance.Value);

            if (dangerBullet == null)
                return TaskStatus.Success;

            Vector2 bulletToShip = _ourSpaceShip.Position - dangerBullet.Position;
            bulletToShip.Normalize();

            Vector2 bulletDir = dangerBullet.Velocity.normalized;
            
            Vector2 shipUp = EchoMath.Rotate(Vector2.right, _ourSpaceShip.Orientation);
            Vector2 shipToBullet = -bulletToShip;

            // useful to know which direction is better to flee
            float determinant = bulletDir.x * bulletToShip.y - bulletDir.y * bulletToShip.x;

            // if bullet from behind -> invert direction
            float scalar = Vector2.Dot(shipUp, shipToBullet);

            // If bullet from front but already has enough rotation to flee -> invert direction
            float alreadyEnoughOrtientationFront = 1f;
            if (scalar > 0)
            {
                float angle = EchoMath.OriginToTargetVectorAngle(shipUp, shipToBullet);

                if (angle > angleNeededInvertDirectionFront.Value)
                {
                    alreadyEnoughOrtientationFront *= -1f;
                }
            }

            float rotationDir = -Mathf.Sign(determinant) * Mathf.Sign(scalar) * alreadyEnoughOrtientationFront;

            float targetOrientation = _ourSpaceShip.Orientation + 90f * rotationDir;

            Vector2 debugUp = EchoMath.Rotate(Vector2.right, targetOrientation);

            Debug.DrawLine(_ourSpaceShip.Position, _ourSpaceShip.Position + debugUp * 5f, Color.green, 1f);

            float thrust = 1f;

            _echoController.GetInputDataByRef().thrust = thrust;
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            return TaskStatus.Running;
        }
    }
}