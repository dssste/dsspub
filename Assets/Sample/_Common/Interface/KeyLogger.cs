using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dss.pub.dummy {
	public class KeyLogger : MonoBehaviour {
		private void OnF(InputValue inputValue) {
			Debug.Log("f pressed");
		}
	}
}
