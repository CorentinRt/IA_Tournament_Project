using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckEnergyForAction : EchoConditional
    {
        public enum ACTION
        {
            MINE = 0,
            SHOOT = 1,
            SHOCKWAVE = 2,
        }

        // ----- FIELDS ----- //
        [Tooltip("Action to check energy")]
        public ACTION actionToCheck = ACTION.MINE;

        [Tooltip("Minimum of energy to always keep")]
        public SharedFloat alwaysKeepEnergy = 0.4f; // voir si on met variable dans echo data si utilisée partout

        private float _mineEnergyCost = 0.2f;
        private float _shootEnergyCost = 0.12f;
        private float _shockwaveEnergyCost = 0.4f;

        private SpaceShipView _ourSpaceShip;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _ourSpaceShip = _echoData.GetOurSpaceship();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check energy.");
                return TaskStatus.Failure;
            }

            if (_ourSpaceShip == null)
            {
                UnityEngine.Debug.LogError($"No spaceship found in check energy");
                return TaskStatus.Failure;
            }

            float currentEnergy = _ourSpaceShip.Energy;
            bool hasMinimumEnergy = false;

            switch (actionToCheck)
            {
                case ACTION.MINE:
                    hasMinimumEnergy = (currentEnergy - _mineEnergyCost >= alwaysKeepEnergy.Value);
                    break;
                case ACTION.SHOOT:
                    hasMinimumEnergy = (currentEnergy - _shootEnergyCost >= alwaysKeepEnergy.Value);
                    break;
                case ACTION.SHOCKWAVE:
                    hasMinimumEnergy = (currentEnergy - _shockwaveEnergyCost >= alwaysKeepEnergy.Value);
                    break;
            }

            return hasMinimumEnergy ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
