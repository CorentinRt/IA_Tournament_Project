using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using UnityEngine;

namespace Echo
{
    public class Flee : EchoAction
    {
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
            return base.OnUpdate();

            if (_ourSpaceShip == null)
                return TaskStatus.Failure;



        }
    }
}