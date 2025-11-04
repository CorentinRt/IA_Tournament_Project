using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Echo
{
    [TaskCategory("Echo")]
    public abstract class EchoConditional : Conditional
    {
        protected EchoController _echoController;
        protected EchoData _echoData;
        protected EchoDebug _echoDebug;

        public override void OnAwake()
        {
            base.OnAwake();
            
            _echoData = GetComponent<EchoData>();
            _echoController = GetComponent<EchoController>();
            _echoDebug = GetComponent<EchoDebug>();
        }
    }
}

