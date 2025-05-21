using dss.pub.dummy;
using UnityEngine;
using UnityEngine.UIElements;

namespace dss.pub.options {
	[UxmlElement]
	public partial class Options: VisualElement {
		public Options() {
#if(UNITY_EDITOR)
			if(panel != null && panel.contextType == ContextType.Editor) return;
			if(!Application.isPlaying) return;
#endif
			RegisterCallbackOnce<GeometryChangedEvent>(OnGeometryChanged);
		}

		private void OnGeometryChanged(GeometryChangedEvent evt) {
			var om = OptionsDummy.instance;

			var localeField = this.Q<ChooseField>("locale");
			localeField.createChoiceElement = s => new ChooseField.LocalizedChoice("StringLocaleTable", s);
			localeField.Mod(om.locale);

			var interactAttackField = this.Q<ChooseField>("interact-attack");
			interactAttackField.createChoiceElement = s => new ChooseField.LocalizedChoice("StringLocaleTable", s);
			interactAttackField.Mod(om.interactAttack);

			this.Query<KeybindField>().ForEach(ve => ve.Mod(om.keybind));
		}
	}
}
