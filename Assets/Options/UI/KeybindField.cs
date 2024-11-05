using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace dss.pub.options{
	[UxmlElement]
	public partial class KeybindField: BaseField<List<string>>{
		public static readonly string bindingLabelUssClassName = "keybind-field__binding";
		public static readonly string emptyBindingLabelUssClassName = "keybind-field__binding-empty";

		[UxmlAttribute] public InputActionReference key;
		[UxmlAttribute] public int enabledBits = 0b1111;

		public KeybindField(): base("", new()){
			focusable = false;
			var inputElement = this.Q<VisualElement>(className: inputUssClassName);
			inputElement.style.flexDirection = FlexDirection.Row;
			inputElement.style.justifyContent = Justify.Center;
			RegisterCallbackOnce((EventCallback<GeometryChangedEvent>)(ev => {
				var value = key.action.bindings.Select(binding => binding.effectivePath).ToList();
				((INotifyValueChanged<List<string>>)this).SetValueWithoutNotify(value);
				for(var i = 0; i < value.Count; i++){
					var keybind = new Keybind(this, i);
					keybind.SetEnabled((enabledBits & (1 << i)) != 0);
					keybind.AddToClassList(bindingLabelUssClassName);
					inputElement.Add(keybind);
				}
			}));
		}

		private class Keybind: Label{
			public readonly KeybindField keybindField;
			private readonly int bindingIndex;

			public Keybind(KeybindField keybindField, int bindingIndex){
				this.keybindField = keybindField;
				this.bindingIndex = bindingIndex;
				style.flexDirection = FlexDirection.Row;
				RegisterCallback<MouseDownEvent>(OnMouseDown);
				RefreshView();
			}

			private void OnMouseDown(MouseDownEvent ev){
				RegisterCallbackOnce<MouseUpEvent, int>(OnMouseUp, ev.button);
			}

			private void OnMouseUp(MouseUpEvent ev, int downButton){
				if(ev.button != downButton) return;

				if(ev.button == 0){
					var action = keybindField.key.action;
					action.Disable();
					action
						.PerformInteractiveRebinding()
						.WithControlsHavingToMatchPath("Keyboard")
						.WithControlsHavingToMatchPath("Mouse")
						.WithExpectedControlType("Button")
						.WithExpectedControlType("Axis")
						.WithControlsExcluding("<Mouse>/scroll/y")
						.WithControlsExcluding("<Mouse>/LeftButton")
						.WithControlsExcluding("<Mouse>/press")
						.WithCancelingThrough("<Keyboard>/escape")
						.WithTargetBinding(bindingIndex)
						.OnComplete(operation => {
							var value = new List<string>(keybindField.value);
							value[bindingIndex] = action.bindings[bindingIndex].effectivePath;
							keybindField.value = value;
							action.Enable();
							RefreshView();
						})
						.OnCancel(operation => {
							action.Enable();
							RefreshView();
						})
						.Start();
					((INotifyValueChanged<string>)this).SetValueWithoutNotify("[ ... ]");
				}else if(ev.button == 1){
 					var action = keybindField.key.action;
					action.ApplyBindingOverride(bindingIndex, "");
					var value = new List<string>(keybindField.value);
					value[bindingIndex] = "";
					keybindField.value = value;
					RefreshView();
				}
			}

			private void RefreshView(){
				var bindingPath = bindingIndex < keybindField.value.Count ? keybindField.value[bindingIndex] : null;
				if(string.IsNullOrEmpty(bindingPath)){
					AddToClassList(emptyBindingLabelUssClassName);
					((INotifyValueChanged<string>)this).SetValueWithoutNotify("[     ]");
				}else{
					RemoveFromClassList(emptyBindingLabelUssClassName);
					// todo: built in to sprite?
					((INotifyValueChanged<string>)this).SetValueWithoutNotify($"[ {bindingPath} ]");
				}
			}
		}
	}
}
