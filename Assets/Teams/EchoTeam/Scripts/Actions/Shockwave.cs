using BehaviorDesigner.Runtime.Tasks;
using DoNotModify;
using TMPro;

namespace Echo
{
    [TaskCategory("Echo")]
    public class Shockwave : Action
    {
        // ----- FIELDS ----- //
        private EchoController _echoController;
        // ----- FIELDS ----- //

        public override void OnAwake()
        {
            base.OnAwake();

            _echoController = GetComponent<EchoController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (_echoController == null)
            {
                UnityEngine.Debug.LogError("Couldn't find echo controller in shockwave.");
                return TaskStatus.Failure;
            }

            _echoController.GetInputDataByRef().fireShockwave = true;

            return TaskStatus.Success;
        }
    }

    


}
