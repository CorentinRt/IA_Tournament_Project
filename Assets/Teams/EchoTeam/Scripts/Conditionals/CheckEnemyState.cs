using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckEnemyState : EchoConditional
    {
        public enum STATE
        {
            NOTSTUN = 0,
            NOTHIT = 1,
            NOTSTUNANDNOTHIT = 2
        }

        // ----- FIELDS ----- //
        [Tooltip("State(s) to check")]
        public STATE stateToCheck = STATE.NOTSTUN;

        private SpaceShipView _enemySpaceShip;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _enemySpaceShip = _echoData.GetEnemySpaceship();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check enemy state.");
                return TaskStatus.Failure;
            }

            bool isNotInState = true;

            switch (stateToCheck)
            {
                case STATE.NOTSTUN:
                    isNotInState = _enemySpaceShip.HitPenaltyCountdown == 0;
                    break;

                case STATE.NOTHIT:
                    isNotInState = _enemySpaceShip.StunPenaltyCountdown == 0;
                    break;

                case STATE.NOTSTUNANDNOTHIT:
                    isNotInState = _enemySpaceShip.HitPenaltyCountdown == 0 && _enemySpaceShip.StunPenaltyCountdown == 0;
                    break;
            }

            return isNotInState ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
