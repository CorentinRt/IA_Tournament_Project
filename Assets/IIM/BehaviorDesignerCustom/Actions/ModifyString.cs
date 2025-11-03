using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class ModifyString : Action
	{
		public enum OPERATOR
		{
			SET = 0,
			CONCATENATE = 1,
			REMOVE_ALL = 2,
		}

		[Tooltip("Variable to modify")]
		public SharedString variable;
		[Tooltip("Modification operator")]
		public OPERATOR op;
		[Tooltip("Value used with operator")]
		public SharedString value;

		public override TaskStatus OnUpdate()
		{
			switch (op)
			{
				case OPERATOR.SET: variable.Value = value.Value; break;
				case OPERATOR.CONCATENATE: variable.Value = variable.Value + value.Value; break;
				case OPERATOR.REMOVE_ALL: variable.Value = variable.Value.Replace(value.Value, ""); break;
			}
			return TaskStatus.Success;
		}
	}
}