using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;

namespace Echo
{
    [TaskCategory("Echo")]
    public class CheckEnergyForAction : Conditional
    {
        public enum ACTION
        {
            MINE = 0,
            SHOOT = 1,
            SHOCKWAVE = 2,
        }

        // ----- FIELDS ----- //
        [Tooltip("Action to check energy")]
        public ACTION ActionToCheck = ACTION.MINE;

        [Tooltip("Minimum of energy to always keep")]
        public SharedFloat AlwaysKeepEnergy = 0.4f; // voir si on met variable dans echo data si utilisée partout

        private EchoData _echoData;

        private float _mineEnergyCost = 0.2f;
        private float _shootEnergyCost = 0.12f;
        private float _shockwaveEnergyCost = 0.4f;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _echoData = GetComponent<EchoData>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoData == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo data in check energy.");
                return TaskStatus.Failure;
            }

            GameData gameData = _echoData.GetGameData();
            SpaceShipView spaceShip = _echoData.GetOurSpaceship();

            if (spaceShip == null || gameData == null)
            {
                UnityEngine.Debug.LogError($"No spaceship or game data found in check energy");
                return TaskStatus.Failure;
            }

            float currentEnergy = spaceShip.Energy;
            bool hasMinimumEnergy = false;

            switch (ActionToCheck)
            {
                case ACTION.MINE:
                    hasMinimumEnergy = (currentEnergy - _mineEnergyCost >= AlwaysKeepEnergy.Value);
                    break;
                case ACTION.SHOOT:
                    hasMinimumEnergy = (currentEnergy - _shootEnergyCost >= AlwaysKeepEnergy.Value);
                    break;
                case ACTION.SHOCKWAVE:
                    hasMinimumEnergy = (currentEnergy - _shockwaveEnergyCost >= AlwaysKeepEnergy.Value);
                    break;
            }

            return hasMinimumEnergy ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
