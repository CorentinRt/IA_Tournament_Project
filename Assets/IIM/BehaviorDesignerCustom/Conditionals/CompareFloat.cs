using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace IIM
{
	[TaskCategory("IIM")]
	public class CompareFloat : AConditionalCompare
	{
		[Tooltip("The first variable to compare")]
		public SharedFloat variable;
		[Tooltip("The comparison operator")]
		public OPERATOR op = OPERATOR.EQUAL;
		[Tooltip("The variable to compare to")]
		public SharedFloat compareTo;

		public override TaskStatus OnUpdate()
		{
			return Compare(variable.Value, compareTo.Value, op);
		}
	}
}
