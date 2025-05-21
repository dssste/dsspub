using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace dss.pub.options {
	[UxmlElement]
	public partial class ChooseField : BaseField<string> {
		public static readonly string choiceLabelUssClassName = "choose-field__choice";
		public static readonly string chosenLabelUssClassName = "choose-field__chosen";

		private VisualElement inputElement;
		[UxmlAttribute] public int enabledBits = -1;
		public Func<string, Choice> createChoiceElement = s => new Choice(s);

		public List<string> choices {
			set {
				inputElement.Clear();
				for (int i = 0; i < value.Count; i++) {
					var ve = createChoiceElement(value[i]);
					ve.SetEnabled((enabledBits & (1 << i)) != 0);
					ve.AddToClassList(choiceLabelUssClassName);
					ve.RegisterCallback<ClickEvent>(ev => {
						this.value = ve.key;
					});
					ve.InitView();
					inputElement.Add(ve);
				}
			}
		}

		public override string value {
			get => base.value;
			set {
				base.value = value;
				inputElement.Query<Choice>().ForEach(ve => {
					var isChosen = ve.key == value;
					if (ve.isChosen == isChosen) return;

					ve.isChosen = isChosen;
					if (ve.isChosen) {
						ve.AddToClassList(chosenLabelUssClassName);
					} else {
						ve.RemoveFromClassList(chosenLabelUssClassName);
					}
					ve.RefreshView();
				});
			}
		}

		public ChooseField() : base("", new()) {
			focusable = false;
			inputElement = this.Q<VisualElement>(className: inputUssClassName);
			inputElement.style.flexDirection = FlexDirection.Row;
			inputElement.style.justifyContent = Justify.Center;
		}

		public void Mod(OptionsModel.IOption<string> option) {
			choices = option.choices;
			value = option.value;
			RegisterCallback<ChangeEvent<string>>(ev => {
				if (ev.target == this) {
					option.value = ev.newValue;
				}
			});
		}

		public void Mod<T>(OptionsModel.IOption<T> option) where T : struct, Enum {
			choices = option.choices.Select(choice => choice.ToString()).ToList();
			value = option.value.ToString();
			RegisterCallback<ChangeEvent<string>>(ev => {
				if (ev.target == this) {
					option.value = Enum.Parse<T>(ev.newValue);
				}
			});
		}

		public class Choice : Label {
			public readonly string key;
			public bool isChosen;

			public Choice(string key) {
				this.key = key;
			}

			public virtual void InitView() {
				RefreshView();
			}

			public virtual void RefreshView() {
				if (isChosen) {
					((INotifyValueChanged<string>)this).SetValueWithoutNotify($"[{key}]");
				} else {
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(key);
				}
			}
		}

		public class LocalizedChoice : Choice {
			private LocalizedString localizedString;

			public LocalizedChoice(string key) : this("MenuLocaleTable", key) { }
			public LocalizedChoice(string table, string key) : base(key) {
				localizedString = new(table, key);
			}

			public override void InitView() {
				OnStringChanged(localizedString.GetLocalizedString());
				localizedString.StringChanged += OnStringChanged;
				RegisterCallback<DetachFromPanelEvent>(e => {
					localizedString.StringChanged -= OnStringChanged;
				});
			}

			public override void RefreshView() {
				localizedString.RefreshString();
			}

			private void OnStringChanged(string value) {
				if (isChosen) {
					((INotifyValueChanged<string>)this).SetValueWithoutNotify($"[ {value} ]");
				} else {
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
				}
			}
		}
	}
}
