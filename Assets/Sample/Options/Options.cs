using System;
using System.Collections.Generic;
using System.Linq;
using dss.pub.dummy;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace dss.pub.options{
	[UxmlElement]
	public partial class Options: VisualElement{
		public Options(){
			RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
		}

		private void OnGeometryChanged(GeometryChangedEvent evt){
			var om = OptionsDummy.instance;

			Mod(this.Q<ChooseField>("locale"), om.locale);
			Mod(this.Q<ChooseField>("interact-attack"), om.interactAttack);
			// this.Query<KeybindField>().ForEach(ve => Mod(ve, om.keybind));
		}

		private void Mod(ChooseField ve, OptionsModel.IOption<string> option){
			// ve.createChoiceElement = s => new ChooseField.Choice(s);
			ve.choices = option.choices;
			ve.value = option.value;
			ve.RegisterCallback<ChangeEvent<string>>(ev => {
				if(ev.target == ve){
					option.value = ev.newValue;
				}
			});
		}

		private void Mod<T>(ChooseField ve, OptionsModel.IOption<T> option) where T: struct, Enum{
			// ve.createChoiceElement = s => new ChooseField.Choice(s);
			ve.choices = option.choices.Select(choice => choice.ToString()).ToList();
			ve.value = option.value.ToString();
			ve.RegisterCallback<ChangeEvent<string>>(ev => {
				if(ev.target == ve){
					option.value = Enum.Parse<T>(ev.newValue);
				}
			});
		}

		private void Mod(KeybindField ve, OptionsModel.IKeybind keybind){
			ve.RegisterCallback<ChangeEvent<List<string>>>(ev => {
				if(ev.target == ve){
					for(int i = 0; i < ev.newValue.Count; i++){
						keybind.value = new(){
							action = ve.key.action.name,
							bindingIndex = i,
							path = ev.newValue[i],
						};
					}
				}
			});
		}

		private class Choice: ChooseField.Choice{
			private LocalizedString localizedString;

			private Choice(string key): base(key){}

			public static Choice Create(string key){
				var choice = new Choice(key);
				choice.localizedString = new("MenuLocaleTable", key);
				return choice;
			}

			public override void InitView(){
				OnStringChanged(localizedString.GetLocalizedString());
				localizedString.StringChanged += OnStringChanged;
				RegisterCallback<DetachFromPanelEvent>(e => {
					localizedString.StringChanged -= OnStringChanged;
				});
			}

			public override void RefreshView(){
				localizedString.RefreshString();
			}

			private void OnStringChanged(string value){
				if(isChosen){
					((INotifyValueChanged<string>)this).SetValueWithoutNotify($"[ {value} ]");
				}else{
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
				}
			}
		}
	}
}
