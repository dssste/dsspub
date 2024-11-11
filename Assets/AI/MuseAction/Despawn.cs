using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

namespace dss.ai{
	[Serializable]
	[NodeDescription(name: "Despawn", story: "despawn [object]", category: "pic/object", id: "ce3054ed78083a0d039199c9976ae444")]
	public class Despawn: Action{
		public BlackboardVariable<GameObject> Object;

		protected override Status OnStart(){
			UnityEngine.Object.Destroy(Object.Value);
			return Status.Success;
		}
	}
}
