using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DataCollectorLayer;
using SendManager;
using DataGrid = SendManager.DataGrid;

namespace tree_manager.view
{
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        private new void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
        /// <summary>
        /// blokuje znaki niecyfrowe w rozmiarze indeksu
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool IsTextAllowed(string text)
        {
            {
                var regex = new Regex("[^0-9]+");
                return !regex.IsMatch(text);
            }
        }
    }
}


