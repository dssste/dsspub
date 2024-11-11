using System;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

namespace pic.ai{
	[Serializable]
	[NodeDescription(name: "PickFloat", story: "picks within [ [min] , [max] ] as [value]", category: "pic/value", id: "3944f0805ceee0e7f646e5c05ea187a5")]
	public class PickFloat: Action{
		public BlackboardVariable<float> Min;
		public BlackboardVariable<float> Max;
		public BlackboardVariable<float> Value;

		protected override Status OnStart(){
			Value.Value = UnityEngine.Random.Range(Min.Value, Max.Value);
			return Status.Success;
		}
	}
}
