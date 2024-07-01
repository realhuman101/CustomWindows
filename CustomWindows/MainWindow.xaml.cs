﻿using Microsoft.Win32;
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
    public partial class MainWindow : Window
    {
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
            String? SelectedItem = (sender as ComboBox).SelectedItem as string;

            switch (SelectedItem)
            {
                case null:
                    //MessageBox.Show("No language selected"); // Debugging
                    scriptCmdTxt.Text = "";
                    break;
                case "python":
                    //MessageBox.Show("Python"); // Debugging
                    scriptCmdTxt.Text = "python";
                    break;
                case "c#":
                    //MessageBox.Show("C#"); // Debugging
                    scriptCmdTxt.Text = "dotnet-script";
                    break;
                //case "java":
                //    //MessageBox.Show("Java"); // Debugging
                //    scriptCmdTxt.Text = "java";
                //    break;
            }
        }

        private void scriptCmdTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            execSelection.SelectedIndex = -1;
        }
    }
}