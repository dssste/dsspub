using dss.pub.options;
using UnityEngine;

namespace dss.pub.dummy{
	public class OptionsDummy: OptionsModel{
		public static OptionsDummy instance => GetInstance<OptionsDummy>();
		private OptionsDummy(){}

		public enum InteractType{interact_attack, interact_no_attack}

		public IOption<InteractType> interactAttack => _interact_attack;
		public IOption<string> locale => _locale;
		public IKeybind keybind => _keybind;

		[SerializeField] private EnumEntry<InteractType> _interact_attack = new();
		[SerializeField] private LocaleEntry _locale = new();
		[SerializeField] private Keybind _keybind = new();
	}
}
