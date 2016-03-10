﻿using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Threading;

namespace ShopClient
{
    [Serializable]
    public class ShopData
    {
        public String Name { get; set; }
        public String Address { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }

        public int Token { get; set; }
        /// <summary>
        /// Listener Port на клиенте
        /// </summary>
        public int Port { get; set; }
    }

    public class ShopModel : ShopData, IDataErrorInfo
    {
        public String Error
        {
            get { return null; }
        }

        public String this[String controlName]
        {
            get
            {
                String result = String.Empty;

                if (controlName == "Name" && String.IsNullOrEmpty(Name))
                {
                    result = "Имя фирмы обязательно к заполнению";
                }
                else if (controlName == "Address" && String.IsNullOrEmpty(Address))
                {
                    result = "Адрес обязателен к заполнению";
                }
                else if (controlName == "PhoneNumber")
                {
                    if (String.IsNullOrEmpty(PhoneNumber))
                    {
                        result = "Телефон обязателен к заполнению";
                    }
                    else
                    {
                        Regex phoneRegEx = new Regex(@"^\d{1}\(\d{3}\)\d{3}-\d{2}-\d{2}$");

                        if (!phoneRegEx.IsMatch(PhoneNumber))
                            result = "Введите телефон полностью";
                    }
                }
                else if (controlName == "Email")
                {
                    if (String.IsNullOrEmpty(Email))
                    {
                        result = "Email обязателен к заполнению";
                    }
                    else
                    {
                        Regex simpleMailRegEx = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

                        if (!simpleMailRegEx.IsMatch(Email))
                            result = "Email введён некорректно, пример: example@service.com";
                    }
                }

                return result;
            }
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ShopModel _shop = new ShopModel();
        int _errorsCount = 0;

        //TODO: config
        ServerInteraction _interaction = new ServerInteraction(9316);

        //первое сообщение - xml с mappingoм, далее сообщения с товарами
        bool _isShopMapped = false;
        bool _isProcessing = false;

        public MainWindow()
        {
            InitializeComponent();
            ShopDataGrid.DataContext = _shop;
        }

        void Send_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _errorsCount == 0;
            e.Handled = true;
        }

        void ProcessingReceivedFiles()
        {
            Thread receiveThread = new Thread(() =>
            {
                _isProcessing = true;
                _interaction.ReceiveXmlMessages();
                _isProcessing = false;
            });

            receiveThread.IsBackground = true;
            receiveThread.Start();
        } 

        void Send_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            //TODO: включение информации о адресе и порте
            ShopData shopData = new ShopData()
            {
                Name = _shop.Name,
                Address = _shop.Address,
                PhoneNumber = _shop.PhoneNumber,
                Email = _shop.Email,
                Token = _shop.GetHashCode(),
                Port = _interaction.GetListenerPort()
            };

            String sendResult;

            if (_interaction.SendMessage(shopData))
                sendResult = "Сообщение успешно передано";
            else
                sendResult = "Не удалось передать сообщение, повторите отправку";

            MessageBox.Show(sendResult);

            if (!_isProcessing)
                ProcessingReceivedFiles();

            e.Handled = true;
        }

        void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errorsCount++;
            else
                _errorsCount--;
        }
    }
}
