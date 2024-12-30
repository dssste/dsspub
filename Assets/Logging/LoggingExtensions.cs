using System.Text;
using Unity.Logging;
using UnityEngine;

namespace dss.pub.logging {
	public static class LoggingExtensions {
		public static void log(this object obj, string message) {
			Log.Info(message);
		}

#if (UNITY_EDITOR)
		public static void print(this object obj, params object[] args) {
			var sb = new StringBuilder();
			sb.AppendJoin(", ", args);
			if (obj is Object unityObj) {
				Debug.Log(sb.ToString(), unityObj);
			} else {
				Debug.Log(sb.ToString());
			}
		}

		public static void iprint(this Object obj, params object[] args) {
			var go = obj switch {
				Component c => c.gameObject,
				_ => obj,
			};
			if (UnityEditor.Selection.activeObject == go) {
				print(go, args);
			}
		}
#endif
	}
}
