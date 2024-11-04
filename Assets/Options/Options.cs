using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace dss.pub.options{
	public class OptionsModel{
		private static string folder => Path.Combine(Application.persistentDataPath, "savedata");
		public static string filePath => Path.Combine(folder, "options.json");

		private static OptionsModel _instance;
		public static OptionsModel instance{
			get{
				if(_instance == null){
					try{
						_instance = JsonUtility.FromJson<OptionsModel>(File.ReadAllText(filePath));
					}catch(System.Exception e){
						if(e is not (FileNotFoundException or DirectoryNotFoundException)){
							Debug.LogWarning($"Failed to load options: {e.Message}");
						}
						_instance = new();
					}
					_instance.Init();
				}
				return _instance;
			}
		}

		private void Save(){
			if(!System.IO.Directory.Exists(folder)){
				System.IO.Directory.CreateDirectory(folder);
			}
			File.WriteAllText(filePath, JsonUtility.ToJson(this, true));
		}

		protected virtual void Init(){
			_locale.Init();
			_keybind.Init();
		}

		public IOption<string> locale => _locale;
		public IKeybind keybind => _keybind;

		[SerializeField] private LocaleEntry _locale = new();
		[SerializeField] private Keybind _keybind = new();

		protected OptionsModel(){}


		public interface IOption<T>{
			List<T> choices{get;}
			T value{get;set;}
			Action onValueChanged{get;set;}
		}

		public interface IKeybind{
			[Serializable]
			public class Value{
				public string action;
				public int bindingIndex;
				public string path;
			}

			Value value{set;}
		}

		[Serializable]
		protected class Entry<T>: IOption<T>{
			public List<T> choices => getChoices().ToList();

			[SerializeField] private T _value;
			public T value{
				get => _value;
				set{
					if(!isValid(value)) return;

					_value = value;
					onValueChanged?.Invoke();
					instance.Save();
				}
			}

			public void Init(){
				if(!isValid(_value)){
					_value = getFallbackValue();
				}
			}

			public Func<IEnumerable<T>> getChoices;
			public Func<T, bool> isValid;
			public Func<T> getFallbackValue;

			public Action onValueChanged{get;set;}
		}

		[Serializable]
		protected class EnumEntry<T>: Entry<T> where T: Enum{
			public EnumEntry(){
				// getChoices = () => EnumExtensions.GetEnumerable<T>();
				isValid = value => Enum.IsDefined(typeof(T), value);
				getFallbackValue = () => default;
			}
		}

		private class LocaleEntry: Entry<string>{
			public LocaleEntry(){
				getChoices = GetChoices;
				isValid = IsValid;
				getFallbackValue = GetFallbackValue;
				/* the built in locale selector would try to pick a locale on start up, and
				 * instead of overwriting it's selection, we make it able to pick the right one
				 * by syncing our option to a player pref that it consumes. This is a slight optimization
				 * and not the true data source of selected locale. The true data source should still be OptionsModel.
				 * A pitfall is that the outside world would want to use += instead of = on this event,
				 * but even if they overwrite the listeners, they would only omit an optimization, and would not break the system.
				 */
				onValueChanged = RefreshLocalePlayerPref;
			}

			private static IEnumerable<string> GetChoices() => LocalizationSettings.AvailableLocales.Locales.Select(locale => locale.Identifier.Code);
			private static bool IsValid(string value) => !string.IsNullOrEmpty(value);
			private static string GetFallbackValue() => LocalizationSettings.SelectedLocale.Identifier.Code;
			private static void RefreshLocalePlayerPref() => PlayerPrefs.SetString("ui-manager-selected-locale", _instance.locale.value);
		}

		[Serializable]
		private class Keybind: IKeybind{
			[SerializeField] private List<IKeybind.Value> _values = new();
			// where does the user define the actions?
			private InputActionMap actions;

			public IKeybind.Value value{
				set{
					if(IsDefault(value)){
						_values.RemoveAll(v => v.action == value.action && v.bindingIndex == value.bindingIndex);
					}else{
						bool found = false;
						foreach(var v in _values){
							if(v.action == value.action && v.bindingIndex == value.bindingIndex){
								v.path = value.path;
								found = true;
								break;
							}
						}
						if(!found){
							_values.Add(value);
						}
					}
					instance.Save();
				}
			}

			public void Init(){
				foreach(var value in _values){
					var action = actions.FindAction(value.action);
					if(action == null) continue;

					action.ApplyBindingOverride(value.bindingIndex, value.path);
				}
			}

			private bool IsDefault(IKeybind.Value value){
				var action = actions.FindAction(value.action);
				if(action == null) return false;
				if(value.bindingIndex >= action.bindings.Count) return false;

				return action.bindings[value.bindingIndex].path == value.path;
			}
		}
	}
}
