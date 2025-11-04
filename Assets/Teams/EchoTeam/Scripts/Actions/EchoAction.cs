using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using UnityEngine;

namespace Echo
{
    [TaskCategory("Echo")]
    public abstract class EchoAction : Action
    {
        protected EchoController _echoController;
        protected EchoData _echoData;

        public override void OnAwake()
        {
            base.OnAwake();
            
            _echoData = GetComponent<EchoData>();
            _echoController = GetComponent<EchoController>();
        }
    }
}

