using System.IO;
using System.Text;
using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEngine;

namespace dss.pub.logging {
	public static class LoggingExtensions {
		public static string filePath => Path.Combine(Application.persistentDataPath, "savedata", "server.log");

		private static bool initialized = false;

		public static void log(this object obj, string message) {
			if (!initialized) {
				Log.Logger = new(new LoggerConfig().MinimumLevel.Info().WriteTo.File(filePath));
				initialized = true;
			}

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
