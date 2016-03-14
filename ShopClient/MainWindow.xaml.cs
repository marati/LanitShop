using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Threading;

namespace ShopClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ShopModel _shop = new ShopModel();
        int _errorsCount = 0;

        Model.GoodModel _goodModel;

        //TODO: config
        ServerInteraction _interaction = new ServerInteraction(9316);

        //первое сообщение - xml с mappingoм, далее сообщения с товарами
        bool _isShopMapped = false;

        public MainWindow()
        {
            InitializeComponent();

            ShopPanel.DataContext = _shop;

            _goodModel = new Model.GoodModel();
            GoodsList.DataContext = _goodModel;
            _goodModel.Goods.CollectionChanged += Goods_CollectionChanged;

            ProcessingReceivedFiles();
        }

        private void Goods_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                
            }
        }

        void ProcessingReceivedFiles()
        {
            Thread receiveThread = new Thread(() =>
            {
                //TODO: отлавливать выход из ф-ции и перезапускать её
                _interaction.ReceiveXmlMessages();
            });

            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        #region Shop Commands

        void Send_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _errorsCount == 0;
            e.Handled = true;
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

            e.Handled = true;
        }

        #endregion

        void Validation_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
                _errorsCount++;
            else
                _errorsCount--;
        }

        void QuantityValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
