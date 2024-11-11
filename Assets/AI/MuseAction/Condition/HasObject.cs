using System;
using UnityEngine;
using Unity.Muse.Behavior;

namespace dss.pub.ai{
	[Serializable]
	[Condition(name: "HasObject", story: "[has] [object]", category: "Variable Conditions", id: "118cd40e027fe8111ea05d7da64a9c0b")]
	public class HasObject: Condition{
		public BlackboardVariable<bool> Has;
		public BlackboardVariable<GameObject> Object;

		public override bool IsTrue(){
			return (Object.Value == null) != Has.Value;
		}
	}
}
