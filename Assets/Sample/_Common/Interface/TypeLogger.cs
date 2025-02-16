using dss.pub.types;
using UnityEngine;

namespace dss.pub.dummy {
	public class TypeLogger: MonoBehaviour {
		public ScriptableProperty<string> hello;
		public string world;

		private void Start() {
			print($"hello {hello?.GetValue()}");
			print($"world {world}");
		}
	}
}
