using UnityEngine;

namespace dss.pub.types {
	public abstract class ScriptableProperty<T>: ScriptableObject {
		public abstract T GetValue();
	}
}
