﻿using System;
using System.Data.Entity.Core;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using GestionCaisse_MVVM.Exceptions;
using GestionCaisse_MVVM.Model.Entities;

namespace GestionCaisse_MVVM.Model.Services
{
    public class LoginService
    {
        private readonly LoginContext _loginContext = new LoginContext();
        public static Action ShowLoginWindow;

        public LoginContext GetLoginContext()
        {
            return _loginContext;
        }

        public LoginResult Login(string username, SecureString password)
        {
            var passwordBSTR = default(IntPtr);
            var insecurePassword = "";

            try
            {
                passwordBSTR = Marshal.SecureStringToBSTR(password);
                insecurePassword = Marshal.PtrToStringBSTR(passwordBSTR);
            }
            catch
            {
                insecurePassword = "";
            }

            return IsUserAuthorizedToLogIn(username, insecurePassword);
        }

        private LoginResult IsUserAuthorizedToLogIn(string username, string plainTextPassword)
        {
            if (username == null || plainTextPassword == null) return null;
            var convertedPassword = CalculateMd5Hash(plainTextPassword);

            try
            {
                using (var context = new DBConnection())
                {
                    var user = context.Users.FirstOrDefault(x => x.Name.Equals(username) &&
                                                                 x.PersonnalPassword.Equals(convertedPassword));
                    if (user == null)
                    {
                        return new LoginResult(ConnectionResult.NotFound, null);
                    }
                    return !user.IsActive ? new LoginResult(ConnectionResult.Disabled, user) : new LoginResult(ConnectionResult.Authorized, user);
                }
            }
            catch (EntityException ex)
            {
                throw new ConnectionFailedException(ex.Message, ex);
            }
        }

        public static string CalculateMd5Hash(string input)
        {
            var md5 = MD5.Create();

            var inputBytes = Encoding.ASCII.GetBytes(input);

            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            foreach (var t in hash)
                sb.Append(t.ToString("X2"));

            return sb.ToString();
        }

        public bool IsTimerActive;

        #region Singleton

        private static LoginService _instance;

        public static LoginService Instance => _instance ?? (_instance = new LoginService());

        private LoginService()
        {
        }

        #endregion
    }
}