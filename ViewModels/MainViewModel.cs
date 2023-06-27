using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Helpers;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public ICommand LoadCommand { get; private set; }

        public MainViewModel()
        {
            LoadCommand = new RelayCommand(LoadCommandExecute);
        }

        private void LoadCommandExecute(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv";

            if (openFileDialog.ShowDialog() == true)
            {
                var selectedFilePath = openFileDialog.FileName;
                MessageBox.Show("Selected File: " + selectedFilePath);

                EmployeeTableViewModel viewModel = new EmployeeTableViewModel(selectedFilePath);

                EmployeeTableView newWindow = new EmployeeTableView(viewModel);
                newWindow.Show();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }
    }
}