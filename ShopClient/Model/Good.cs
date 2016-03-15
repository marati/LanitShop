using System;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShopClient.Model
{
    public class Good
    {
        ObservableCollection<GoodEntity> _goods = new ObservableCollection<GoodEntity>();

        public ObservableCollection<GoodEntity> Goods
        {
            get { return _goods; }
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
            GoodEntity good = parameter as GoodEntity;

            if (good != null)
                if (!Goods.Contains(good))
                    Goods.Add(good);
        }
    }
}
