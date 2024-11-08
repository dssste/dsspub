using UnityEngine;
using UnityEngine.InputSystem;

namespace dss.pub.dummy{
	public class Dummy: MonoBehaviour{
		private void Awake(){
			OptionsDummy.instance.keybind.ApplyAll(GetComponent<PlayerInput>().actions);
		}
	}
}
