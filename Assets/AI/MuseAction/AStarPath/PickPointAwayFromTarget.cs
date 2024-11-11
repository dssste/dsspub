using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;
using Pathfinding;

namespace pic.ai{
	[Serializable]
	[NodeDescription(name: "PickPointAwayFromTarget", story: "[origin] picks [point] [distance] away from [target]", category: "pic/path", id: "21e14b7ee891d3e3bb4fb5a328420621")]
	public class PickPointAwayFromTarget: Action{
		public BlackboardVariable<GameObject> Origin;
		public BlackboardVariable<Vector2> Point;
		public BlackboardVariable<float> Distance;
		public BlackboardVariable<GameObject> Target;

		protected override Status OnStart(){
			var direction = (Origin.Value.transform.position - Target.Value.transform.position).normalized;
			var point = Origin.Value.transform.position + direction * Distance.Value;
			Point.Value = AstarPath.active.GetNearest(point, NNConstraint.Walkable).position;
			return Status.Success;
		}
	}
}
