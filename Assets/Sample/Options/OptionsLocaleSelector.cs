using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace dss.pub.dummy{
	[Serializable, DisplayName("Options Locale Selector")]
	public class OptionsLocaleSelector: IStartupLocaleSelector{
		public Locale GetStartupLocale(ILocalesProvider availableLocales){
			return availableLocales.GetLocale(OptionsDummy.instance.locale.value);
		}
	}
}
