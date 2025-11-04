using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;
using UnityEngine;

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
        private List<MineView> _mines = new List<MineView>();
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _shockwaveRadius = _echoData.GetShockwaveRadius();

            _ourSpaceShip = _echoData.GetOurSpaceship();
            _enemySpaceShip = _echoData.GetEnemySpaceship();
            _mines = _echoData.GetMines();

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
                    isInRadius = distance <= _shockwaveRadius / 2;
                    break;

                case TARGET.MINE:
                    foreach (MineView mine in _mines)
                    {
                        distance = UnityEngine.Vector3.Distance(_ourSpaceShip.Position, mine.Position);
                        if (distance <= _shockwaveRadius / 2)
                        {
                            isInRadius = true;
                            break;
                        }
                    }
                    break;
            }

            return isInRadius ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
