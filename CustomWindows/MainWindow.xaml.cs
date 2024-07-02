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

namespace CustomWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private bool customWrite = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                    scriptCmdTxt.Text = "gcc " + filePath + "-o output.exe; output.exe";
                    break;
                case "C":
                    //MessageBox.Show("C"); // Debugging
                    scriptCmdTxt.Text = "gcc " + filePath + "-o output.exe; output.exe";
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
            ComboBox_SelectionChanged(sender, e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (scriptNameTxt.Text.Trim() != "" && scriptPathTxt.Text.Trim() != "" && scriptCmdTxt.Text.Trim() != "")
            {
                // add item here
            } else
            {
                MessageBox.Show("Please fill in all fields");
            }
        }
    }
}