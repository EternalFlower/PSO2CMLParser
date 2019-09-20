using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PSO2CMLParser.Properties;

namespace PSO2CMLParser
{
	// Token: 0x02000003 RID: 3
	internal class CMLparser
	{
		// Token: 0x0600000C RID: 12 RVA: 0x00002729 File Offset: 0x00000929
		public static void ReloadCML()
		{
			if (CMLparser.inFile != "")
			{
				CMLparser.ParseCML(CMLparser.inFile);
			}
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002748 File Offset: 0x00000948
		public static void ParseCML(string cmlFile)
		{
			CMLparser.inFile = cmlFile;
			CMLparser.outFile = Path.GetFileNameWithoutExtension(CMLparser.inFile);
			CMLparser.LoadDictionariesFromInternal();
			if (CMLparser.br != null)
			{
				CMLparser.br.Close();
			}
			try
			{
				FileInfo fileInfo = new FileInfo(CMLparser.inFile);
				Console.WriteLine("File size: " + fileInfo.Length + Environment.NewLine);
				if (fileInfo.Length > 1048576L)
				{
					throw new Exception();
				}
				CMLparser.br = new BinaryReader(new MemoryStream(File.ReadAllBytes(CMLparser.inFile)));
			}
			catch (IOException ex)
			{
				CMLparser.tb.Text = ex.Message + AppText.GetText("err_file_notfound");
				return;
			}
			catch (ArgumentException ex2)
			{
				CMLparser.tb.Text = ex2.Message + AppText.GetText("err_file_notfound");
				return;
			}
			catch (UnauthorizedAccessException ex3)
			{
				CMLparser.tb.Text = ex3.Message;
				return;
			}
			catch (Exception ex4)
			{
				CMLparser.tb.Text = ex4.Message;
				return;
			}
			CMLparser.tb.Text = CMLparser.inFile;
			CMLparser.FindFileData();
			CMLparser.GetBioData();
			CMLparser.tb.AppendText(Environment.NewLine + string.Format(AppText.GetText("log_bioinfo"), AppText.GetRace(CMLparser.race), AppText.GetSex(CMLparser.sex)));
			CMLparser.GetFIGRData();
			CMLparser.GetCOLRData();
			CMLparser.GetSLCTData();
			CMLparser.GetAccessoriesData();
			if (CMLparser.outputParts)
			{
				CMLparser.outputPartsInfo();
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000028F4 File Offset: 0x00000AF4
		public static string GetFileType()
		{
			string str = ".";
			int num = CMLparser.sex;
			if (num != 0)
			{
				if (num == 1)
				{
					str += "f";
				}
			}
			else
			{
				str += "m";
			}
			switch (CMLparser.race)
			{
			case 0:
				str += "h";
				break;
			case 1:
				str += "n";
				break;
			case 2:
				str += "c";
				break;
			case 3:
				str += "d";
				break;
			}
			return str + "p";
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002994 File Offset: 0x00000B94
		public static void WriteFileData(string filename)
		{
			string fileType = CMLparser.GetFileType();
			string str = Path.ChangeExtension(filename, null);
			if (CMLparser.FIGRdata == null || CMLparser.COLRdata == null || CMLparser.SLCTdata == null)
			{
				CMLparser.tb.AppendText(Environment.NewLine + AppText.GetText("err_file_couldnotsave_invalidcml"));
				CMLparser.latestWrittenFile = "";
				return;
			}
			try
			{
				CMLparser.bw = new BinaryWriter(new FileStream(str + fileType, FileMode.Create));
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message + AppText.GetText("err_file_couldnotsave_unknown"));
				return;
			}
			CMLparser.FIGRWriteOff = CMLparser.COLRPos - (CMLparser.FIGRPos + CMLparser.FIGRdata.Length * 4);
			CMLparser.COLRWriteOff = CMLparser.SLCTPos - (CMLparser.COLRPos + CMLparser.COLRdata.Length * 4);
			CMLparser.SLCTWriteOff = CMLparser.ACCPos - (CMLparser.SLCTPos + CMLparser.SLCTdata.Length * 4);
			byte[] buffer = new byte[CMLparser.FIGRWriteOff];
			byte[] buffer2 = new byte[CMLparser.COLRWriteOff];
			byte[] buffer3 = new byte[CMLparser.SLCTWriteOff];
			int value = 6;
			int value2 = 732;
			int value3 = 0;
			CMLparser.bw.Write(value);
			CMLparser.bw.Write(value2);
			CMLparser.bw.Write(value3);
			CMLparser.bw.Write(value3);
			CMLparser.bw.Write(CMLparser.race);
			CMLparser.bw.Write(CMLparser.sex);
			CMLparser.bw.Write(value3);
			foreach (int value4 in CMLparser.FIGRdata)
			{
				CMLparser.bw.Write(value4);
			}
			CMLparser.bw.Write(buffer);
			foreach (int value5 in CMLparser.COLRdata)
			{
				CMLparser.bw.Write(value5);
			}
			CMLparser.bw.Write(buffer2);
			foreach (int value6 in CMLparser.SLCTdata)
			{
				CMLparser.bw.Write(value6);
			}
			CMLparser.bw.Write(buffer3);
			foreach (byte value7 in CMLparser.AccData)
			{
				CMLparser.bw.Write(value7);
			}
			foreach (byte value8 in CMLparser.endFileData)
			{
				CMLparser.bw.Write(value8);
			}
			CMLparser.bw.Close();
			CMLparser.tb.AppendText(Environment.NewLine + string.Format(AppText.GetText("log_system_filesaved"), str + fileType));
			CMLparser.latestWrittenFile = str + fileType;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002C50 File Offset: 0x00000E50
		private static void FindFileData()
		{
			CMLparser.FindDataTag("cml");
			CMLparser.br.ReadByte();
			CMLparser.FileSizeTotal = CMLparser.br.ReadInt32();
			CMLparser.FileSize = CMLparser.br.ReadInt32();
			CMLparser.VTBFLoc = CMLparser.br.ReadInt32();
			if (CMLparser.showLog)
			{
				Console.WriteLine(string.Concat(new string[]
				{
					"File Size Total: ",
					CMLparser.FileSizeTotal.ToString(),
					"\nFileSize: ",
					CMLparser.FileSize.ToString(),
					"\nVTBFLoc: ",
					CMLparser.VTBFLoc.ToString()
				}));
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002CF8 File Offset: 0x00000EF8
		private static void GetBioData()
		{
			CMLparser.FindDataTag("DOC ");
			CMLparser.br.ReadInt32();
			CMLparser.br.ReadInt16();
			CMLparser.race = CMLparser.br.ReadInt32();
			CMLparser.br.ReadInt16();
			CMLparser.sex = CMLparser.br.ReadInt32();
			if (CMLparser.showLog)
			{
				Console.WriteLine("Race: " + CMLparser.race.ToString() + "\nSex: " + CMLparser.sex.ToString());
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002D7E File Offset: 0x00000F7E
		private static byte GetByteFromBCD(byte input)
		{
			return (byte)((input >> 4 & 1) * 10 + (int)(input & 15));
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002D90 File Offset: 0x00000F90
		private static void GetFIGRData()
		{
			if (!CMLparser.FindDataTag("FIGR"))
			{
				return;
			}
			CMLparser.br.ReadInt16();
			int b = (int)CMLparser.br.ReadByte();
			CMLparser.br.ReadByte();
			CMLparser.FIGRdata = new int[60];
			if (CMLparser.showLog)
			{
				Console.WriteLine("FIGR elements: " + b.ToString());
			}
			for (int i = 0; i < b; i++)
			{
				int num = (int)CMLparser.br.ReadByte();
				CMLparser.br.ReadBytes(2);
				int num2 = 0;
				for (int j = 0; j < 3; j++)
				{
					try
					{
						CMLparser.FIGRdata[i * 3 + num2] = CMLparser.br.ReadInt32();
					}
					catch (IndexOutOfRangeException)
					{
						CMLparser.tb.AppendText(Environment.NewLine + "Unhandled datatag: " + num);
					}
					num2++;
				}
			}
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 0, 13);
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 1, 14);
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 2, 15);
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 3, 16);
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 4, 17);
                CMLparser.DupeSliderData(CMLparser.FIGRdata, 11, 18);
			    CMLparser.DupeSliderData(CMLparser.FIGRdata, 12, 19);

			if (CMLparser.showLog)
			{
				int num3 = 0;
				foreach (int num4 in CMLparser.FIGRdata)
				{
					Console.WriteLine(Enum.GetName(typeof(CMLparser.FIGRtypes), num3 / 3) + " " + num4.ToString());
					num3++;
				}
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002F34 File Offset: 0x00001134
		private static void DupeSliderData(int[] intarr, int origin, int destination)
		{
			for (int i = 0; i < 3; i++)
			{
				intarr[destination * 3 + i] = intarr[origin * 3 + i];
				if (CMLparser.showDeepLog)
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"Origin ",
						i,
						" = ",
						intarr[origin + i]
					}));
				}
				if (CMLparser.showDeepLog)
				{
					Console.WriteLine(string.Concat(new object[]
					{
						"Dest ",
						i,
						" = ",
						intarr[destination + i]
					}));
				}
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002FDC File Offset: 0x000011DC
		private static void GetCOLRData()
		{
			if (!CMLparser.FindDataTag("COLR"))
			{
				return;
			}
			CMLparser.br.ReadInt16();
			int num = (int)CMLparser.br.ReadInt16();
			CMLparser.COLRdata = new int[num * 3];
			if (CMLparser.showLog)
			{
				Console.WriteLine("COLR elements: " + num.ToString());
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = (int)(CMLparser.br.ReadByte() - 32);
				int num3 = 0;
				if (CMLparser.showLog)
				{
					Console.WriteLine("datatag: " + num2.ToString());
				}
				CMLparser.br.ReadBytes(2);
				for (int j = 0; j < 3; j++)
				{
					CMLparser.COLRdata[num2 * 3 + num3] = CMLparser.br.ReadInt32();
					num3++;
				}
			}
			if (CMLparser.showLog)
			{
                int num3 = 0;
                foreach (int num4 in CMLparser.COLRdata)
				{
					Console.WriteLine(Enum.GetName(typeof(CMLparser.COLRtypes), num3 / 3) + " " + num4.ToString());
                    num3++;
				}
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x000030D8 File Offset: 0x000012D8
		private static void GetSLCTData()
		{
			if (!CMLparser.FindDataTag("SLCT"))
			{
				return;
			}
			CMLparser.br.ReadInt16();
			int num = (int)CMLparser.br.ReadInt16();
			if (num > 20)
			{
				CMLparser.SLCTdata = new int[num];
			}
			else
			{
				CMLparser.SLCTdata = new int[20];
			}
			if (CMLparser.showLog)
			{
				Console.WriteLine("SLCT elements: " + num.ToString());
			}
			for (int i = 0; i < num; i++)
			{
				int byteFromBCD = (int)CMLparser.GetByteFromBCD((byte)(CMLparser.br.ReadByte() - 64));
				if (CMLparser.showLog)
				{
					Console.WriteLine("datatag: " + byteFromBCD.ToString());
				}
				CMLparser.br.ReadByte();
				CMLparser.SLCTdata[byteFromBCD] = CMLparser.br.ReadInt32();
			}
			if (CMLparser.showLog)
			{

                int num3 = 0;
                foreach (int num2 in CMLparser.SLCTdata)
                {
                    Console.WriteLine(Enum.GetName(typeof(CMLparser.SLCTtypes), num3) + " " + num2.ToString());
                    num3++;
                }
            }
			if (CMLparser.SLCTdata[17] == 0)
			{
				CMLparser.SLCTdata[17] = 20001;
			}
			if (CMLparser.SLCTdata[18] == 0)
			{
				CMLparser.SLCTdata[18] = 20001;
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000031FC File Offset: 0x000013FC
		private static void GetAccessoriesData()
		{
			CMLparser.AccData = new byte[CMLparser.AccessoriesSize];
			for (int i = 0; i < CMLparser.AccData.Length; i++)
			{
				CMLparser.AccData[i] = 0;
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00003234 File Offset: 0x00001434
		private static bool FindDataTag(string tag)
		{
			int num = (int)CMLparser.br.BaseStream.Position;
			for (;;)
			{
				CMLparser.br.BaseStream.Seek((long)num, SeekOrigin.Begin);
				try
				{
                    string temp = new string(CMLparser.br.ReadChars(tag.Length));
                    if (temp == tag)
					{
						if (CMLparser.showLog)
						{
							Console.WriteLine("Tag " + tag + " found");
						}
						return true;
					}
				}
				catch (EndOfStreamException)
				{
					Console.WriteLine("Tag " + tag + " not found");
					return false;
				}
				catch (IOException ex)
				{
					Console.WriteLine("Unknown error occured: " + ex.Message);
					return false;
				}
				catch (ArgumentException ex)
				{
                    Console.WriteLine("Invalid String");
                    //CMLparser.tb.AppendText(Environment.NewLine + "Tag " + tag + " not found");
                    //return false;
                    num++;
                    continue;
				}
				if (CMLparser.br.BaseStream.Position == CMLparser.br.BaseStream.Length)
				{
					break;
				}
				num++;
			}
            Console.WriteLine("Tag " + tag + " not found");
            CMLparser.tb.AppendText(Environment.NewLine + "Tag " + tag + " not found");
			return false;
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000338C File Offset: 0x0000158C
		private static void LoadDictionaries()
		{
			CMLparser.TextToDictionary(CMLparser.accessoryFile, CMLparser.accessories);
			CMLparser.TextToDictionary(CMLparser.bodypaint1File, CMLparser.bodypaints);
			CMLparser.TextToDictionary(CMLparser.eyesFile, CMLparser.eyes);
			CMLparser.TextToDictionary(CMLparser.eyebrowFile, CMLparser.eyebrows);
			CMLparser.TextToDictionary(CMLparser.eyelashFile, CMLparser.eyelashes);
			CMLparser.TextToDictionary(CMLparser.facepaint2File, CMLparser.makeups);
			CMLparser.TextToDictionary(CMLparser.facevariationsFile, CMLparser.faces);
			CMLparser.TextToDictionary(CMLparser.hairstylesFile, CMLparser.hairstyles);
			CMLparser.TextToDictionary(CMLparser.bodypaint2File, CMLparser.stickers);
			CMLparser.TextToDictionary(CMLparser.legFile, CMLparser.legs);
			CMLparser.TextToDictionary(CMLparser.armFile, CMLparser.arms);
			CMLparser.TextToDictionary(CMLparser.costumeFile, CMLparser.costumes);
			CMLparser.TextToDictionary(CMLparser.bodyFile, CMLparser.bodyparts);
			CMLparser.TextToDictionary(CMLparser.baseFile, CMLparser.basewears);
			CMLparser.TextToDictionary(CMLparser.innerFile, CMLparser.innerwears);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x0000347C File Offset: 0x0000167C
		private static void LoadDictionariesFromInternal()
		{
			CMLparser.TextToDictionary(CMLparser.emb_accessoryFile, CMLparser.accessories, true);
			CMLparser.TextToDictionary(CMLparser.emb_bodypaint1File, CMLparser.bodypaints, true);
			CMLparser.TextToDictionary(CMLparser.emb_eyesFile, CMLparser.eyes, true);
			CMLparser.TextToDictionary(CMLparser.emb_eyebrowFile, CMLparser.eyebrows, true);
			CMLparser.TextToDictionary(CMLparser.emb_eyelashFile, CMLparser.eyelashes, true);
			CMLparser.TextToDictionary(CMLparser.emb_facepaint2File, CMLparser.makeups, true);
			CMLparser.TextToDictionary(CMLparser.emb_facevariationsFile, CMLparser.faces, true);
			CMLparser.TextToDictionary(CMLparser.emb_hairstylesFile, CMLparser.hairstyles, true);
			CMLparser.TextToDictionary(CMLparser.emb_bodypaint2File, CMLparser.stickers, true);
			CMLparser.TextToDictionary(CMLparser.emb_legFile, CMLparser.legs, true);
			CMLparser.TextToDictionary(CMLparser.emb_armFile, CMLparser.arms, true);
			CMLparser.TextToDictionary(CMLparser.emb_costumeFile, CMLparser.costumes, true);
			CMLparser.TextToDictionary(CMLparser.emb_bodyFile, CMLparser.bodyparts, true);
			CMLparser.TextToDictionary(CMLparser.emb_baseFile, CMLparser.basewears, true);
			CMLparser.TextToDictionary(CMLparser.emb_innerFile, CMLparser.innerwears, true);
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000357C File Offset: 0x0000177C
		public static void TextToDictionary(string textfile, Dictionary<string, string> dictionary)
		{
			string[] array = File.ReadAllLines(CMLparser.path + "\\" + textfile);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					','
				});
				array2[0].Trim();
				array2[1].Trim();
				dictionary[array2[0]] = array2[1];
			}
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000035E0 File Offset: 0x000017E0
		public static void TextToDictionary(string text, Dictionary<string, string> dictionary, bool internalfile)
		{
			string[] array = text.Split(new string[]
			{
				Environment.NewLine
			}, StringSplitOptions.None);
			Console.Write("Lines: " + array.Length);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string[] array3 = array2[i].Split(new char[]
				{
					','
				});
				if (array3.Length >= 2)
				{
					array3[0].Trim();
					array3[1].Trim();
					dictionary[array3[0]] = array3[1];
				}
			}
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003664 File Offset: 0x00001864
		private static void outputPartsInfo()
		{
			string partkey = CMLparser.SLCTdata[0].ToString("D5");
			string partkey2 = CMLparser.SLCTdata[0].ToString("D5");
			string partkey3 = CMLparser.SLCTdata[0].ToString("D5");
			string partkey4 = CMLparser.SLCTdata[14].ToString("D5");
			string partkey5 = CMLparser.SLCTdata[15].ToString("D5");
			string partkey6 = CMLparser.SLCTdata[17].ToString("D5");
			string partkey7 = CMLparser.SLCTdata[18].ToString("D5");
			string partkey8 = CMLparser.SLCTdata[1].ToString("D5");
			string partkey9 = CMLparser.SLCTdata[19].ToString("D5");
			string partkey10 = CMLparser.SLCTdata[2].ToString("D5");
			string partkey11 = CMLparser.SLCTdata[3].ToString("D5");
			string partkey12 = CMLparser.SLCTdata[3].ToString("D5");
			string partkey13 = CMLparser.SLCTdata[4].ToString("D5");
			string partkey14 = CMLparser.SLCTdata[5].ToString("D5");
			string partkey15 = CMLparser.SLCTdata[6].ToString("D5");
			string partkey16 = CMLparser.SLCTdata[8].ToString("D5");
			string partkey17 = CMLparser.SLCTdata[13].ToString("D5");
			string partkey18 = CMLparser.SLCTdata[9].ToString("D5");
			string partkey19 = CMLparser.SLCTdata[10].ToString("D5");
			string partkey20 = CMLparser.SLCTdata[11].ToString("D5");
			string partkey21 = CMLparser.SLCTdata[12].ToString("D5");
			string partkey22 = CMLparser.SLCTdata[16].ToString("D5");
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_costume"), partkey, CMLparser.costumes);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_outerwear"), partkey3, CMLparser.costumes);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_part"), partkey2, CMLparser.bodyparts);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_legpart"), partkey4, CMLparser.legs);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_armpart"), partkey5, CMLparser.arms);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_basewear"), partkey6, CMLparser.basewears);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_innerwear"), partkey7, CMLparser.innerwears);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_bodypaint1"), partkey8, CMLparser.bodypaints);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_bodypaint2"), partkey9, CMLparser.bodypaints);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_sticker"), partkey10, CMLparser.stickers);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_right_eye"), partkey11, CMLparser.eyes);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_eyes"), partkey12, CMLparser.eyes);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_eyebrows"), partkey13, CMLparser.eyebrows);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_eyelash"), partkey14, CMLparser.eyelashes);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_facetype"), partkey15, CMLparser.faces);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_makeup1"), partkey16, CMLparser.makeups);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_makeup2"), partkey17, CMLparser.makeups);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_hairstyle"), partkey18, CMLparser.hairstyles);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_accessory1"), partkey19, CMLparser.accessories);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_accessory2"), partkey20, CMLparser.accessories);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_accessory3"), partkey21, CMLparser.accessories);
			CMLparser.WriteOutPartInfo(AppText.GetText("log_partinfo_accessory4"), partkey22, CMLparser.accessories);
		}

		// Token: 0x0600001E RID: 30 RVA: 0x00003A54 File Offset: 0x00001C54
		private static void WriteOutPartInfo(string partname, string partkey, Dictionary<string, string> dictionary)
		{
			string text;
			if (dictionary.TryGetValue(partkey, out text))
			{
				CMLparser.tb.AppendText(string.Concat(new string[]
				{
					Environment.NewLine,
					partname,
					" ",
					dictionary[partkey],
					"\t ID:  ",
					partkey
				}));
				return;
			}
			CMLparser.tb.AppendText(Environment.NewLine + AppText.GetText("log_partinfo_unknown_name") + partname + partkey);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00003AD4 File Offset: 0x00001CD4
		// Note: this type is marked as 'beforefieldinit'.
		static CMLparser()
		{
			byte[] array = new byte[22];
			array[0] = 2;
			array[2] = 1;
			CMLparser.endFileData = array;
			CMLparser.accessoryFile = "parts_acc.txt";
			CMLparser.bodypaint1File = "parts_bodypaint1.txt";
			CMLparser.eyesFile = "parts_eyes.txt";
			CMLparser.eyebrowFile = "parts_eyebrows.txt";
			CMLparser.eyelashFile = "parts_eyelashes.txt";
			CMLparser.facepaint2File = "parts_facepaint2.txt";
			CMLparser.facepaint1File = "parts_facepaint1.txt";
			CMLparser.facevariationsFile = "parts_facevariation.txt";
			CMLparser.hairstylesFile = "parts_hairstyles.txt";
			CMLparser.bodypaint2File = "parts_boypaint2.txt";
			CMLparser.faceFile = "parts_faces.txt";
			CMLparser.legFile = "parts_legs.txt";
			CMLparser.armFile = "parts_arms.txt";
			CMLparser.costumeFile = "parts_costume.txt";
			CMLparser.bodyFile = "parts_body.txt";
			CMLparser.baseFile = "parts_base.txt";
			CMLparser.innerFile = "parts_inner.txt";
			CMLparser.emb_accessoryFile = Resources.parts_acc;
			CMLparser.emb_bodypaint1File = Resources.parts_bodypaint1;
			CMLparser.emb_eyesFile = Resources.parts_eyes;
			CMLparser.emb_eyebrowFile = Resources.parts_eyebrows;
			CMLparser.emb_eyelashFile = Resources.parts_eyelashes;
			CMLparser.emb_facepaint2File = Resources.parts_facepaint2;
			CMLparser.emb_facepaint1File = Resources.parts_facepaint1;
			CMLparser.emb_facevariationsFile = Resources.parts_facevariation;
			CMLparser.emb_hairstylesFile = Resources.parts_hairstyles;
			CMLparser.emb_bodypaint2File = Resources.parts_boypaint2;
			CMLparser.emb_faceFile = Resources.parts_faces;
			CMLparser.emb_legFile = Resources.parts_legs;
			CMLparser.emb_armFile = Resources.parts_arms;
			CMLparser.emb_costumeFile = Resources.parts_costume;
			CMLparser.emb_bodyFile = Resources.parts_body;
			CMLparser.emb_baseFile = Resources.parts_base;
			CMLparser.emb_innerFile = Resources.parts_inner;
			CMLparser.accessories = new Dictionary<string, string>();
			CMLparser.bodypaints = new Dictionary<string, string>();
			CMLparser.stickers = new Dictionary<string, string>();
			CMLparser.eyes = new Dictionary<string, string>();
			CMLparser.eyebrows = new Dictionary<string, string>();
			CMLparser.eyelashes = new Dictionary<string, string>();
			CMLparser.makeups = new Dictionary<string, string>();
			CMLparser.hairstyles = new Dictionary<string, string>();
			CMLparser.faces = new Dictionary<string, string>();
			CMLparser.costumes = new Dictionary<string, string>();
			CMLparser.bodyparts = new Dictionary<string, string>();
			CMLparser.legs = new Dictionary<string, string>();
			CMLparser.arms = new Dictionary<string, string>();
			CMLparser.innerwears = new Dictionary<string, string>();
			CMLparser.basewears = new Dictionary<string, string>();
			CMLparser.showLog = true;
			CMLparser.showDeepLog = false;
		}

		// Token: 0x0400000F RID: 15
		private static BinaryReader br;

		// Token: 0x04000010 RID: 16
		private static BinaryWriter bw;

		// Token: 0x04000011 RID: 17
		public static bool outputParts = false;

		// Token: 0x04000012 RID: 18
		public static string path = "";

		// Token: 0x04000013 RID: 19
		public static string outFile = "";

		// Token: 0x04000014 RID: 20
		public static string inFile = "";

		// Token: 0x04000015 RID: 21
		public static string latestWrittenFile = "";

		// Token: 0x04000016 RID: 22
		private static int FileSizeTotal = 0;

		// Token: 0x04000017 RID: 23
		private static int FileSize = 0;

		// Token: 0x04000018 RID: 24
		private static int VTBFLoc = 0;

		// Token: 0x04000019 RID: 25
		private static int AccessoriesSize = 18;

		// Token: 0x0400001A RID: 26
		private static int FIGRPos = 28;

		// Token: 0x0400001B RID: 27
		private static int COLRPos = 376;

		// Token: 0x0400001C RID: 28
		private static int SLCTPos = 580;

		// Token: 0x0400001D RID: 29
		private static int ACCPos = 708;

		// Token: 0x0400001E RID: 30
		private static int FIGRWriteOff = 0;

		// Token: 0x0400001F RID: 31
		private static int COLRWriteOff = 0;

		// Token: 0x04000020 RID: 32
		private static int SLCTWriteOff = 0;

		// Token: 0x04000021 RID: 33
		private static byte[] endFileData;

		// Token: 0x04000022 RID: 34
		public static TextBox tb;

		// Token: 0x04000023 RID: 35
		private static string accessoryFile;

		// Token: 0x04000024 RID: 36
		private static string bodypaint1File;

		// Token: 0x04000025 RID: 37
		private static string eyesFile;

		// Token: 0x04000026 RID: 38
		private static string eyebrowFile;

		// Token: 0x04000027 RID: 39
		private static string eyelashFile;

		// Token: 0x04000028 RID: 40
		private static string facepaint2File;

		// Token: 0x04000029 RID: 41
		private static string facepaint1File;

		// Token: 0x0400002A RID: 42
		private static string facevariationsFile;

		// Token: 0x0400002B RID: 43
		private static string hairstylesFile;

		// Token: 0x0400002C RID: 44
		private static string bodypaint2File;

		// Token: 0x0400002D RID: 45
		private static string faceFile;

		// Token: 0x0400002E RID: 46
		private static string legFile;

		// Token: 0x0400002F RID: 47
		private static string armFile;

		// Token: 0x04000030 RID: 48
		private static string costumeFile;

		// Token: 0x04000031 RID: 49
		private static string bodyFile;

		// Token: 0x04000032 RID: 50
		private static string baseFile;

		// Token: 0x04000033 RID: 51
		private static string innerFile;

		// Token: 0x04000034 RID: 52
		private static string emb_accessoryFile;

		// Token: 0x04000035 RID: 53
		private static string emb_bodypaint1File;

		// Token: 0x04000036 RID: 54
		private static string emb_eyesFile;

		// Token: 0x04000037 RID: 55
		private static string emb_eyebrowFile;

		// Token: 0x04000038 RID: 56
		private static string emb_eyelashFile;

		// Token: 0x04000039 RID: 57
		private static string emb_facepaint2File;

		// Token: 0x0400003A RID: 58
		private static string emb_facepaint1File;

		// Token: 0x0400003B RID: 59
		private static string emb_facevariationsFile;

		// Token: 0x0400003C RID: 60
		private static string emb_hairstylesFile;

		// Token: 0x0400003D RID: 61
		private static string emb_bodypaint2File;

		// Token: 0x0400003E RID: 62
		private static string emb_faceFile;

		// Token: 0x0400003F RID: 63
		private static string emb_legFile;

		// Token: 0x04000040 RID: 64
		private static string emb_armFile;

		// Token: 0x04000041 RID: 65
		private static string emb_costumeFile;

		// Token: 0x04000042 RID: 66
		private static string emb_bodyFile;

		// Token: 0x04000043 RID: 67
		private static string emb_baseFile;

		// Token: 0x04000044 RID: 68
		private static string emb_innerFile;

		// Token: 0x04000045 RID: 69
		private static Dictionary<string, string> accessories;

		// Token: 0x04000046 RID: 70
		private static Dictionary<string, string> bodypaints;

		// Token: 0x04000047 RID: 71
		private static Dictionary<string, string> stickers;

		// Token: 0x04000048 RID: 72
		private static Dictionary<string, string> eyes;

		// Token: 0x04000049 RID: 73
		private static Dictionary<string, string> eyebrows;

		// Token: 0x0400004A RID: 74
		private static Dictionary<string, string> eyelashes;

		// Token: 0x0400004B RID: 75
		private static Dictionary<string, string> makeups;

		// Token: 0x0400004C RID: 76
		private static Dictionary<string, string> hairstyles;

		// Token: 0x0400004D RID: 77
		private static Dictionary<string, string> faces;

		// Token: 0x0400004E RID: 78
		private static Dictionary<string, string> costumes;

		// Token: 0x0400004F RID: 79
		private static Dictionary<string, string> bodyparts;

		// Token: 0x04000050 RID: 80
		private static Dictionary<string, string> legs;

		// Token: 0x04000051 RID: 81
		private static Dictionary<string, string> arms;

		// Token: 0x04000052 RID: 82
		private static Dictionary<string, string> innerwears;

		// Token: 0x04000053 RID: 83
		private static Dictionary<string, string> basewears;

		// Token: 0x04000054 RID: 84
		private static int race;

		// Token: 0x04000055 RID: 85
		private static int sex;

		// Token: 0x04000056 RID: 86
		private static int[] FIGRdata;

		// Token: 0x04000057 RID: 87
		private static int[] COLRdata;

		// Token: 0x04000058 RID: 88
		private static int[] SLCTdata;

		// Token: 0x04000059 RID: 89
		private static byte[] AccData;

		// Token: 0x0400005A RID: 90
		private static bool showLog;

		// Token: 0x0400005B RID: 91
		private static bool showDeepLog;

		// Token: 0x02000009 RID: 9
		public enum Races
		{
			// Token: 0x04000067 RID: 103
			Human,
			// Token: 0x04000068 RID: 104
			Newman,
			// Token: 0x04000069 RID: 105
			Cast,
			// Token: 0x0400006A RID: 106
			Dewman
		}

		// Token: 0x0200000A RID: 10
		public enum Sexes
		{
			// Token: 0x0400006C RID: 108
			Male,
			// Token: 0x0400006D RID: 109
			Female
		}

		// Token: 0x0200000B RID: 11
		public enum FIGRtypes
		{
			// Token: 0x0400006F RID: 111
			MainBody,
			// Token: 0x04000070 RID: 112
			Arm,
			// Token: 0x04000071 RID: 113
			Leg,
			// Token: 0x04000072 RID: 114
			Bust,
			// Token: 0x04000073 RID: 115
			Unknown1,
			// Token: 0x04000074 RID: 116
			FaceShape,
			// Token: 0x04000075 RID: 117
			EyeShape,
			// Token: 0x04000076 RID: 118
			NoseHeight,
			// Token: 0x04000077 RID: 119
			NoseShape,
			// Token: 0x04000078 RID: 120
			Mouth,
			// Token: 0x04000079 RID: 121
			Ears,
			// Token: 0x0400007A RID: 122
			Horns = 10,
			// Token: 0x0400007B RID: 123
			Neck,
			// Token: 0x0400007C RID: 124
			Waist,
			// Token: 0x0400007D RID: 125
			MainBody2,
			// Token: 0x0400007E RID: 126
			Arm2,
			// Token: 0x0400007F RID: 127
			Leg2,
			// Token: 0x04000080 RID: 128
			Bust2,
			// Token: 0x04000081 RID: 129
			Unknown2,
			// Token: 0x04000082 RID: 130
			Neck2,
			// Token: 0x04000083 RID: 131
			Waist2,
			// Token: 0x04000084 RID: 132
			Unknown3,
			// Token: 0x04000085 RID: 133
			Unknown4
		}

		// Token: 0x0200000C RID: 12
		public enum COLRtypes
		{
			// Token: 0x04000087 RID: 135
			Costume,
			// Token: 0x04000088 RID: 136
			Main,
			// Token: 0x04000089 RID: 137
			Sub1,
			// Token: 0x0400008A RID: 138
			Skin,
			// Token: 0x0400008B RID: 139
			Sub2 = 3,
			// Token: 0x0400008C RID: 140
			LeftEye,
			// Token: 0x0400008D RID: 141
			Sub3 = 4,
			// Token: 0x0400008E RID: 142
			RightEye,
			// Token: 0x0400008F RID: 143
			Eyes = 5,
			// Token: 0x04000090 RID: 144
			Hair,
			// Token: 0x04000091 RID: 145
			Unknown
		}

		// Token: 0x0200000D RID: 13
		public enum SLCTtypes
		{
			// Token: 0x04000093 RID: 147
			Costume,
			// Token: 0x04000094 RID: 148
			OuterWear = 0,
			// Token: 0x04000095 RID: 149
			BodyPart = 0,
			// Token: 0x04000096 RID: 150
			BodyPaint,
			// Token: 0x04000097 RID: 151
			Sticker,
			// Token: 0x04000098 RID: 152
			RightEye,
			// Token: 0x04000099 RID: 153
			Eyes = 3,
			// Token: 0x0400009A RID: 154
			EyeBrow,
			// Token: 0x0400009B RID: 155
			EyeLash,
			// Token: 0x0400009C RID: 156
			FaceType,
			// Token: 0x0400009D RID: 157
			Unknown1,
			// Token: 0x0400009E RID: 158
			MakeUp1,
			// Token: 0x0400009F RID: 159
			HairStyle,
			// Token: 0x040000A0 RID: 160
			Acc1,
			// Token: 0x040000A1 RID: 161
			Acc2,
			// Token: 0x040000A2 RID: 162
			Acc3,
			// Token: 0x040000A3 RID: 163
			MakeUp2,
			// Token: 0x040000A4 RID: 164
			LegPart,
			// Token: 0x040000A5 RID: 165
			ArmPart,
			// Token: 0x040000A6 RID: 166
			Acc4,
			// Token: 0x040000A7 RID: 167
			BaseWear,
			// Token: 0x040000A8 RID: 168
			InnerWear,
			// Token: 0x040000A9 RID: 169
			BodyPaint2
		}
	}
}
