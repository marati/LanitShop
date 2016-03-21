using System;

namespace ShopClient.Model
{
    /// <summary>
    /// Является частью модели Good
    /// Отсылается на сервер при добавлении товара; принимается от сервера, когда другой клиент добавит товар
    /// </summary>
    [Serializable]
    public class GoodEntity : CustomPropertyChanged
    {
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
