using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ShopClient.Model
{
    public class GoodModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        ObservableCollection<Good> _goods = new ObservableCollection<Good>();

        public ObservableCollection<Good> Goods
        {
            get { return _goods;  }

            set
            {
                //_goods = value;
                //RaisePropertyChanged("Goods");
            }
        }

        private ICommand _addGood;
        public ICommand AddGood
        {
            get
            {
                return _addGood ?? (_addGood = new GoodCommand<object>(Add));
            }
        }

        public void Add(object parameter)
        {
            Good good = parameter as Good;

            if (good != null)
            {
                if (!Goods.Contains(good))
                {
                    Goods.Add(good);
                    RaisePropertyChanged("Goods");
                }
            }
        }
    }
}
