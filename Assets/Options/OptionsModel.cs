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
		protected static T GetInstance<T>() where T: OptionsModel{
			if(_instance == null){
				try{
					_instance = JsonUtility.FromJson<T>(File.ReadAllText(filePath));
					if(_instance == null){
						Debug.LogWarning("Failed to parse options");
						_instance = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
					}
				}catch(System.Exception e){
					if(e is not (FileNotFoundException or DirectoryNotFoundException)){
						Debug.LogWarning($"Failed to load options: {e.Message}");
					}
					_instance = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
				}
			}
			return (T)_instance;
		}

		private static void Save(){
			if(!System.IO.Directory.Exists(folder)){
				System.IO.Directory.CreateDirectory(folder);
			}
			File.WriteAllText(filePath, JsonUtility.ToJson(_instance, true));
		}

		protected OptionsModel(){}

		public interface IOption<T>{
			List<T> choices{get;}
			T value{get;set;}
			Action<T> onValueChanged{get;set;}
		}

		public interface IKeybind{
			[Serializable]
			public class Value{
				public string action;
				public int bindingIndex;
				public string path;
			}

			Value value{set;}

			void ApplyAll(InputActionAsset inputActionAsset);
		}

		[Serializable]
		protected class Entry<T>: IOption<T>{
			public List<T> choices => getChoices().ToList();

			[SerializeField] private T _value;
			public T value{
				get{
					if(!isValid(_value)){
						_value = getFallbackValue();
					}
					return _value;
				}
				set{
					if(!isValid(value)) return;

					_value = value;
					onValueChanged?.Invoke(_value);
					Save();
				}
			}

			public Func<IEnumerable<T>> getChoices;
			public Func<T, bool> isValid;
			public Func<T> getFallbackValue = () => default;

			public Action<T> onValueChanged{get;set;}
		}

		[Serializable]
		protected class EnumEntry<T>: Entry<T> where T: Enum{
			public EnumEntry(){
				getChoices = GetChoices;
				isValid = value => Enum.IsDefined(typeof(T), value);
			}

			private static IEnumerable<T> GetChoices(){
				return ((T[])Enum.GetValues(typeof(T))).OrderBy(e => e);
			}
		}

		[Serializable]
		protected class LocaleEntry: IOption<string>{
			[SerializeField] private string _value;

			public LocaleEntry(){
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

			public string value{
				get => _value;
				set{
					var locale = LocalizationSettings.AvailableLocales.GetLocale(value);
					if(LocalizationSettings.SelectedLocale != locale){
						LocalizationSettings.SelectedLocale = locale;
					}
				}
			}

			public Action<string> onValueChanged{get;set;}
		}

		[Serializable]
		protected class Keybind: IKeybind{
			[SerializeField] private List<IKeybind.Value> _values = new();
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
					Save();
				}
			}

			private InputActionAsset inputActionAsset;

			public void ApplyAll(InputActionAsset inputActionAsset){
				foreach(var binding in _values){
					var action = inputActionAsset.FindAction(binding.action);
					if(action == null) continue;

					action.ApplyBindingOverride(binding.bindingIndex, binding.path);
				}
				this.inputActionAsset = inputActionAsset;
			}

			private bool IsDefault(IKeybind.Value value){
				if(inputActionAsset == null) return false;

				var action = inputActionAsset.FindAction(value.action);
				if(action == null) return false;
				if(value.bindingIndex >= action.bindings.Count) return false;

				return action.bindings[value.bindingIndex].path == value.path;
			}
		}
	}
}
