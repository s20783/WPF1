using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Helpers;
using WpfApp1.Models;

namespace WpfApp1.ViewModels
{
    public class EditEmployeeViewModel
    {
        public ICommand SaveCommand { get; private set; }
        public ICommand BackCommand { get; private set; }
        public Employee Employee { get; set; }

        public EditEmployeeViewModel(int employeeId) 
        {
            LoadEmployeeById(employeeId);
            BackCommand = new RelayCommand(BackCommandExecute);
            SaveCommand = new RelayCommand(SaveCommandExecute);
        }

        private void LoadEmployeeById(int employeeId)
        {
            using(var context = new MyDbContext())
            {
                Employee = context.Employees.Find(employeeId);
                if(Employee == null)
                {
                    MessageBox.Show("Couldn't find the employee", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

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
            }
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

        private void SaveCommandExecute(object parameter)
        {
            using(var context = new MyDbContext())
            {
                if (Employee != null)
                {
                    // Sprawdzenie poprawności wprowadzonych danych
                    if(!IsValid(Employee.Name, Employee.Surname, Employee.Email, Employee.Phone))
                    {
                        MessageBox.Show("Invalid data, try again", "Invalid", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    //zapisanie do bazy edytowanego pracownika
                    context.Employees.Update(Employee);
                    context.SaveChanges();

                    MessageBox.Show("Employee successfully saved");
                }
                else
                {
                    MessageBox.Show("Employee not found in the database.");
                }
            }
        }

        private bool IsValid(string name, string surname, string email, string phone)
        {
            if(string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname) || string.IsNullOrEmpty(phone))
            {
                return false;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)");
            if (!regex.IsMatch(email))
            {
                return false;
            }

            return true;
        }
    }
}
