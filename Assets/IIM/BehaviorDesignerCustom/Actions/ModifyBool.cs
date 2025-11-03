using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class ModifyBool : Action
	{
		public enum OPERATOR
		{
			SET = 0,
			SET_NEGATION = 1,
		}

		[Tooltip("Variable to modify")]
		public SharedBool variable;
		[Tooltip("Modification operator")]
		public OPERATOR op;
		[Tooltip("Value used with operator")]
		public SharedBool value;

		public override TaskStatus OnUpdate()
		{
			switch (op)
			{
				case OPERATOR.SET: variable.Value = value.Value; break;
				case OPERATOR.SET_NEGATION: variable.Value = !value.Value; break;
			}
			return TaskStatus.Success;
		}
	}
}