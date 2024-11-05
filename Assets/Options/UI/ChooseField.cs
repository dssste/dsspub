using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace dss.pub.options{
	[UxmlElement]
	public partial class ChooseField: BaseField<string>{
		public static readonly string choiceLabelUssClassName = "choose-field__choice";
		public static readonly string chosenLabelUssClassName = "choose-field__chosen";

		private VisualElement inputElement;
		public Func<string, Choice> createChoiceElement;

		public List<string> choices{
			set{
				inputElement.Clear();
				foreach(var choice in value){
					var ve = createChoiceElement == null
						? new Choice(choice)
						: createChoiceElement(choice);
					ve.AddToClassList(choiceLabelUssClassName);
					ve.RegisterCallback<ClickEvent>(ev => {
						this.value = ve.key;
					});
					ve.InitView();
					inputElement.Add(ve);
				}
			}
		}

		public override string value{
			get => base.value;
			set{
				base.value = value;
				inputElement.Query<Choice>().ForEach(ve => {
					var isChosen = ve.key == value;
					if(ve.isChosen == isChosen) return;

					ve.isChosen = isChosen;
					if(ve.isChosen){
						ve.AddToClassList(chosenLabelUssClassName);
					}else{
						ve.RemoveFromClassList(chosenLabelUssClassName);
					}
					ve.RefreshView();
				});
			}
		}

		public class Choice: Label{
			public readonly string key;
			public bool isChosen;

			public Choice(string key){
				this.key = key;
			}

			public virtual void InitView(){
				RefreshView();
			}

			public virtual void RefreshView(){
				if(isChosen){
					((INotifyValueChanged<string>)this).SetValueWithoutNotify($"[{key}]");
				}else{
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(key);
				}
			}
		}

		public ChooseField(): base("", new()){
			focusable = false;
			inputElement = this.Q<VisualElement>(className: inputUssClassName);
			inputElement.style.flexDirection = FlexDirection.Row;
			inputElement.style.justifyContent = Justify.Center;
		}
	}
}
