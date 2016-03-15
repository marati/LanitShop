using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShopClient.Model
{
    [Serializable]
    public class GoodEntity : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Mapping на базу данных
        /// </summary>
        public int Id { get; set; }

        String _name;

        public String Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        int _quantity;

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    RaisePropertyChanged();
                }
            }
        }

        public GoodEntity(String name, int quantity)
        {
            _name = name;
            _quantity = quantity;
        }
    }
}
