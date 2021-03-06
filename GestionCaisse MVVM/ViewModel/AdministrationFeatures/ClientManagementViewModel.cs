﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GestionCaisse_MVVM.Model.Services;

namespace GestionCaisse_MVVM.ViewModel.AdministrationFeatures
{
    public class ClientManagementViewModel : INotifyPropertyChanged
    {

        public ClientManagementViewModel()
        {
            var dialogService = new DialogService();

            EditClient = new RelayCommand(() =>
            {
                if (SelectedClient == null)
                {
                    dialogService.ShowInformationWindow("Vous devez sélectionner un client !",
                        "Opération impossible !", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                dialogService.ShowEditClient(SelectedClient);

                OnPropertyChanged(nameof(Clients));
            }, o => true);

            RemoveClient = new RelayCommand(() =>
            {
                if (SelectedClient == null)
                {
                    dialogService.ShowInformationWindow("Vous devez sélectionner un client !",
                        "Opération impossible !", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = dialogService.ShowInformationWindow(
                    "Voulez-vous vraiment supprimer ce client ?",
                    "Confirmation de l'opération",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (!result.Equals(MessageBoxResult.Yes)) return;

                try
                {
                    ClientService.RemoveClient(SelectedClient);
                    dialogService.ShowInformationWindow("Ce client a bien été supprimé du système.",
                        "Suppression réussie !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception)
                {
                    dialogService.ShowInformationWindow("Suppression impossible !",
                        "Suppression impossible !", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                OnPropertyChanged(nameof(Clients));
            }, o => true);

            AddClient = new RelayCommand(() =>
            {
                dialogService.ShowAddClient();

                OnPropertyChanged(nameof(Clients));
            }, o => true);
        }

        #region Properties

        public IEnumerable<Client> Clients => ClientService.GetClients();

        private Client _selectedClient;

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { _selectedClient = value; }
        }

        #endregion

        #region Commands
        public ICommand EditClient { get; set; }
        public ICommand AddClient { get; set; }
        public ICommand RemoveClient { get; set; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string p = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
    }
}
