using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace ShopClient.Model
{
    public class Shop : ShopEntity, IDataErrorInfo
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

        public bool IsEmpty()
        {
            return
                String.IsNullOrEmpty(Name) &&
                String.IsNullOrEmpty(Address) &&
                String.IsNullOrEmpty(PhoneNumber) &&
                String.IsNullOrEmpty(Email) &&
                Port == 0;
        }

        public void SetData(ShopEntity entity)
        {
            Name = entity.Name;
            Address = entity.Address;
            PhoneNumber = entity.PhoneNumber;
            Email = entity.Email;
            Port = entity.Port;
        }
    }
}
