using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace dss.pub.options {
	[UxmlElement]
	public partial class KeybindField : BaseField<List<string>> {
		public const string bindingLabelUssClassName = "keybind-field__binding";
		public const string emptyBindingLabelUssClassName = "keybind-field__binding-empty";
		public const string interactiveRebindingIndicator = "...";

		private VisualElement inputElement;
		[UxmlAttribute] public InputActionReference key;
		[UxmlAttribute] public int enabledBits = 0b1111;
		public OptionsModel.IKeybind keybind;
		public Func<string, string> getDisplayPath = bindingPath => bindingPath switch {
			interactiveRebindingIndicator => "[ ... ]",
			null or "" => "[     ]",
			_ => $"[ {bindingPath} ]",
		};

		public KeybindField() : base("", new()) {
			focusable = false;
			inputElement = this.Q<VisualElement>(className: inputUssClassName);
			inputElement.style.flexDirection = FlexDirection.Row;
			inputElement.style.justifyContent = Justify.Center;
		}

		public void Mod(OptionsModel.IKeybind keybind) {
			this.keybind = keybind;
			var bindings = keybind.actions.FindAction(key.action.id).bindings.Select(b => b.effectivePath).ToList();
			((INotifyValueChanged<List<string>>)this).SetValueWithoutNotify(bindings);
			for (var i = 0; i < bindings.Count; i++) {
				var ve = new KeybindLabel();
				ve.Init(this, i);
				ve.SetEnabled((enabledBits & (1 << i)) != 0);
				ve.AddToClassList(bindingLabelUssClassName);
				inputElement.Add(ve);
			}

			RegisterCallback<ChangeEvent<List<string>>>(ev => {
				if (ev.target == this) {
					keybind[key.action] = ev.newValue;
				}
			});
		}

		private class KeybindLabel : Label {
			private KeybindField keybindField;
			private int bindingIndex;

			public KeybindLabel() {
				style.flexDirection = FlexDirection.Row;
				RegisterCallback<MouseDownEvent>(OnMouseDown);
			}

			public void Init(KeybindField keybindField, int bindingIndex) {
				this.keybindField = keybindField;
				this.bindingIndex = bindingIndex;
				RefreshView();
			}

			private void OnMouseDown(MouseDownEvent ev) {
				RegisterCallbackOnce<MouseUpEvent, int>(OnMouseUp, ev.button);
			}

			private void OnMouseUp(MouseUpEvent ev, int downButton) {
				if (ev.button != downButton) return;

				if (ev.button == 0) {
					var actionInstance = keybindField.keybind.actions.FindAction(keybindField.key.action.id);
					actionInstance.Disable();
					actionInstance
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
							value[bindingIndex] = actionInstance.bindings[bindingIndex].effectivePath;
							keybindField.value = value;
							actionInstance.Enable();
							RefreshView();
						})
						.OnCancel(operation => {
							actionInstance.Enable();
							RefreshView();
						})
						.Start();
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(keybindField.getDisplayPath(KeybindField.interactiveRebindingIndicator));
				} else if (ev.button == 1) {
					var actionInstance = keybindField.keybind.actions.FindAction(keybindField.key.action.id);
					actionInstance.ApplyBindingOverride(bindingIndex, "");
					var value = new List<string>(keybindField.value);
					value[bindingIndex] = "";
					keybindField.value = value;
					RefreshView();
				}
			}

			public void RefreshView() {
				var bindingPath = bindingIndex < keybindField.value.Count ? keybindField.value[bindingIndex] : null;
				if (string.IsNullOrEmpty(bindingPath)) {
					AddToClassList(emptyBindingLabelUssClassName);
				} else {
					RemoveFromClassList(emptyBindingLabelUssClassName);
				}
				((INotifyValueChanged<string>)this).SetValueWithoutNotify(keybindField.getDisplayPath(bindingPath));
			}
		}
	}
}
