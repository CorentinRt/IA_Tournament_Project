using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class DropMine : Action
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
                UnityEngine.Debug.LogError("Couldn't find echo controller in drop mine.");
                return TaskStatus.Failure;
            }

            _echoController.GetInputDataByRef().dropMine = true;

            return TaskStatus.Success;
        }
    }
}

{

}
}
