using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
}
