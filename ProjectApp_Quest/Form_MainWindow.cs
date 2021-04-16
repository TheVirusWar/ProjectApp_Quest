using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

//app created for the purpose of being used as a job application assignment
//created by Božidar Miku

namespace ProjectApp_Quest
{
    public partial class Form_MainWindow : Form
    {
        //initializing resources that will be used throughout the entire applications
        List<string> recentProjects = new List<string>();
        ProjectFile fi;
        XmlSerializer xs;
        public int recentCount = 5; //default value as specified

        public Form_MainWindow()
        {
            InitializeComponent();
            xs = new XmlSerializer(typeof(ProjectFile));
            LoadProject(); //loading project, which has default parameter preset
            SaveLoadRecent(); //without a integer passed, the method only loads recent files

        }

        //all the "tools" are grouped in this area
        void SaveLoadRecent(int sw = 0)
        {
            //summary: checks if it should save and load or just load the file where recent projects are tracked in,
            //if it finds one it loads the information in it populates the toolstripmenu
            if (sw != 0)
            {
                //after a check if the method should save as well, it does so
                recentProjects.Add(recentCount.ToString());
                File.WriteAllLines("recent.txt", recentProjects.ToArray());

                recentProjects.Clear();

            }
            if (File.Exists("recent.txt"))
            {
                //file is looked for, and if it does exist, its loaded
                string [] lines = File.ReadAllLines("recent.txt");
                foreach (string line in lines)
                {
                    recentProjects.Add(line);
                }
            }
            else
            {
                return;
            }

            //dynamically removing the counter information from the loaded file
            int.TryParse(recentProjects[recentProjects.Count-1], out recentCount);
            recentProjects.RemoveAt(recentProjects.Count-1);
            //reversing the array, which makes most recent files appear on top
            recentProjects.Reverse();
            if(recentProjects.Count > recentCount)
            {
                //if the recent files tracked are greated than needed, theyre removed
                recentProjects.RemoveRange(recentCount, recentProjects.Count - recentCount);
            }
            int counter = 0;
            //adding items to the toolstripmenu via a loop
            ToolstripRecentFiles.DropDownItems.Clear();
            foreach (string str in recentProjects)
            {
                ToolStripMenuItem recent = new ToolStripMenuItem()
                {
                    Name = str,
                    Text = (counter++ + 1).ToString() + ". " + str
                };
                ToolstripRecentFiles.DropDownItems.Add(recent);
                recent.Click += new EventHandler(RecentItem_Click);
            }
            //reversing because the array is used elsewhere and List.Add() method puts the object at the end, this makes it so the array doesnt have objects that are not properly sorted
            recentProjects.Reverse();
        }
        void Clear()
        {
            //summary: clears the fields and the ProjectFile object
            textBoxName.Clear();
            textBoxInput.Clear();
            textBoxOutput.Clear();
            richTextBoxContent.Clear();
            dateTimePickerStart.Format = DateTimePickerFormat.Custom;
            dateTimePickerStart.CustomFormat = " ";
            dateTimePickerEnd.Format = DateTimePickerFormat.Custom;
            dateTimePickerEnd.CustomFormat = " ";
            fi = new ProjectFile();
        }
        bool Validation()
        {
            //summary: goes through all the required input boxes and checks if the necessery ones are filled with proper data, if not, returns false
            //also sets the color of labels so the user knows which one to fill
            bool vBool = true;
            if (textBoxName.Text == "")
            {
                labelName.ForeColor = Color.Red;
                vBool = false;
            }
            if (textBoxInput.Text == "")
            {
                labelInput.ForeColor = Color.Red;
                vBool = false;
            }
            if(textBoxOutput.Text == "")
            {
                labelOutput.ForeColor = Color.Red;
                vBool = false;
            }
            if (dateTimePickerStart.Format == DateTimePickerFormat.Custom)
            {
                labelStartDate.ForeColor = Color.Red;
                vBool = false;
            }
            if (dateTimePickerEnd.Format == DateTimePickerFormat.Custom)
            {
                labelEndDate.ForeColor = Color.Red;
                vBool = false;
            }
            if (richTextBoxContent.Text == "")
            {
                labelPreview.ForeColor = Color.Red;
                vBool = false;
            }
            if(dateTimePickerStart.Value > dateTimePickerEnd.Value)
            {
                MessageBox.Show("Start date cannot be greater than end date.");
                labelStartDate.ForeColor = Color.Red;
                labelEndDate.ForeColor = Color.Red;
                return false;
            }
            return vBool;
        }
        void GenerateDefaultCfg()
        {
            //summary: if the DefaultCfg is not found, generates one and saves it
            FileStream fs = new FileStream("Default.cfg", FileMode.Create, FileAccess.Write);
            fi = new ProjectFile
            {
                ProjectName = "default name",
                ProjectPath = @"Default.cfg",
                ProjectFileName = "Default.cfg",
                PathInput = @".\projects\",
                PathOutput = @".\projects\",
                Extension = ".cfg",
                Content = "default",
                Startd = DateTime.Parse("Jan 1, 2020"),
                Endd = DateTime.Parse("Jan 2, 2020")
            };

            xs.Serialize(fs, fi);
            fs.Close();
            fi = new ProjectFile();
        }
        List<ProjectFile> DirSearch(string dir)
        {
            //summary: searches into files and folders in where the program is located,
            //and loads found projects while checking if required parameters are correct (file size, case sensitivity...)
            //NOTE: the creator understood the requested 'search' functionality in this way and thus made it like that
            List<ProjectFile> prjFiles = new List<ProjectFile>();
            string[] allFiles = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);

            foreach (string p in allFiles)
            {
                FileStream fs = new FileStream(p, FileMode.Open, FileAccess.Read);
                if(textBoxMaxSize.Text != string.Empty)
                {
                    if (int.TryParse(textBoxMaxSize.Text, out int size) == false)
                    {
                        textBoxMaxSize.Text = "";
                    }
                    else 
                    {
                        if (fs.Length > size * 1024)
                            continue;
                    }
                }
                try
                {
                    ProjectFile _pf = (ProjectFile)xs.Deserialize(fs);
                    if (_pf.ProjectName == "default name")
                        continue;
                    prjFiles.Add(_pf);
                }
                catch (Exception)
                {

                }
                fs.Close();
            }
            return prjFiles;
        }

        //project editing is grouped here
        void LoadProject(string path = "Default.cfg")
        {
            //summary: loads the project by the path information, if none is given, it uses the default one, which is only at startup
            //it also adds the file to the most recent one
            if (!File.Exists(path))
            {
                GenerateDefaultCfg();
            }
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            try
            {
                fi = (ProjectFile)xs.Deserialize(fs);
                //shows a message if the file is unsupported
            }
            catch (Exception)
            {
                MessageBox.Show("Chosen file is not a supported Project file!");
                return;
                
            }
            LoadGUI(fi);
            fs.Close();
            if (fi.ProjectPath != "Default.cfg")
            {
                recentProjects.Add(fi.ProjectPath);
                SaveLoadRecent(1);
            }
        }
        void SaveProject(ProjectFile fiIn, string path)
        {
            //summary: saves the file on disk
            FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            xs.Serialize(fs, fiIn);
            fs.Close();
        }
        void LoadGUI(ProjectFile fiIn)
        {
            //summary: fills in all the data from the ProjectFile object and makes it ready to be viewed and edited
            textBoxName.Text = fiIn.ProjectName;
            textBoxInput.Text = fiIn.PathInput;
            textBoxOutput.Text = fiIn.PathOutput;
            richTextBoxContent.Text = fiIn.Content;

            if (fiIn.Startd <= fiIn.Endd)
            {
                if (dateTimePickerStart.Format == DateTimePickerFormat.Custom)
                {
                    dateTimePickerStart.Format = DateTimePickerFormat.Long;
                    dateTimePickerStart.Value = fiIn.Startd;
                    labelStartDate.ForeColor = Color.Black;
                }
                else
                {
                    dateTimePickerStart.Value = fiIn.Startd;
                    labelStartDate.ForeColor = Color.Black;
                }
                if (dateTimePickerEnd.Format == DateTimePickerFormat.Custom)
                {
                    dateTimePickerEnd.Format = DateTimePickerFormat.Long;
                    dateTimePickerEnd.Value = fiIn.Endd;
                    labelEndDate.ForeColor = Color.Black;
                }
                else
                {
                    dateTimePickerEnd.Value = fiIn.Endd;
                    labelEndDate.ForeColor = Color.Black;
                }
            }
            else
            {
                MessageBox.Show("Dates are incorrect");
            }
        }

        //all the button_clicked events
        private void New_Clicked(object sender, EventArgs e)
        {
            //summary: clears the GUI and the file
            fi = new ProjectFile();

            Clear();
        }
        private void Load_Clicked(object sender, EventArgs e)
        {
            //summary: opens a file dialog to choose the file and load the said file if it can, which is checked by the LoadProject() method
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "All files (*.*)|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadProject(ofd.FileName);
            }
            ofd.Dispose();
        }
        private void Save_Clicked(object sender, EventArgs e)
        {
            //summary: saves the current open file, if its a default one, shows an error.
            //populates the ProjectFile object and saves it to the open files location
            //if the currently open file is not saved, shows a savefile dialog, similarly as the Save_As
            if (fi.ProjectPath == "Default.cfg")
            {
                MessageBox.Show("Cannot edit Default file");
                return;
            }
            if (!Validation())
            {
                MessageBox.Show("All marked fields must be filled.");
                return;
            }
            fi.ProjectName = textBoxName.Text;
            fi.PathInput = textBoxInput.Text;
            fi.PathOutput = textBoxOutput.Text;
            fi.Startd = dateTimePickerStart.Value;
            fi.Endd = dateTimePickerEnd.Value;
            fi.Content = richTextBoxContent.Text;
            if (fi.ProjectPath != string.Empty && fi.ProjectPath != null)
            {
                fi.ProjectFileName = fi.ProjectPath.Substring(fi.ProjectPath.LastIndexOf('\\') + 1, fi.ProjectPath.Length - fi.ProjectPath.LastIndexOf('\\') - 1);
                SaveProject(fi, fi.ProjectPath);
                MessageBox.Show("Successfully saved.");
            }
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = "Project";
                sfd.DefaultExt = ".xml";
                sfd.Filter = "XML File |*.xml|Data File |*.dat|All Files|*.*";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    fi.ProjectPath = sfd.FileName;
                    fi.ProjectFileName = fi.ProjectPath.Substring(fi.ProjectPath.LastIndexOf('\\') + 1, fi.ProjectPath.Length - fi.ProjectPath.LastIndexOf('\\') - 1);
                    SaveProject(fi, fi.ProjectPath);
                    MessageBox.Show("Successfully saved.");
                }
                sfd.Dispose();
            }
        }
        private void SaveAs_Clicked(object sender, EventArgs e)
        {
            //summary: same as Save, just instead of chosing the original location, presents an option to save a same file under a diffrent name or make a copy out of it
            if (!Validation())
            {
                MessageBox.Show("All marked fields must be filled.");
                return;
            }
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "Project";
            sfd.DefaultExt = ".xml";
            sfd.Filter = "XML File |*.xml|Data File |*.dat|All Files|*.*";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                //populates the ProjectFile object and saves it
                fi.ProjectPath = sfd.FileName;
                fi.ProjectName = textBoxName.Text;
                fi.PathInput = textBoxInput.Text;
                fi.PathOutput = textBoxOutput.Text;
                fi.Startd = dateTimePickerStart.Value;
                fi.Endd = dateTimePickerEnd.Value;
                fi.Content = richTextBoxContent.Text;
                fi.Extension = Path.GetExtension(sfd.FileName);
                fi.ProjectFileName = fi.ProjectPath.Substring(fi.ProjectPath.LastIndexOf('\\') + 1, fi.ProjectPath.Length - fi.ProjectPath.LastIndexOf('\\') - 1);
                SaveProject(fi, sfd.FileName);
                //MessageBox.Show("success?");
            }
            sfd.Dispose();
        }
        private void RecentItem_Click(object sender, EventArgs e)
        {
            //summary: loads the selected project from the toolstripmenu
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            LoadProject(item.Name);
        }
        private void Preferences_Clicked(object sender, EventArgs e)
        {
            //summary: opens preferences window, where the user can customize supported options
            Form_Preferences fp = new Form_Preferences();
            fp.ShowDialog();
            recentCount = fp.RecentFiles;
            SaveLoadRecent(1);
        }
        private void ButtonSearch_Clicked(object sender, EventArgs e)
        {
            //summary: commences a search for the project files and content within them
            string search = textBoxSearch.Text;

            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            List<ProjectFile> projectFiles = DirSearch(path);
            List<ProjectFile> searchResults = new List<ProjectFile>();

            
            foreach (ProjectFile _pf in projectFiles)
            {
                //for the files found, checks the options that the user selected and limits that are set up, and then search withing each file for matches using regex
                if (search.Length == 0)
                {
                    return;
                }
                if (checkBoxWholeWords.Checked == true && checkBoxCaseSens.Checked == true)
                {
                    int counter = 0;
                    MatchCollection mtchs;
                    string[] list = { _pf.ProjectName, _pf.Content, _pf.ProjectPath, _pf.ProjectFileName };
                    string searchWhole = @"\b" + search.Trim() + @"\b";
                    foreach (string str in list)
                    {
                        mtchs = Regex.Matches(str, searchWhole);
                        counter += mtchs.Count;
                    }
                    if (int.TryParse(textBoxMaxResults.Text, out int maxres) == true)
                    {
                        if (counter > 0 && counter < maxres)
                            searchResults.Add(_pf);
                        continue;
                    } 
                    else
                    {
                        textBoxMaxResults.Text = "";
                    }
                    if (counter > 0)
                    {
                        searchResults.Add(_pf);
                        continue;
                    }


                }
                else if (checkBoxCaseSens.Checked == true)
                {
                    int counter = 0;
                    MatchCollection mtchs;
                    string[] list = { _pf.ProjectName, _pf.Content, _pf.ProjectPath, _pf.ProjectFileName };
                    string searchWhole = search.Trim();

                    foreach (string str in list)
                    {
                        mtchs = Regex.Matches(str, searchWhole);
                        counter += mtchs.Count;
                    }
                    if (int.TryParse(textBoxMaxResults.Text, out int maxres) == true)
                    {
                        if (counter > 0 && counter < maxres)
                            searchResults.Add(_pf);
                        continue;
                    }
                    else
                    {
                        textBoxMaxResults.Text = "";
                    }
                    if (counter > 0)
                    {
                        searchResults.Add(_pf);
                        continue;
                    }
                }
                else if (checkBoxWholeWords.Checked == true)
                {
                    int counter = 0;
                    MatchCollection mtchs;
                    string[] list = { _pf.ProjectName.ToLower(), _pf.Content.ToLower(), _pf.ProjectPath.ToLower(), _pf.ProjectFileName.ToLower() };
                    string searchWhole = @"\b" + search.Trim().ToLower() + @"\b";
                    foreach (string str in list)
                    {
                        mtchs = Regex.Matches(str, searchWhole);
                        counter += mtchs.Count;
                    }
                    if (int.TryParse(textBoxMaxResults.Text, out int maxres) == true)
                    {
                        if (counter > 0 && counter < maxres)
                            searchResults.Add(_pf);
                        continue;
                    }
                    else
                    {
                        textBoxMaxResults.Text = "";
                    }
                    if (counter > 0)
                    {
                        searchResults.Add(_pf);
                        continue;
                    }
                }
                else
                {
                    int counter = 0;
                    MatchCollection mtchs;
                    string[] list = { _pf.ProjectName.ToLower(), _pf.Content.ToLower(), _pf.ProjectPath.ToLower(), _pf.ProjectFileName.ToLower() };
                    string searchWhole = search.Trim().ToLower();

                    foreach (string str in list)
                    {
                        mtchs = Regex.Matches(str, searchWhole);
                        counter += mtchs.Count;
                    }
                    if (int.TryParse(textBoxMaxResults.Text, out int maxres) == true)
                    {
                        if (counter > 0 && counter < maxres)
                            searchResults.Add(_pf);
                        continue;
                    }
                    else
                    {
                        textBoxMaxResults.Text = "";
                    }
                    if (counter > 0)
                    {
                        searchResults.Add(_pf);
                        continue;
                    }
                }
            }
            //populates the listboxresults
            listBoxResults.DataSource = null;
            listBoxResults.DataSource = searchResults;
        }
        private void ButtonLoad_Clicked(object sender, EventArgs e)
        {
            if(listBoxResults.SelectedIndex == - 1)
            {
                return;
            }
            //summary: loads the selected file from the listbox to the application to be edited, asks the user to save the currently open project, if its not the default one
            if(fi.ProjectPath != "Default.cfg")
            {
            DialogResult res =  MessageBox.Show("Do you want to save? Choosing no will discard everything.","Save", MessageBoxButtons.YesNoCancel);
                if(res == DialogResult.Yes)
                {
                    if (!Validation())
                    {
                        MessageBox.Show("All marked fields must be filled.");
                        return;
                    }
                    fi.ProjectName = textBoxName.Text;
                    fi.PathInput = textBoxInput.Text;
                    fi.PathOutput = textBoxOutput.Text;
                    fi.Startd = dateTimePickerStart.Value;
                    fi.Endd = dateTimePickerEnd.Value;
                    fi.Content = richTextBoxContent.Text;
                    if (fi.ProjectPath != string.Empty)
                    {
                        fi.ProjectFileName = fi.ProjectPath.Substring(fi.ProjectPath.LastIndexOf('\\') + 1, fi.ProjectPath.Length - fi.ProjectPath.LastIndexOf('\\') - 1);
                        SaveProject(fi, fi.ProjectPath);
                    }
                    else
                    {
                        SaveFileDialog sfd = new SaveFileDialog();
                        sfd.FileName = "Project";
                        sfd.DefaultExt = ".xml";
                        sfd.Filter = "XML File |*.xml|Data File |*.dat|All Files|*.*";

                        if (sfd.ShowDialog() == DialogResult.OK)
                        {
                            fi.ProjectPath = sfd.FileName;
                            fi.ProjectFileName = fi.ProjectPath.Substring(fi.ProjectPath.LastIndexOf('\\') + 1, fi.ProjectPath.Length - fi.ProjectPath.LastIndexOf('\\') + 1);
                            SaveProject(fi, fi.ProjectPath);
                        }
                        sfd.Dispose();
                    }
                    Clear();
                    if (listBoxResults.SelectedItem != null)
                    {
                        fi = listBoxResults.SelectedItem as ProjectFile;
                        LoadProject(fi.ProjectPath);
                    }
                }
                else if (res == DialogResult.No)
                {
                    Clear();
                    if(listBoxResults.SelectedItem != null)
                    {
                        fi = listBoxResults.SelectedItem as ProjectFile;
                        LoadProject(fi.ProjectPath);
                    }
                }

            }
            else
            {
                if (listBoxResults.SelectedItem != null)
                {
                    Clear();
                    fi = listBoxResults.SelectedItem as ProjectFile;
                    LoadProject(fi.ProjectPath);
                }
            }
        }

        //checking and changing the color of the labels depending on the status of their textboxes
        private void Name_Changed(object sender, EventArgs e)
        {
            labelName.ForeColor = Color.Black;
        }
        private void StartDate_Changed(object sender, EventArgs e)
        {
            if(dateTimePickerStart.Format == DateTimePickerFormat.Custom)
            {
                dateTimePickerStart.Format = DateTimePickerFormat.Long;
                labelStartDate.ForeColor = Color.Black;
            }
        }
        private void EndDate_Changed(object sender, EventArgs e)
        {
            if (dateTimePickerEnd.Format == DateTimePickerFormat.Custom)
            {
                dateTimePickerEnd.Format = DateTimePickerFormat.Long;
                labelEndDate.ForeColor = Color.Black;
            }
        }
        private void Input_Changed(object sender, EventArgs e)
        {
            labelInput.ForeColor = Color.Black;
        }
        private void Output_Changed(object sender, EventArgs e)
        {
            labelOutput.ForeColor = Color.Black;
        }
        private void ButtonInput_Clicked(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if(fbd.ShowDialog().ToString() != string.Empty)
            {
                textBoxInput.Text = fbd.SelectedPath;
            }
            fbd.Dispose();
        }
        private void ButtonOutputClicked(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            fi.ProjectName = textBoxName.Text;
            if (fbd.ShowDialog().ToString() != string.Empty)
            {
                textBoxOutput.Text = fbd.SelectedPath;
            }
            fbd.Dispose();
        }
        private void Content_Changed(object sender, EventArgs e)
        {
            labelPreview.ForeColor = Color.Black;
        }

        //closing event of the application
        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            SaveLoadRecent();
        }
    }
}
