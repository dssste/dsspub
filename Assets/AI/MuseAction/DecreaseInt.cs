using System;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

namespace dss.ai{
	[Serializable]
	[NodeDescription(name: "DecreaseInt", story: "reduces [value] by [rate]", category: "pic/value", id: "5e2857c62fe34adf4db7ed2381b61df2")]
	public class DecreaseInt: Action{
		public BlackboardVariable<int> Value;
		public BlackboardVariable<int> Rate;

		protected override Status OnStart(){
			Value.Value -= Rate.Value;
			return Status.Success;
		}
	}
}
