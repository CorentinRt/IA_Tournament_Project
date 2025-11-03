using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class ModifyInt : Action
	{
		public enum OPERATOR
		{
			SET = 0,
			ADD = 1,
			SUBSTRAT = 2,
			MULTIPLY = 3,
			DIVIDE = 4,
			MODULO = 5,
		}

		[Tooltip("Variable to modify")]
		public SharedInt variable;
		[Tooltip("Modification operator")]
		public OPERATOR op;
		[Tooltip("Value used with operator")]
		public SharedInt value;

		public override TaskStatus OnUpdate()
		{
			switch (op)
			{
				case OPERATOR.SET: variable.Value = value.Value; break;
				case OPERATOR.ADD: variable.Value = variable.Value + value.Value; break;
				case OPERATOR.SUBSTRAT: variable.Value = variable.Value - value.Value; break;
				case OPERATOR.MULTIPLY: variable.Value = variable.Value * value.Value; break;
				case OPERATOR.DIVIDE: variable.Value = variable.Value / value.Value; break;
				case OPERATOR.MODULO: variable.Value = variable.Value % value.Value; break;
			}
			return TaskStatus.Success;
		}
	}
}