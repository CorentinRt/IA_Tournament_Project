using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckShockwaveRadius : EchoConditional
    {
        public enum TARGET
        {
            ENEMY = 0,
            MINE = 1
        }

        // ----- FIELDS ----- //
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Target to check is in radius")]
        public TARGET targetToCheck = TARGET.ENEMY;

        private float _shockwaveRadius;

        private SpaceShipView _ourSpaceShip;
        private SpaceShipView _enemySpaceShip;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _shockwaveRadius = _echoData.GetShockwaveRadius();

            _ourSpaceShip = _echoData.GetOurSpaceship();
            _enemySpaceShip = _echoData.GetEnemySpaceship();

            _echoDebug.AddCircle("ShockwaveRadius", _ourSpaceShip.Position, _shockwaveRadius, Color.magenta);
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check can hit.");
                return TaskStatus.Failure;
            }

            _echoDebug.UpdateDebugCirclePosition("ShockwaveRadius", _ourSpaceShip.Position);

            float distance;
            bool isInRadius = false;

            switch (targetToCheck)
            {
                case TARGET.ENEMY:
                    distance = UnityEngine.Vector3.Distance(_ourSpaceShip.Position, _enemySpaceShip.Position);
                    isInRadius = distance <= _shockwaveRadius;
                    break;

                case TARGET.MINE:
                    MineView nearestMine = _echoData.GetNearestMine();
                    distance = UnityEngine.Vector3.Distance(_ourSpaceShip.Position, nearestMine.Position);
                    isInRadius = distance <= _shockwaveRadius;
                    break;
            }

            return isInRadius ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
