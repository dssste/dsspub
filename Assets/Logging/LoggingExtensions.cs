using System.IO;
using System.Text;
using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEngine;

namespace dss.pub.logging {
	public static class LoggingExtensions {
		public static string filePath = Path.Combine(dss.pub.common.DataPath.savedata, "server.log");

		private static bool initialized = false;

		private static void CheckLogger() {
			if (!initialized) {
				Log.Logger = new(new LoggerConfig()
					.MinimumLevel.Info()
					.WriteTo.File(filePath, outputTemplate: "[{Timestamp}] {Message} [{Level}]")
					.WriteTo.UnityEditorConsole(minLevel: LogLevel.Warning, outputTemplate: "{Message}"));
				initialized = true;
			}
		}

		private static string MakeMessage(object key, string message) {
			return message + " (" + key switch {
				string s => s,
				Component component => component.gameObject.name + "." + component.GetType().Name,
				Object unityObj => unityObj.name,
				_ => key.ToString()
			} + ")";
		}

		public static object log(this object key, string message) {
			CheckLogger();
			Log.Info(MakeMessage(key, message));
			return key;
		}

		public static object warn(this object key, string message) {
			CheckLogger();
			Log.Warning(MakeMessage(key, message));
			return key;
		}

		public static object err(this object key, string message) {
			CheckLogger();
			Log.Error(MakeMessage(key, message));
			return key;
		}

#if (UNITY_EDITOR)
		public static object print(this object key, params object[] args) {
			var sb = new StringBuilder();
			sb.AppendJoin(", ", args);
			if (key is Object unityObj) {
				Debug.Log(sb.ToString(), unityObj);
			} else {
				Debug.Log(sb.ToString());
			}
			return key;
		}

		public static object iprint(this Object obj, params object[] args) {
			var go = obj switch {
				Component c => c.gameObject,
				_ => obj,
			};
			if (UnityEditor.Selection.activeObject == go) {
				print(go, args);
			}
			return obj;
		}
#endif
	}
}
