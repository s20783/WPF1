using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using WpfApp1.Helpers;
using WpfApp1.Models;
using WpfApp1.Views;

namespace WpfApp1.ViewModels
{
    public class EmployeeTableViewModel : ViewModelBase
    {
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        public ICommand BackCommand { get; private set; }
        public ICommand EditCommand { get; private set; }

        public EmployeeTableViewModel(string selectedFilePath)
        {
            LoadCSV(selectedFilePath);
            BackCommand = new RelayCommand(BackCommandExecute);
            EditCommand = new RelayCommand(EditCommandExecute);
        }

        private void BackCommandExecute(object parameter)
        {
            MainWindow newWindow = new MainWindow();
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

        private void EditCommandExecute(object sender)
        {
            int employeeId = (int)sender;

            EditEmployeeViewModel viewModel = new EditEmployeeViewModel(employeeId);

            EditEmployeeView newWindow = new EditEmployeeView(viewModel);
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

        private void SaveToDatabase()
        {
            using (var context = new MyDbContext())
            {
                context.Employees.AddRange(Employees);
                context.SaveChanges();
            }
        }

        private void LoadCSV(string path)
        {
            try
            {
                //pomijam pierwszy wiersz, w którym są nazwy kolumn
                var lines = File.ReadAllLines(path).Skip(1);
                foreach (var line in lines)
                {
                    Employees.Add(ParseEmployee(line));
                }
            } 
            catch(Exception ex)
            {
                MessageBox.Show($"Couldn't open file: {path}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine(ex.Message);
                return;
            }

            try
            {
                SaveToDatabase();
            } 
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private Employee ParseEmployee(string line)
        {
            var a = line.Split(",");
            return new Employee
            {
                Id = int.Parse(a[0]),
                Name = a[1],
                Surname = a[2],
                Email = a[3],
                Phone = a[4]
            };
        }
    }
}