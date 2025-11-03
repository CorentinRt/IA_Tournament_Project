using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class CompareString : AConditionalCompare
	{
		[Tooltip("The first variable to compare")]
		public SharedString variable;
		[Tooltip("The comparison operator")]
		public OPERATOR op = OPERATOR.EQUAL;
		[Tooltip("The variable to compare to")]
		public SharedString compareTo;

		public override TaskStatus OnUpdate()
		{
			return Compare(variable.Value, compareTo.Value, op);
		}
	}
}
