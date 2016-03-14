using System;
using System.ComponentModel;

namespace ShopClient.Model
{
    public class Good : INotifyPropertyChanged
    {
        /// <summary>
        /// Mapping на базу данных
        /// </summary>
        public int Id { get; set; }
        public String Name { get; set; }
        public int Quantity { get; set; }

        public Good(String name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
