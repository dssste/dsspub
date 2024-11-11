using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;
using Pathfinding;

namespace dss.ai{
	[Serializable, NodeDescription(name: "PathTo", story: "[agent] pathes to [point]", category: "path", id: "75a2b6b431b437216868fe309b5b56ba")]
	public class PathTo: Action{
		private const float timeout = 10f;

		public BlackboardVariable<GameObject> Agent;
		public BlackboardVariable<Vector2> Point;

		private AIPath agent;
		private float startTime;

		protected override Status OnStart(){
			agent = Agent.Value.GetComponent<AIPath>();
			agent.destination = Point.Value;
			startTime = Time.time;
			return Status.Running;
		}

		protected override Status OnUpdate(){
			if(Time.time - startTime > timeout) return Status.Success;

			if(agent.reachedDestination){
				return Status.Success;
			}else{
				return Status.Running;
			}
		}
	}
}

