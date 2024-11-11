using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

namespace dss.ai{
	[Serializable]
	[NodeDescription(name: "SeekNearbyTarget", story: "seeks [layer] near [origin] within [range] as [result]", category: "pic/seek", id: "e816ef9e2417a7a4712f148ed141f688")]
	public class SeekNearbyTarget: Action{
		public BlackboardVariable<int> Layer;
		public BlackboardVariable<GameObject> Origin;
		public BlackboardVariable<GameObject> Result;
		public BlackboardVariable<float> Range;

		protected override Status OnStart(){
			var collider = Physics2D.OverlapCircle(Origin.Value.transform.position, Range.Value, 1 << Layer.Value);
			Result.Value = collider
				? collider.gameObject
				: null;
			return Status.Success;
		}
	}
}
