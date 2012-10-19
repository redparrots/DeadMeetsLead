using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Resources;
using System.Collections;

namespace SetupEmptyResXFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            foreach (string mainFile in lbMainFile.Items)
            {
                string path, fileName;
                if (!ValidMainFile(mainFile, out path, out fileName))
                {
                    lblResults.Text += "File doesn't exist!" + Environment.NewLine;
                    continue;
                }
                string[] extraLanguages = GetValidLanguageEntries();


                foreach (var lang in extraLanguages)
                {
                    string fullLangPath = Path.Combine(path, LanguifyFileName(fileName, lang));
                    string tempName = Path.GetTempFileName();
                    using (ResXResourceReader mainReader = new ResXResourceReader(mainFile))
                    {
                        using (ResXResourceWriter writer = new ResXResourceWriter(tempName))
                        {
                            if (File.Exists(fullLangPath))
                            {
                                using (ResXResourceSet langReader = new ResXResourceSet(fullLangPath))
                                {
                                    foreach (DictionaryEntry de in mainReader)
                                    {
                                        object o = langReader.GetObject((string)de.Key);
                                        if (o is String && !string.IsNullOrEmpty((string)o))
                                            writer.AddResource((string)de.Key, (string)o);
                                        else
                                            writer.AddResource((string)de.Key, "-");
                                    }
                                }
                            }
                            else
                            {
                                foreach (DictionaryEntry de in mainReader)
                                    writer.AddResource((string)de.Key, "-");
                            }
                        }
                    }
                    string newTempFile = "";
                    if (File.Exists(fullLangPath))
                    {
                        newTempFile = Path.GetTempFileName();
                        File.Delete(newTempFile);
                        File.Move(fullLangPath, newTempFile);
                    }
                    try { File.Move(tempName, fullLangPath); }
                    catch { if (newTempFile.Length > 0) File.Move(newTempFile, fullLangPath); }
                    if (File.Exists(newTempFile))
                        File.Delete(newTempFile);
                }
            }
        }

        private bool ValidMainFile(string mainFileFullPath, out string mainFilePath, out string mainFileName)
        {
            mainFilePath = Path.GetDirectoryName(mainFileFullPath);
            mainFileName = Path.GetFileName(mainFileFullPath);
            return File.Exists(mainFileFullPath) && mainFileFullPath.ToLower().EndsWith(".resx");
        }

        private string[] GetValidLanguageEntries()
        {
            List<String> entries = new List<String>();
            foreach (var l in tbExtraLanguages.Lines)
            {
                if (!string.IsNullOrEmpty(l) && l.Length == 2)
                    entries.Add(l);
            }
            return entries.ToArray();
        }

        private string LanguifyFileName(string name, string isoName)
        {
            if (isoName == "en")
                return string.Format("{0}.resx", System.IO.Path.GetFileNameWithoutExtension(name));
            return string.Format("{0}.{1}.resx", System.IO.Path.GetFileNameWithoutExtension(name), isoName);
        }

        private void lbMainFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            for (int i=0; i<files.Length; i++)
                lbMainFile.Items.Add(files[i]);
        }

        private void lbMainFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void lbMainFile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lbMainFile.SelectedIndex >= 0)
                lbMainFile.Items.RemoveAt(lbMainFile.SelectedIndex);
        }
    }
}
