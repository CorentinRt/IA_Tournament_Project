using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;

namespace Echo
{
    public class CheckEnergyForMultipleActions : EchoConditional
    {
        // ----- FIELDS ----- //
        [Tooltip("Check mine energy")]
        public SharedBool checkMine = false;

        [Tooltip("Check shoot energy")]
        public SharedBool checkShoot = false;

        [Tooltip("Check shockwave energy")]
        public SharedBool checkShockwave = false;

        [Tooltip("Minimum of energy to always keep")]
        public SharedFloat alwaysKeepEnergy = 0.4f;

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
            if (!checkMine.Value && !checkShoot.Value && !checkShockwave.Value) return TaskStatus.Success;

            float currentEnergy = _ourSpaceShip.Energy;
            float actionsCost = 0.0f;

            if (checkMine.Value) actionsCost += _mineEnergyCost;
            if (checkShoot.Value) actionsCost += _shootEnergyCost;
            if (checkShockwave.Value) actionsCost += _shockwaveEnergyCost;

            bool hasMinimumEnergy = (currentEnergy - actionsCost >= alwaysKeepEnergy.Value);

            return hasMinimumEnergy ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
