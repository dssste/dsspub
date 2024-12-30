using Unity.Logging;

public static class LoggingExtensions {
	public static void log(this object obj, string message) {
		Log.Info(message);
	}
}
