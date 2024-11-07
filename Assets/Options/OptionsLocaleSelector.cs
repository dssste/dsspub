using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[Serializable, DisplayName("Options Locale Selector")]
public class OptionsLocaleSelector: IStartupLocaleSelector{
	public Locale GetStartupLocale(ILocalesProvider availableLocales){
		return availableLocales.GetLocale("ja-JP");
	}
}
