using dss.pub.types;
using UnityEngine;

namespace dss.pub.dummy {
	[CreateAssetMenu(menuName = "dummy/get string")]
	public class GetString: ScriptableProperty<string> {
		public string value;

		public override string GetValue() {
			return value;
		}
	}
}
