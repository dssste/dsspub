using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;
using Pathfinding;

namespace pic.ai{
	[Serializable, NodeDescription(name: "PickPointTowardTarget", story: "picks [point] [distance] from [origin] towards [target]", category: "pic/path", id: "b72ebb74829a1fc95cc5721c9f2f5328")]
	public class PickPointTowardTarget : Action{
		public BlackboardVariable<Vector2> Point;
		public BlackboardVariable<float> Distance;
		public BlackboardVariable<GameObject> Origin;
		public BlackboardVariable<GameObject> Target;

		protected override Status OnStart(){
			var direction = (Target.Value.transform.position - Origin.Value.transform.position);
			if(direction.magnitude > Distance.Value){
				direction = direction.normalized * Distance.Value;
			}
			var point = Origin.Value.transform.position + direction;
			Point.Value = AstarPath.active.GetNearest(point, NNConstraint.Walkable).position;
			return Status.Success;
		}
	}
}
