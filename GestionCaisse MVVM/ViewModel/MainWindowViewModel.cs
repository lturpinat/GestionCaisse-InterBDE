﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GestionCaisse_MVVM.Exceptions;
using GestionCaisse_MVVM.Model.Entities;
using GestionCaisse_MVVM.Model.Services;

namespace GestionCaisse_MVVM.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly BasketService _basketService = BasketService.Instance;
        private readonly LoginService _loginService = LoginService.Instance;

        public MainWindowViewModel()
        {
            var dialogService = new DialogService();

            InsertProduct = new RelayCommand(() =>
            {
                LoginService.Instance.IsTimerActive = false;
                dialogService.ShowProductInsertPage();
            }, o => true);

            ResetBasket = new RelayCommand(() => _basketService.GetBasket().ResetBasket(), o => true);
            DeleteBasketProduct = new RelayCommand(() => _basketService.GetBasket().RemoveProduct(_currentBasketProduct), o => true);
            ShowAdministrationWindow = new RelayCommand(() =>
            {
                LoginService.Instance.IsTimerActive = false;
                dialogService.ShowAdministrationWindow();
            }, o =>
            {
                return _loginService.GetLoginContext().User.IsAdmin;
            });

            ValidateSell = new RelayCommand(() =>
            {
                try
                {
                    if (_basketService.ValidateSell())
                    {
                        _basketService.GetBasket().ResetBasket();
                        dialogService.ShowInformationWindow(
                            "Vente effectuée !",
                            "Confirmation",
                            MessageBoxButton.OK,
                            MessageBoxImage.Exclamation);
                    }
                    else
                    {
                        dialogService.ShowInformationWindow(
                            "Vente invalide ou impossible !",
                            "Attention",
                            MessageBoxButton.OK,
                            MessageBoxImage.Hand);
                    }
                }
                catch (ConnectionFailedException ex)
                {
                    if (ex.InnerException != null)
                        dialogService.ShowInformationWindow(
                            "Problème de connexion à la base de données !\n" + ex.InnerException.Message,
                            "Connexion impossible !", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }, o => true);

            RefreshSessionDelay = new RelayCommand(() => { _timer = AppInformations.DefaultSessionDelay; OnPropertyChanged(nameof(Countdown)); }, o => true);

            Logout = new RelayCommand(() =>
            {
                _basketService.GetBasket().ResetBasket();
                try
                {
                    Timer.Stop();
                }
                catch { }
                Close();
            }, o => true);

            _timer = AppInformations.DefaultSessionDelay;
            StartTimer();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string p = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));

        #region Commands

        public ICommand ResetBasket { get; }

        public ICommand InsertProduct { get; }

        public ICommand ValidateSell { get; }

        public ICommand Logout { get; }

        public ICommand DeleteBasketProduct { get; }

        public ICommand ShowAdministrationWindow { get; }

        public ICommand RefreshSessionDelay { get; }
        #endregion

        #region Properties

        public Basket Basket => BasketService.Instance.GetBasket();

        public IEnumerable<BDE> BDEs
        {
            get
            {
                var bdes = Enumerable.Empty<BDE>();

                try
                {
                    bdes = BDEService.GetBDEs();
                }
                catch (ConnectionFailedException ex)
                {
                    var dialogService = new DialogService();
                    dialogService.ShowInformationWindow(
                        "Problème de connexion à la base de données !\n" + ex.InnerException.Message,
                        "Connexion impossible !", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return bdes;
            }
        }

        private BasketProduct _currentBasketProduct;

        public BasketProduct CurrentBasketProduct
        {
            get { return _currentBasketProduct; }
            set { _currentBasketProduct = value; }
        }

        private BDE _selectedBDE;

        public BDE SelectedBDE
        {
            get => _selectedBDE;
            set
            {
                _selectedBDE = value;
                _loginService.GetLoginContext().BuyingBDE = value;
                OnPropertyChanged();
            }
        }

        public string CurrentUser => $"{_loginService.GetLoginContext().User.Name} ({_loginService.GetLoginContext().BuyingBDE.Name})";

        private DispatcherTimer Timer;
        private int _timer;

        public string Countdown
        {
            get
            {
                var timespan = TimeSpan.FromSeconds(_timer);
                return timespan.ToString(@"hh\:mm\:ss");
            }
        }

        public void StartTimer()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Tick += Timer_Tick;
            LoginService.Instance.IsTimerActive = true;
            Timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (LoginService.Instance.IsTimerActive)
            {
                if (_timer > 0)
                {
                    _timer--;
                    OnPropertyChanged(nameof(Countdown));
                }
                else
                {
                    DialogService dialogService = new DialogService();
                    dialogService.ShowInformationWindow("La session a expiré, reconnectez-vous !", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Logout.Execute(null);
                } 
            }
        }

        #endregion
    }
}