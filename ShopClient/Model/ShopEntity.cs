using System;

namespace ShopClient.Model
{
    /// <summary>
    /// Является частью модели Shop, отсылается в виде xml сообщения на сервер для добавления магазина в базу
    /// </summary>
    [Serializable]
    public class ShopEntity : CustomPropertyChanged
    {
        String _name;

        public String Name
        {
            get { return _name; }

            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        String _address;

        public String Address
        {
            get { return _address; }

            set
            {
                if (_address != value)
                {
                    _address = value;
                    RaisePropertyChanged();
                }
            }
        }

        String _phoneNumber;

        public String PhoneNumber
        {
            get { return _phoneNumber; }

            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    RaisePropertyChanged();
                }
            }
        }

        String _email;

        public String Email
        {
            get { return _email;  }

            set
            {
                if (_email != value)
                {
                    _email = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Listener Port на клиенте
        /// </summary>
        public int Port { get; set; }
    }
}
