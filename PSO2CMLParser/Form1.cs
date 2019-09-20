using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace PSO2CMLParser
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : Form
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public Form1()
		{
			this.InitializeComponent();
			CMLparser.tb = this.textbox_OutText;
			CMLparser.path = Directory.GetCurrentDirectory();
			LinkLabel.Link link = new LinkLabel.Link();
			link.LinkData = "http://psumods.co.uk/viewtopic.php?f=4&t=1207";
			this.link_releaseThread.Links.Add(link);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020A4 File Offset: 0x000002A4
		private void Form1_Load(object sender, EventArgs e)
		{
			this.comboBox1.Items.Add("English");
			this.comboBox1.Items.Add("日本語");
			this.comboBox1.SelectedIndex = 0;
			this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
			AppText.InitializeDictionaries();
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020FA File Offset: 0x000002FA
		private void button1_Click(object sender, EventArgs e)
		{
			if (this.loadCMLDialog.ShowDialog() == DialogResult.OK)
			{
				CMLparser.ParseCML(this.loadCMLDialog.FileName);
			}
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000211C File Offset: 0x0000031C
		private void button_MakeFile_Click(object sender, EventArgs e)
		{
			this.saveCCFileDialog.FileName = CMLparser.outFile + CMLparser.GetFileType();
			if (this.saveCCFileDialog.ShowDialog() == DialogResult.OK)
			{
				Console.WriteLine(this.saveCCFileDialog.FileName);
				CMLparser.WriteFileData(this.saveCCFileDialog.FileName);
				if (this.CB_compressCCFile.Checked && CMLparser.latestWrittenFile != "")
				{
					try
					{
						Process.Start("CharacterCrypt.exe", string.Concat(new string[]
						{
							"encrypt  \"",
							CMLparser.latestWrittenFile,
							"\" \"",
							CMLparser.latestWrittenFile,
							"\""
						}));
					}
					catch (Win32Exception ex)
					{
						this.textbox_OutText.AppendText(Environment.NewLine + ex.Message + Environment.NewLine + AppText.GetText("err_characrypt_notfound"));
					}
				}
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002210 File Offset: 0x00000410
		private void CB_outputParts_CheckedChanged(object sender, EventArgs e)
		{
			CMLparser.outputParts = this.CB_outputParts.Checked;
			CMLparser.ReloadCML();
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002227 File Offset: 0x00000427
		private void link_releaseThread_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(e.Link.LinkData as string);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002240 File Offset: 0x00000440
		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (this.comboBox1.SelectedItem.ToString() == "English")
			{
				this.ChangeLanguage("en");
				Form1._currentLanguage = Form1.CurrentLanguage.en;
			}
			else if (this.comboBox1.SelectedItem.ToString() == "日本語")
			{
				this.ChangeLanguage("ja-JP");
				Form1._currentLanguage = Form1.CurrentLanguage.jp;
			}
			CMLparser.ReloadCML();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000022B0 File Offset: 0x000004B0
		private void ChangeLanguage(string lang)
		{
			foreach (object obj in base.Controls)
			{
				Control control = (Control)obj;
				new ComponentResourceManager(typeof(Form1)).ApplyResources(control, control.Name, new CultureInfo(lang));
				control.Update();
			}
		}

		// Token: 0x04000001 RID: 1
		public static Form1.CurrentLanguage _currentLanguage;

		// Token: 0x02000008 RID: 8
		public enum CurrentLanguage
		{
			// Token: 0x04000064 RID: 100
			en,
			// Token: 0x04000065 RID: 101
			jp
		}
	}
}
