using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using System.Collections.Generic;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckCanHit : EchoConditional
    {
        public enum TARGET
        {
            ENEMY = 0,
            MINE = 1
        }

        // ----- FIELDS ----- //
        [Tooltip("Target to check can hit")]
        public TARGET targetToCheck = TARGET.ENEMY;

        [Tooltip("Hit time tolerance to hit enemy (in seconds)")]
        public SharedFloat hitTimeTolerance = .1f;

        [Tooltip("Angle tolerance to hit mine (in degrees)")]
        public SharedFloat angleTolerance = 5f;

        private SpaceShipView _ourSpaceship;
        private SpaceShipView _enemySpaceship;
        private List<MineView> _mines = new List<MineView>();
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _ourSpaceship = _echoData.GetOurSpaceship();
            _enemySpaceship = _echoData.GetEnemySpaceship();
            _mines = _echoData.GetMines();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check can hit.");
                return TaskStatus.Failure;
            }

            bool canHit = false;

            switch (targetToCheck)
            {
                case TARGET.ENEMY:
                    canHit = AimingHelpers.CanHit(_ourSpaceship, _enemySpaceship.Position, _enemySpaceship.Velocity, hitTimeTolerance.Value);
                    break;

                case TARGET.MINE:
                    foreach (MineView mine in _mines)
                    {
                        canHit = AimingHelpers.CanHit(_ourSpaceship, mine.Position, angleTolerance.Value);
                        if (canHit)
                        {
                            break;
                        }
                    }
                    break;
            }

            return canHit ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}