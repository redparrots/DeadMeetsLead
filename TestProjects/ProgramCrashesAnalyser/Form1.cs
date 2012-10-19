using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;

namespace ProgramCrashesAnalyser
{
    public partial class Form1 : Form
    {
        private void InitGrid(DataGridView d)
        {
            //d.AutoSize = true;
            d.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            d.ColumnCount = 5;
            d.Columns[0].Name = "Date";
            d.Columns[1].Name = "Exception";
            d.Columns[2].Name = "Cause";
            d.Columns[3].Name = "Known Location";
            d.Columns[4].Name = "Direct Location";
        }

        public Form1()
        {
            WindowState = FormWindowState.Maximized;

            InitializeComponent();
            Update.Location = new Point(Width, Height);
            InitGrid(dataGridView1);
            InitGrid(dataGridView2);
        }

        string[] knownProjects = new string[] { "Client", "Graphics", "Common", "JMOD" };

        public class ExceptionEntry
        {
            private string exception;
            public string Exception { get { return exception; } set { exception = value; } }

            private string directLocation;
            public string DirectLocation { get { return directLocation; } set { directLocation = value; } }

            private string knownLocation;
            public string KnownLocation { get { return knownLocation; } set { knownLocation = value; } }
            
            private string cause;
            public string Cause { get { return cause; } set { cause = value; } }

            private string date;
            public string Date { get { return date; } set { date = value; } }
        }

        List<ExceptionEntry> entries = new List<ExceptionEntry>();

        private string GetException(string line)
        {
            string[] components = line.Split(' ');

            return components[1].Remove(components[1].Length - 1, 1);
        }

        private string GetCause(string line)
        {
            string[] components = line.Split(' ');

            string result = "";

            for (int i = 2; i < components.Length; i++)
                result += " " + components[i];

            return result.Remove(0, 1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] directories = Directory.GetDirectories(".");

            foreach (string dir in directories)
            {
                if (!dir.Contains("svn")&& !dir.Contains("Logs"))
                {
                    string[] zipFiles = Directory.GetFiles(dir);
                    foreach (string fileName in zipFiles)
                    {
                        var zf = new ZipFile(fileName);
                        
                        if (zf.ContainsEntry("Logs/ApplicationLog.txt"))
                        {
                            if (Directory.Exists("Logs"))
                                Directory.Delete("Logs", true);

                            zf["Logs/ApplicationLog.txt"].Extract(".");
                            StreamReader reader;
                            if (zf.ContainsEntry("Logs/SoundException.txt"))
                            {
                                zf["Logs/SoundException.txt"].Extract(".");
                                reader = new StreamReader("Logs/SoundException.txt");
                            }
                            else
                                reader = new StreamReader("Logs/ApplicationLog.txt");

                            string line = "";

                            ExceptionEntry entry = new ExceptionEntry();

                            entry.Date = dir.ToString();
                            entry.Date = entry.Date.Remove(0, 2);
                            string year = entry.Date.Substring(0, 4);
                            string month = entry.Date.Substring(4, 2);
                            string day = entry.Date.Substring(6, 2);
                            entry.Date = year + "-" + month + "-" + day;

                            if (zf.ContainsEntry("Logs/SoundException.txt"))
                            {
                                bool first = true;
                                string firstLine = "";
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (first)
                                    {
                                        firstLine = line;
                                        first = false;
                                    }
                                    if (line.Contains("Exception") || line.Contains("INVALID") || line.Contains("reference"))
                                    {
                                        while (line.StartsWith(" "))
                                            line = line.Remove(0, 1);

                                        entry.Exception = GetException(line);
                                        entry.Cause = GetCause(line);

                                        bool foundStartTrace = false;
                                        int index = 4;

                                        while((line = reader.ReadLine()) != null)
                                        {
                                            if ((line.Contains("   ") && !foundStartTrace))
                                            {
                                                foundStartTrace = true;
                                                line = line.Remove(0, 5);
                                                entry.DirectLocation = line;
                                            }
                                            else if (foundStartTrace && (line.Contains(knownProjects[0]) || line.Contains(knownProjects[1]) || line.Contains(knownProjects[2]) || line.Contains(knownProjects[3])))
                                            {
                                                line = line.Remove(0, 5);
                                                entry.KnownLocation = line;
                                                break;
                                            }
                                        }
                                    }
                                    if (entry.Exception == null)
                                    {
                                        entry.Exception = firstLine;
                                        entry.Cause = "All info in exception field";
                                        entry.DirectLocation = "All info in exception field";
                                        entry.KnownLocation = "All info in exception field";
                                    }
                                }
                            }
                            else
                            {
                                while ((line = reader.ReadLine()) != null)
                                {
                                    if (line.Contains("Exception"))
                                    {
                                        while (line.StartsWith(" "))
                                            line = line.Remove(0, 1);

                                        entry.Exception = GetException(line);
                                        entry.Cause = GetCause(line);

                                        bool foundStartTrace = false;

                                        int index = 4;

                                        while ((line = reader.ReadLine()) != null)
                                        {
                                            //if (((line.Contains(" at ") || line.Contains(" en ") || line.Contains(" bei ") || line.Contains(" bij ") || line.Contains(" в ") || line.Contains(" w ") || line.Contains(" konum:") || line.Contains(" vid ") || line.Contains(" 在 ") || line.Contains(" v ") || line.Contains(" a ")) && !foundStartTrace))
                                            if(line.Contains("   ") && !foundStartTrace)
                                            {
                                                foundStartTrace = true;
                                                line = line.Remove(0, 5);
                                                entry.DirectLocation = line;
                                            }
                                            else if (foundStartTrace && (line.Contains(knownProjects[0]) || line.Contains(knownProjects[1]) || line.Contains(knownProjects[2]) || line.Contains(knownProjects[3])))
                                            {
                                                line = line.Remove(0, 5);
                                                entry.KnownLocation = line;
                                                break;
                                            }
                                        }

                                        break;
                                    }
                                }
                            }

                            reader.Close();
                            
                            dataGridView1.Rows.Add(new string[] { entry.Date, entry.Exception, entry.Cause, entry.KnownLocation, entry.DirectLocation });
                            entries.Add(entry);

                            //dataGridView1.Columns.Add(new DataGridViewColumn { a });
                            //propertyGrid1.SelectedObject = new List<ExceptionEntry>();
                            Directory.Delete("./Logs", true);

                            
                        }
                    }
                }
            }
        }
    }
}
