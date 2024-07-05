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
using Microsoft.WindowsAPICodePack.Shell;
using System.Globalization;
using Xceed.Wpf.Toolkit.Primitives;
using System.Threading;

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
        private int? condition = null;
        private bool conditionRequirement = false;

        public MainWindow()
        {
            InitializeComponent();

            string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string fileName = "scripts.json";
            string filePath = System.IO.Path.Combine(path, fileName);

            //MessageBox.Show(filePath); // Debugging

            AddApplicationToCurrentUserStartup();

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

            Thread scriptRunner = new Thread(new ParameterizedThreadStart(Active_Script));

            foreach (Script script in scripts.AllScripts)
            {
                if (script.Condition == 0) // Computer Start
                {
                    Run_Script(script);
                }
            }
        }

        public static void AddApplicationToCurrentUserStartup()
        {
            // From https://stackoverflow.com/questions/5089601/how-to-run-a-c-sharp-application-at-windows-startup/5527457#5527457

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                key.SetValue("CustomWindows", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
            }
        }

        private void Active_Script(object? parameter)
        {
            while (true)
            {
                string currTime = DateTime.Now.ToString("HH:mm");

                foreach (Script script in scripts.AllScripts)
                {
                    switch (script.Condition)
                    {
                        case 1: // App Start
                            
                            break;
                        case 2: // Certain Time
                            if (script.ConditionRequirement == currTime)
                            {
                                Run_Script(script);
                            }
                            break;
                        case 3: // Always On
                            Run_Script(script);
                            break;
                    }
                }   
            }
        }

        private void Run_Script(Script script)
        {
            System.Diagnostics.Process.Start("CMD.exe", script.Cmd);
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

        private void Reset_Fields()
        {
            scriptNameTxt.Text = "";
            scriptPathTxt.Text = "";
            scriptCmdTxt.Text = "";

            execSelection.SelectedValue = "";
            RunCondition.SelectedValue = "";

            customWrite = true;
            condition = null;
            conditionRequirement = false;

            ConditionRequirementLabel.Visibility = Visibility.Collapsed;
            ConditionRequirements.Visibility = Visibility.Collapsed;
            conditionTime.Visibility = Visibility.Collapsed;
        }

        private bool Check_Condition_Filled()
        {
            switch (condition)
            {
                case null:
                    return false;
                case 0:
                    return true;
                case 1:
                    if (ConditionRequirements.SelectedItem == "" || ConditionRequirements.SelectedItem == null)
                    {
                        return false;
                    } else
                    {
                        return true;
                    }
                case 2:
                    string text = conditionTime.Text;
                    string format = "HH:mm";
                    CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
                    DateTime dt;
                    if (DateTime.TryParseExact(text, format, invariant, DateTimeStyles.None, out dt))
                    {
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Please input a correct time \n24 hour in the format HH:mm");
                        return false;
                    }
                default:
                    return false;
            }
        }

        private void Add_Script(object sender, RoutedEventArgs e)
        {
            conditionRequirement = Check_Condition_Filled();

            if (scriptNameTxt.Text.Trim() != "" && scriptPathTxt.Text.Trim() != "" && scriptCmdTxt.Text.Trim() != "" && condition != null && conditionRequirement == true)
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

                // Reset old fields
                Reset_Fields();
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
            script.Condition = condition ?? 0;

            scripts.AllScripts.Add(script);
            
            string serializer = JsonSerializer.Serialize(scripts);

            System.IO.File.WriteAllText(filePath, serializer);

            //MessageBox.Show("Script saved"); // Debugging
        }

        private void RunCondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? selected = null;

            try
            {
                selected = (e.AddedItems[0] as ComboBoxItem).Content as string;
            }
            catch (IndexOutOfRangeException)
            {
                selected = null;
            }

            conditionTime.Visibility = Visibility.Collapsed;

            switch (selected)
            {
                case null:
                    break;
                case "On Computer Start":
                    condition = 0;

                    ConditionRequirementLabel.Visibility = Visibility.Collapsed;
                    ConditionRequirements.Visibility = Visibility.Collapsed;
                    break;
                case "On App Start":
                    condition = 1;

                    ConditionRequirementLabel.Content = "App";

                    // GUID taken from https://learn.microsoft.com/en-us/windows/win32/shell/knownfolderid
                    var FOLDERID_AppsFolder = new Guid("{1e87508d-89c2-42f0-8a7e-645a0f50ca58}");

                    ShellObject appsFolder = (ShellObject)KnownFolderHelper.FromKnownFolderId(FOLDERID_AppsFolder);

                    foreach (var app in (IKnownFolder)appsFolder)
                    {
                        string name = app.Name;

                        ComboBoxItem appItem = new ComboBoxItem();
                        appItem.Content = name;
                        ConditionRequirements.Items.Add(appItem);
                    }

                    ConditionRequirementLabel.Visibility = Visibility.Visible;
                    ConditionRequirements.Visibility = Visibility.Visible;
                    break;
                case "Certain Time":
                    condition = 2;

                    ConditionRequirementLabel.Content = "Time";

                    ConditionRequirementLabel.Visibility = Visibility.Visible;
                    conditionTime.Visibility = Visibility.Visible;

                    break;
                case "Always On":
                    condition = 3;

                    ConditionRequirementLabel.Visibility = Visibility.Collapsed;
                    ConditionRequirements.Visibility = Visibility.Collapsed;
                    break;
            }
        }
    }
}