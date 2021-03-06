﻿using System.Configuration;
using System.Windows;
using System.Windows.Input;
using GestionCaisse_MVVM.ViewModel;

namespace GestionCaisse_MVVM.View
{
    /// <summary>
    ///     Logique d'interaction pour LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            EncryptConfigSection("GestionCaisse.exe");

            var vm = new LoginViewModel
            {
                Close = () => Close(),
                Show = () => Show(),
                Hide = () => Hide()
            };

            DataContext = vm;

            UsernameTextBox.Focus();
        }

        /// <summary>
        ///     Encrypte le connectionString d'une section de App.config
        /// </summary>
        /// <param name="appName">Le nom du fichier de l'application</param>
        /// <example>EncryptConfigSection(MonApplication.exe);</example>
        private void EncryptConfigSection(string appName)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(appName);
                var section = (ConnectionStringsSection) config.GetSection("connectionStrings");

                if (!section.SectionInformation.IsProtected)
                {
                    section.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    config.Save();
                }
            }
            catch
            {
                MessageBox.Show("Impossible de lire le fichier de configuration !",
                    "Fichier de configuration invalide !", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                ((LoginViewModel) DataContext).CheckAndTryToLogin.Execute(null);
            }
        }
    }
}