namespace PSO2CMLParser
{
	// Token: 0x02000002 RID: 2
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		// Token: 0x06000009 RID: 9 RVA: 0x0000232C File Offset: 0x0000052C
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x0000234C File Offset: 0x0000054C
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager componentResourceManager = new global::System.ComponentModel.ComponentResourceManager(typeof(global::PSO2CMLParser.Form1));
			this.button_LoadCML = new global::System.Windows.Forms.Button();
			this.button_MakeFile = new global::System.Windows.Forms.Button();
			this.CB_outputParts = new global::System.Windows.Forms.CheckBox();
			this.textbox_OutText = new global::System.Windows.Forms.TextBox();
			this.loadCMLDialog = new global::System.Windows.Forms.OpenFileDialog();
			this.CB_compressCCFile = new global::System.Windows.Forms.CheckBox();
			this.saveCCFileDialog = new global::System.Windows.Forms.SaveFileDialog();
			this.label_VersionInfo = new global::System.Windows.Forms.Label();
			this.label1 = new global::System.Windows.Forms.Label();
			this.link_releaseThread = new global::System.Windows.Forms.LinkLabel();
			this.label2 = new global::System.Windows.Forms.Label();
			this.comboBox1 = new global::System.Windows.Forms.ComboBox();
			base.SuspendLayout();
			componentResourceManager.ApplyResources(this.button_LoadCML, "button_LoadCML");
			this.button_LoadCML.Name = "button_LoadCML";
			this.button_LoadCML.UseVisualStyleBackColor = true;
			this.button_LoadCML.Click += new global::System.EventHandler(this.button1_Click);
			componentResourceManager.ApplyResources(this.button_MakeFile, "button_MakeFile");
			this.button_MakeFile.Name = "button_MakeFile";
			this.button_MakeFile.UseVisualStyleBackColor = true;
			this.button_MakeFile.Click += new global::System.EventHandler(this.button_MakeFile_Click);
			componentResourceManager.ApplyResources(this.CB_outputParts, "CB_outputParts");
			this.CB_outputParts.Name = "CB_outputParts";
			this.CB_outputParts.UseVisualStyleBackColor = true;
			this.CB_outputParts.CheckedChanged += new global::System.EventHandler(this.CB_outputParts_CheckedChanged);
			componentResourceManager.ApplyResources(this.textbox_OutText, "textbox_OutText");
			this.textbox_OutText.Name = "textbox_OutText";
			this.loadCMLDialog.FileName = "openFileDialog1";
			componentResourceManager.ApplyResources(this.loadCMLDialog, "loadCMLDialog");
			componentResourceManager.ApplyResources(this.CB_compressCCFile, "CB_compressCCFile");
			this.CB_compressCCFile.Checked = true;
			this.CB_compressCCFile.CheckState = global::System.Windows.Forms.CheckState.Checked;
			this.CB_compressCCFile.Name = "CB_compressCCFile";
			this.CB_compressCCFile.UseVisualStyleBackColor = true;
			componentResourceManager.ApplyResources(this.saveCCFileDialog, "saveCCFileDialog");
			componentResourceManager.ApplyResources(this.label_VersionInfo, "label_VersionInfo");
			this.label_VersionInfo.Name = "label_VersionInfo";
			componentResourceManager.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			componentResourceManager.ApplyResources(this.link_releaseThread, "link_releaseThread");
			this.link_releaseThread.Name = "link_releaseThread";
			this.link_releaseThread.TabStop = true;
			this.link_releaseThread.LinkClicked += new global::System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_releaseThread_LinkClicked);
			componentResourceManager.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			componentResourceManager.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.SelectedIndexChanged += new global::System.EventHandler(this.comboBox1_SelectedIndexChanged);
			componentResourceManager.ApplyResources(this, "$this");
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.Controls.Add(this.comboBox1);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.link_releaseThread);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.label_VersionInfo);
			base.Controls.Add(this.CB_compressCCFile);
			base.Controls.Add(this.textbox_OutText);
			base.Controls.Add(this.CB_outputParts);
			base.Controls.Add(this.button_MakeFile);
			base.Controls.Add(this.button_LoadCML);
			base.Name = "Form1";
			base.Load += new global::System.EventHandler(this.Form1_Load);
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000002 RID: 2
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000003 RID: 3
		private global::System.Windows.Forms.Button button_LoadCML;

		// Token: 0x04000004 RID: 4
		private global::System.Windows.Forms.Button button_MakeFile;

		// Token: 0x04000005 RID: 5
		private global::System.Windows.Forms.CheckBox CB_outputParts;

		// Token: 0x04000006 RID: 6
		private global::System.Windows.Forms.TextBox textbox_OutText;

		// Token: 0x04000007 RID: 7
		private global::System.Windows.Forms.OpenFileDialog loadCMLDialog;

		// Token: 0x04000008 RID: 8
		private global::System.Windows.Forms.CheckBox CB_compressCCFile;

		// Token: 0x04000009 RID: 9
		private global::System.Windows.Forms.SaveFileDialog saveCCFileDialog;

		// Token: 0x0400000A RID: 10
		private global::System.Windows.Forms.Label label_VersionInfo;

		// Token: 0x0400000B RID: 11
		private global::System.Windows.Forms.Label label1;

		// Token: 0x0400000C RID: 12
		private global::System.Windows.Forms.LinkLabel link_releaseThread;

		// Token: 0x0400000D RID: 13
		private global::System.Windows.Forms.Label label2;

		// Token: 0x0400000E RID: 14
		private global::System.Windows.Forms.ComboBox comboBox1;
	}
}
