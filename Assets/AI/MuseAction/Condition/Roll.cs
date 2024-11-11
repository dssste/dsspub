using System;
using Unity.Muse.Behavior;

namespace pic.ai{
	[Serializable]
	[Condition(name: "Roll", story: "rolled [operator] [value]", category: "pds/roll", id: "bd50eca706d9c3db049b7e8677007a34")]
	public class Roll: Condition{
		[Comparison(comparisonType: ComparisonType.All)]
		public BlackboardVariable<ConditionOperator> Operator;
		public BlackboardVariable<float> Value;

		public override bool IsTrue(){
			return ConditionUtils.Evaluate(UnityEngine.Random.value, Operator, Value.Value);
		}
	}
}
