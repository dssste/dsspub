using System.IO;
using UnityEngine;

namespace dss.pub.common {
	public static class DataPath {
		public static string savedata => Path.Combine(
			Application.persistentDataPath,
#if (UNITY_EDITOR)
			"savedata_editorplaymode"
#else
			"savedata"
#endif
		);
	}
}
