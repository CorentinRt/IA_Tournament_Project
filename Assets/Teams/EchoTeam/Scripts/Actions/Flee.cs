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

            Vector2 shipToBullet = dangerBullet.Position - _ourSpaceShip.Position;
            shipToBullet.Normalize();

            /*
            Vector2 shipUp = Vector2.up;
            float tempX = shipUp.x;
            float tempY = shipUp.y;
            shipUp.x = tempX * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) - tempY * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);
            shipUp.y = tempX * Mathf.Sin((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad) + tempY * Mathf.Cos((_ourSpaceShip.Orientation - 90f) * Mathf.Deg2Rad);

            shipUp.Normalize();
            */

            float determinant = shipToBullet.x * _ourSpaceShip.Velocity.y - _ourSpaceShip.Velocity.x * shipToBullet.y;

            float targetOrientation = _ourSpaceShip.Orientation;

            // Bullet goes to the left of the ship -> turn right
            if (determinant > 0)
            {
                targetOrientation -= 90f;
            }
            // Bullet goes to the right of the ship -> turn left
            else
            {
                targetOrientation += 90f;
            }

            Vector2 debugUp = Vector2.up;
            float debugX = debugUp.x;
            float debugY = debugUp.y;
            debugUp.x = debugX * Mathf.Cos((targetOrientation - 90f) * Mathf.Deg2Rad) - debugY * Mathf.Sin((targetOrientation - 90f) * Mathf.Deg2Rad);
            debugUp.y = debugX * Mathf.Sin((targetOrientation - 90f) * Mathf.Deg2Rad) + debugY * Mathf.Cos((targetOrientation - 90f) * Mathf.Deg2Rad);

            Debug.DrawLine(_ourSpaceShip.Position, _ourSpaceShip.Position + debugUp * 5f, Color.green, 1f);

            float thrust = 1f;

            _echoController.GetInputDataByRef().thrust = thrust;
            _echoController.GetInputDataByRef().targetOrientation = targetOrientation;

            return TaskStatus.Running;
        }
    }
}