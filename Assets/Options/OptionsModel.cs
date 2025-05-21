using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace dss.pub.options {
	public class OptionsModel {
		private static string folder => Path.Combine(Application.persistentDataPath, "savedata");
		public static string filePath => Path.Combine(folder, "options.json");

		private static OptionsModel _instance;
		protected static T GetInstance<T>() where T : OptionsModel {
			if (_instance == null) {
				try {
					_instance = JsonUtility.FromJson<T>(File.ReadAllText(filePath));
					if (_instance == null) {
						Debug.LogWarning("Failed to parse options");
						_instance = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
					}
				} catch (System.Exception e) {
					if (e is not (FileNotFoundException or DirectoryNotFoundException)) {
						Debug.LogWarning($"Failed to load options: {e.Message}");
					}
					_instance = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
				}
			}
			return (T)_instance;
		}

		private static void Save() {
			if (!System.IO.Directory.Exists(folder)) {
				System.IO.Directory.CreateDirectory(folder);
			}
			File.WriteAllText(filePath, JsonUtility.ToJson(_instance, true));
		}

		protected OptionsModel() { }

		public interface IOption<T> {
			List<T> choices { get; }
			T value { get; set; }
			Action<T> onValueChanged { get; set; }
		}

		public interface IKeybind {
			InputActionAsset actions { get; set; }
			IEnumerable<string> this[InputAction action] { set; }
			Action<InputAction> onValueChanged { get; set; }
		}

		[Serializable]
		protected class Entry<T> : IOption<T> {
			public List<T> choices => getChoices().ToList();

			[SerializeField] private T _value;
			public T value {
				get {
					if (!isValid(_value)) {
						_value = getFallbackValue();
					}
					return _value;
				}
				set {
					if (!isValid(value)) return;

					_value = value;
					onValueChanged?.Invoke(_value);
					Save();
				}
			}

			public Func<IEnumerable<T>> getChoices;
			public Func<T, bool> isValid;
			public Func<T> getFallbackValue = () => default;

			public Action<T> onValueChanged { get; set; }
		}

		[Serializable]
		protected class EnumEntry<T> : Entry<T> where T : Enum {
			public EnumEntry() {
				getChoices = GetChoices;
				isValid = value => Enum.IsDefined(typeof(T), value);
			}

			private static IEnumerable<T> GetChoices() {
				return ((T[])Enum.GetValues(typeof(T))).OrderBy(e => e);
			}
		}

		[Serializable]
		protected class LocaleEntry : IOption<string> {
			[SerializeField] private string _value;

			public LocaleEntry() {
				LocalizationSettings.SelectedLocaleAsync.Completed += handle => {
					_value = handle.Result.Identifier.Code;
					Save();
				};
				LocalizationSettings.Instance.OnSelectedLocaleChanged += locale => {
					_value = locale.Identifier.Code;
					Save();
				};
			}

			public List<string> choices => LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.Identifier.Code).ToList();

			public string value {
				get => _value;
				set {
					var locale = LocalizationSettings.AvailableLocales.GetLocale(value);
					if (LocalizationSettings.SelectedLocale != locale) {
						LocalizationSettings.SelectedLocale = locale;
					}
				}
			}

			public Action<string> onValueChanged { get; set; }
		}

		[Serializable]
		protected class Keybind : IKeybind {
			[Serializable]
			public class Value {
				public string name;
				public string id;
				public List<string> bindings;
			}

			[SerializeField] private List<Value> values = new();

			[NonSerialized] public InputActionAsset _actions;

			public InputActionAsset actions {
				get => _actions;
				set {
					_actions = value;
					foreach (var v in values) {
						var actionInstance = _actions.FindAction(v.name);
#if (UNITY_EDITOR)
						var actionInstanceById = _actions.FindAction(v.id);
						if (actionInstance == null) {
							Debug.Log($"{v.name} not found, using {actionInstanceById} ( {v.id} ) instead");
							actionInstance = actionInstanceById;
						} else if (!object.ReferenceEquals(actionInstance, actionInstanceById)) {
							Debug.Log($"{actionInstance} ( {v.name} ) does not match {actionInstanceById} ( {v.id} ), using the later one instead");
							actionInstance = actionInstanceById;
						}
#else
						if (actionInstance == null) {
							actionInstance = _actions.FindAction(v.id);
						}
#endif
						if (actionInstance == null) continue;

						for (int i = 0; i < v.bindings.Count; i++) {
							var path = v.bindings[i];
							if (!string.IsNullOrEmpty(path) && actionInstance.bindings[i].path != path) {
								actionInstance.ApplyBindingOverride(i, path);
							}
						}
					}
				}
			}

			public IEnumerable<string> this[InputAction action] {
				set {
					if (actions == null) return;

					var actionInstance = actions.FindAction(action.id);
					if (actionInstance == null) return;

					var id = actionInstance.id.ToString();
					var bindings = value.Take(actionInstance.bindings.Count).ToList();

					if (actionInstance.bindings.Select(b => b.path).SequenceEqual(bindings)) {
						values.RemoveAll(v => v.id == id);
					} else {
						bool found = false;
						var name = (action.actionMap.name + "/" + action.name).Replace("(Clone)", "");
						foreach (var v in values) {
							if (v.id == id) {
								v.name = name;
								v.bindings = bindings;
								found = true;
								break;
							}
						}
						if (!found) {
							values.Add(new() {
								name = name,
								id = id,
								bindings = bindings,
							});
						}
					}
					Save();
					onValueChanged?.Invoke(actionInstance);
				}
			}

			public Action<InputAction> onValueChanged { get; set; }
		}
	}
}
