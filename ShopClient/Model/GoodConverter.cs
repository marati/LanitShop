using System;
using System.Globalization;
using System.Windows.Data;

namespace ShopClient.Model
{
    public class GoodConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type t, object o, CultureInfo ci)
        {
            String goodName = (String)values[0];
            int quantity = 0;

            try
            {
                quantity = Int32.Parse((String)values[1]);
            }
            catch (FormatException)
            {
            }

            return new Good(goodName, quantity);
        }

        public object[] ConvertBack(object value, Type[] types, object o, CultureInfo ci)
        {
            Good good = (Good)value;
            return new object[] { good.Name, good.Quantity };
        }
    }
}
