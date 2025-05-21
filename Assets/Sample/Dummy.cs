using dss.pub.logging;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dss.pub.dummy{
	public class Dummy: MonoBehaviour{
		private void Awake(){
			OptionsDummy.instance.keybind.actions = GetComponent<PlayerInput>().actions;
		}

		private void Start(){
			// this.log("Dummy started");
			// this.warn("Dummy started, warning");
			// this.err("Dummy started, error");
			// this.warn("Dummy , warning x 2");
			// this.warn("Dummy , warning x 3");
			// this.err("Dummy , error x 2");
		}
	}
}
