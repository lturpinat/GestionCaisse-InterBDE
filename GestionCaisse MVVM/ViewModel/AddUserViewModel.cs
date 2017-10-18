﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GestionCaisse_MVVM.Model.Services;

namespace GestionCaisse_MVVM.ViewModel
{
    public class AddUserViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public AddUserViewModel()
        {

            AddUser = new RelayCommand(() =>
            {
                DialogService dialogService = new DialogService();

                if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password) || SelectedBde == null)
                {
                    dialogService.ShowInformationWindow("Vérifier qu'aucun champ n'est vide !", "Formulaire invalide !",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                UserService.AddUser(Login, LoginService.CalculateMD5Hash(Password), SelectedBde, IsActive, IsAdmin);
                Close();
            }, o => true);
            CloseWindow = new RelayCommand(() => Close(), o => true);
        }

        #region Properties
        private string _login;

        public string Login
        {
            get { return _login; }
            set { _login = value; OnPropertyChanged(); }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public IEnumerable<BDE> Bdes => BDEService.GetBDEs();

        private BDE _selecteBde;

        public BDE SelectedBde
        {
            get { return _selecteBde; }
            set { _selecteBde = value; OnPropertyChanged(); }
        }

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }

        private bool _isAdmin;

        public bool IsAdmin
        {
            get { return _isAdmin; }
            set { _isAdmin = value; OnPropertyChanged(); }
        }
        #endregion

        #region Commands
        public ICommand AddUser { get; }
        public ICommand CloseWindow { get; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}