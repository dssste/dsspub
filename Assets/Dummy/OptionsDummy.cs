using dss.pub.options;
using UnityEngine;

namespace dss.pub.dummy{
	public class OptionsDummy: OptionsModel{
		public enum InteractType{interact_attack, interact_no_attack}

		public IOption<InteractType> interactAttack => _interact_attack;
		[SerializeField] private EnumEntry<InteractType> _interact_attack = new();

		protected override void Init(){
			_interact_attack.Init();
		}
	}
}
