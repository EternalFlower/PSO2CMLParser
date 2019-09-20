using System;
using System.Collections.Generic;
using PSO2CMLParser.Properties;

namespace PSO2CMLParser
{
	// Token: 0x02000004 RID: 4
	public static class AppText
	{
		// Token: 0x06000021 RID: 33 RVA: 0x00003D6C File Offset: 0x00001F6C
		public static string GetText(string key)
		{
			Form1.CurrentLanguage currentLanguage = Form1._currentLanguage;
			if (currentLanguage != Form1.CurrentLanguage.en)
			{
				if (currentLanguage != Form1.CurrentLanguage.jp)
				{
					goto IL_53;
				}
			}
			else
			{
				try
				{
					return AppText.EnglishText[key];
				}
				catch (KeyNotFoundException ex)
				{
					return ex.Message + " Key: " + key;
				}
			}
			try
			{
				return AppText.JapaneseText[key];
			}
			catch (KeyNotFoundException ex2)
			{
				return ex2.Message + " Key: " + key;
			}
			IL_53:
			return "Text entry not found";
		}

		// Token: 0x06000022 RID: 34 RVA: 0x00003DF0 File Offset: 0x00001FF0
		public static void InitializeDictionaries()
		{
			CMLparser.TextToDictionary(AppText.emb_text_en, AppText.EnglishText, true);
			CMLparser.TextToDictionary(AppText.emb_text_jp, AppText.JapaneseText, true);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00003E14 File Offset: 0x00002014
		public static string GetRace(int raceid)
		{
			string key;
			switch (raceid)
			{
			case 0:
				key = "race_human";
				break;
			case 1:
				key = "race_newman";
				break;
			case 2:
				key = "race_cast";
				break;
			case 3:
				key = "race_dewman";
				break;
			default:
				key = "err_unk";
				break;
			}
			return AppText.GetText(key);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00003E68 File Offset: 0x00002068
		public static string GetSex(int sexid)
		{
			string key;
			if (sexid != 0)
			{
				if (sexid == 1)
				{
					key = "sex_female";
				}
				else
				{
					key = "err_unk";
				}
			}
			else
			{
				key = "sex_male";
			}
			return AppText.GetText(key);
		}

		// Token: 0x0400005C RID: 92
		private static Dictionary<string, string> EnglishText = new Dictionary<string, string>();

		// Token: 0x0400005D RID: 93
		private static Dictionary<string, string> JapaneseText = new Dictionary<string, string>();

		// Token: 0x0400005E RID: 94
		private static string emb_text_en = Resources.text_en;

		// Token: 0x0400005F RID: 95
		private static string emb_text_jp = Resources.text_ja_JP;
	}
}
