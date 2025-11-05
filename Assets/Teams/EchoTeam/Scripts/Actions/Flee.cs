using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using UnityEngine;

namespace Echo
{
    public class Flee : EchoAction
    {
        public SharedFloat checkRadius;
        public SharedFloat tolerance;
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

            Vector2 shipUp = Vector2.up;
            float tempX = shipUp.x;
            float tempY = shipUp.y;
            shipUp.x = tempX * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) - tempY * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);
            shipUp.y = tempX * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) + tempY * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);

            shipUp.Normalize();

            float determinant = bulletToShip.x * shipUp.y - shipUp.x * bulletToShip.y;

            float targetOrientation = _ourSpaceShip.Orientation;

            if (determinant < 0)
            {
                targetOrientation += 90f;
            }
            else
            {
                targetOrientation -= 90f;
            }

            float thrust = 1f;

            _echoController.GetInputDataByRef().thrust = thrust;
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            return TaskStatus.Running;
        }
    }
}