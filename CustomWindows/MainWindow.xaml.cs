using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace CustomWindows
{
    public class Script
    {
        public string Name { get; set; }
        public string Cmd { get; set; }
        public int Condition { get; set;}
        public string ConditionRequirement { get; set; }
    }
    public class Scripts
    {
        public List<Script> AllScripts { get; set; }

        public Scripts()
        {
            AllScripts = new List<Script>();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool customWrite = true;
        public Scripts scripts = new Scripts();

        public MainWindow()
        {
            InitializeComponent();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fileName = "scripts.json";
            string filePath = System.IO.Path.Combine(path, fileName);

            //MessageBox.Show(filePath); // Debugging

            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                scripts = JsonSerializer.Deserialize<Scripts>(jsonString);
                
                string fileContents = System.IO.File.ReadAllText(filePath);
                Scripts FullScript = JsonSerializer.Deserialize<Scripts>(fileContents);

                if (FullScript.AllScripts != null)
                {
                    foreach (Script script in FullScript.AllScripts)
                    {
                        string name = script.Name;
                        string cmd = script.Cmd;

                        ListBoxItem scriptItem = new ListBoxItem();
                        scriptItem.Content = name;
                        scriptItem.ToolTip = cmd;

                        ScriptList.Items.Add(scriptItem);
                    }
                }
            }
        }

        private void Browse_Files(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            //fileDialog.DefaultExt = ".txt"; // Required file extension 
            //fileDialog.Filter = "Text documents (.txt)|*.txt"; // Optional file extensions

            bool? fileResult = fileDialog.ShowDialog();

            if (fileResult == true)
            {
                string fileName = fileDialog.FileName;
                scriptPathTxt.Text = fileName;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs? e)
        {
            string? SelectedItem = null;
            try
            {
                SelectedItem = (e.AddedItems[0] as ComboBoxItem).Content as string;
            }
            catch (IndexOutOfRangeException)
            {
                SelectedItem = null;
            }

            string filePath = scriptPathTxt.Text;

            switch (SelectedItem)
            {
                case null:
                    //MessageBox.Show("No language selected"); // Debugging
                    customWrite = false;
                    break;
                case "Python":
                    //MessageBox.Show("Python"); // Debugging
                    customWrite = false;
                    scriptCmdTxt.Text = "python " + filePath;
                    break;
                case "C#":
                    //MessageBox.Show("C#"); // Debugging
                    customWrite = false;
                    scriptCmdTxt.Text = "dotnet-script " + filePath;
                    //MessageBox.Show(customWrite.ToString());
                    break;
                case "Java":
                    //MessageBox.Show("Java"); // Debugging
                    scriptCmdTxt.Text = "javac " + filePath + "; java " + filePath.Substring(0, filePath.Length - 2);
                    break;
                case "C++":
                    //MessageBox.Show("C++"); // Debugging
                    scriptCmdTxt.Text = "gcc " + filePath + " -o output.exe; output.exe";
                    break;
                case "C":
                    //MessageBox.Show("C"); // Debugging
                    scriptCmdTxt.Text = "gcc " + filePath + " -o output.exe; output.exe";
                    break;
                case "NodeJS":
                    //MessageBox.Show("Javascript"); // Debugging
                    scriptCmdTxt.Text = "node " + filePath;
                    break;
            }
        }

        private void scriptCmdTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            //MessageBox.Show(customWrite.ToString()); // Debugging
            if (customWrite == true)
            {
                execSelection.SelectedValue = "";
            }
            else
            {
                //MessageBox.Show("test"); // Debugging
                customWrite = true;
            }
        }

        private void scriptPathTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            SelectionChangedEventArgs args = new SelectionChangedEventArgs(ComboBox.SelectionChangedEvent, new List<ComboBoxItem>(), new List<ComboBoxItem>());
            ComboBox_SelectionChanged(sender, args);
        }

        private void Add_Script(object sender, RoutedEventArgs e)
        {
            if (scriptNameTxt.Text.Trim() != "" && scriptPathTxt.Text.Trim() != "" && scriptCmdTxt.Text.Trim() != "")
            {
                // Check names are unique
                if (ScriptList.Items.Count > 0)
                {
                    IList<ListBoxItem> items = ScriptList.Items.Cast<ListBoxItem>().ToList();
                    foreach (ListBoxItem item in items)
                    {
                        if (item.Content.ToString() == scriptNameTxt.Text)
                        {
                            MessageBox.Show("Name already exists");
                            return;
                        }
                    }
                }

                ListBoxItem script = new ListBoxItem();
                script.Content = scriptNameTxt.Text;
                script.ToolTip = scriptCmdTxt.Text;

                ScriptList.Items.Add(script);

                Save_Script_To_File();
            } else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }

        private void Delete_Selection(object sender, RoutedEventArgs e)
        {
            if (ScriptList.SelectedItems != null)
            {
                List<string> ToRemove = new List<string>();
                IList<ListBoxItem> items = ScriptList.SelectedItems.Cast<ListBoxItem>().ToList();
                foreach (ListBoxItem item in items)
                {
                    ScriptList.Items.Remove(item);
                    ToRemove.Add(item.Content.ToString());
                }

                foreach (string name in ToRemove)
                {
                    Script script = scripts.AllScripts.Find(x => x.Name == name);
                    scripts.AllScripts.Remove(script);
                }
            }
        }

        private void Save_Script_To_File()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fileName = "scripts.json";
            string filePath = System.IO.Path.Combine(path, fileName);

            Script script = new Script();
            script.Name = scriptNameTxt.Text;
            script.Cmd = scriptCmdTxt.Text;

            scripts.AllScripts.Add(script);
            
            string serializer = JsonSerializer.Serialize(scripts);

            System.IO.File.WriteAllText(filePath, serializer);

            //MessageBox.Show("Script saved"); // Debugging
        }
    }
}