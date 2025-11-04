using BehaviorDesigner.Runtime.Tasks;

namespace Echo
{
    [TaskCategory("Echo")]
    public class DropMine : EchoAction
    {
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
