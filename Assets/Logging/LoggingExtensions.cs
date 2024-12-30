using System.IO;
using System.Text;
using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEngine;

namespace dss.pub.logging {
	public static class LoggingExtensions {
		public static string filePath => Path.Combine(Application.persistentDataPath, "savedata", "server.log");

		private static bool initialized = false;

		private static void CheckLogger() {
			if (!initialized) {
				Log.Logger = new(new LoggerConfig()
					.MinimumLevel.Info()
					.OutputTemplate("[{Level} {Timestamp}] {Message}{NewLine}{Stacktrace}")
					.WriteTo.File(filePath)
					.WriteTo.StdOut(minLevel: LogLevel.Warning));
				initialized = true;
			}
		}

		private static string MakeMessage(object key, string message) {
			return message + "[" + key switch {
				string s => s,
				Component component => component.gameObject.name + "." + component.name,
				Object unityObj => unityObj.name,
				_ => key.ToString()
			} + "]";
		}


		public static void log(this object key, string message) {
			CheckLogger();
			Log.Info(MakeMessage(key, message));
		}

		public static void warn(this object key, string message) {
			CheckLogger();
			Log.Warning(MakeMessage(key, message));
		}

		public static void err(this object key, string message) {
			CheckLogger();
			Log.Error(MakeMessage(key, message));
		}

#if (UNITY_EDITOR)
		public static void print(this object key, params object[] args) {
			var sb = new StringBuilder();
			sb.AppendJoin(", ", args);
			if (key is Object unityObj) {
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
