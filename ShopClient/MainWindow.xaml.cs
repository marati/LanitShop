using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;

using ShopClient.Server;
using ShopClient.Model;

namespace ShopClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class Models
        {
            public Model.Shop Shop { get; set; }
            public Model.Good Good { get; set; }
        }

        Models _models;
        int _errorsCount = 0;

        Interaction _interaction = new Interaction(Properties.Settings.Default.serverPort);

        public MainWindow()
        {
            InitializeComponent();

            _models = new Models()
            {
                Shop = new Model.Shop(),
                Good = new Model.Good()
            };

            DataContext = _models;
            _models.Good.Goods.CollectionChanged += Goods_CollectionChanged;

            ProcessingReceivedFiles();
            RestoreShopInfo(Properties.Settings.Default.shopId);
        }

        void SetStatusMessage(String message)
        {
            StatusPopup.IsOpen = true;
            StatusMessage.Text = message;
        }

        void RestoreShopInfo(int shopId)
        {
            if (shopId != 0)
            {
                ShopInfo shopInfo = new ShopInfo { Id = shopId };

                if (_interaction.SendMessage(shopInfo))
                {
                    SetStatusMessage("Запрос на получение информации о магазине отправлен, ожидание ответа..");
                    InsertGood.IsEnabled = false;
                }
            }
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
                foreach (String receivedFile in _interaction.ReceiveXmlMessages())
                {
                    if (Properties.Settings.Default.shopId == 0)
                    {
                        var mappingMessage = (ShopInfo)Helper.DeserializeXml(typeof(ShopInfo), receivedFile);

                        if (mappingMessage.Id != 0)
                        {
                            Properties.Settings.Default.shopId = mappingMessage.Id;
                            Properties.Settings.Default.Save();

                            InsertGood.Dispatcher.Invoke(
                                DispatcherPriority.Background, new Action(() => { InsertGood.IsEnabled = true; })
                            );
                        }
                    }
                    else
                    {
                        //сообщение о информации магазина (ShopEntity) от сервера
                        if (_models.Shop.IsEmpty())
                        {
                            ShopEntity shop = (ShopEntity)Helper.DeserializeXml(typeof(ShopEntity), receivedFile);

                            if (shop != null)
                            {
                                _models.Shop.SetData(shop);

                                InsertGood.Dispatcher.Invoke(
                                    DispatcherPriority.Background, new Action(() => {
                                        InsertGood.IsEnabled = true;
                                        SetStatusMessage("Данные о магазине успешно получены.");
                                    })
                                );
                            }
                        }
                        else
                        {

                        }
                    }

                    try
                    {
                        File.Delete(receivedFile);
                    }
                    catch (IOException)
                    { }
                }
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
            var shopData = new Model.ShopEntity
            {
                Name = _models.Shop.Name,
                Address = _models.Shop.Address,
                PhoneNumber = _models.Shop.PhoneNumber,
                Email = _models.Shop.Email,
                Port = _interaction.GetListenerPort()
            };

            String sendResult;

            if (_interaction.SendMessage(shopData))
                sendResult = "Сообщение успешно передано";
            else
                sendResult = "Не удалось передать сообщение, повторите отправку";

            SetStatusMessage(sendResult);

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
