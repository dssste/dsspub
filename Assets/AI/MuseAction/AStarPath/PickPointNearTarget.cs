using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;
using Pathfinding;

namespace pic.ai{
	[Serializable, NodeDescription(name: "PickPointNearTarget", story: "picks [point] [distance] away from [Target]", category: "pic/path", id: "ce4631127fba8d3c0bfb2ed016dd4ef3")]
	public class PickPointNearTarget : Action{
		public BlackboardVariable<Vector2> Point;
		public BlackboardVariable<float> Distance;
		public BlackboardVariable<GameObject> Target;

		protected override Status OnStart(){
			var point = Target.Value.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle.normalized * Distance.Value;
			Point.Value = AstarPath.active.GetNearest(point, NNConstraint.Walkable).position;
			return Status.Success;
		}
	}
}
