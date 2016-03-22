using System;
using System.Data.Entity;

using ShopServer.Client;

namespace ShopServer
{
    class Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shop"></param>
        /// <returns>Id магазина в БД</returns>
        public int InsertShop(ShopEntity shop)
        {
            //TODO: сделать проверку на существующий элемент. В этом случае делать Update
            using (ShopEntityContext db = new ShopEntityContext())
            {
                ShopEntity insertedShop = db.Shops.Add(shop);
                db.SaveChanges();
                return insertedShop.Id;
            }
        }

        public void UpdateShop(ShopEntity shop)
        {
            using (ShopEntityContext db = new ShopEntityContext())
            {
                db.Entry(shop).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ShopEntity GetShopById(int shopId)
        {
            ShopEntity result = null;

            using (ShopEntityContext db = new ShopEntityContext())
            {
                result = db.Shops.Find(shopId);
            }

            return result;
        }

        public ShopInfo ShopEntityResponse(int id)
        {
            return new ShopInfo()
            {
                Id = id
            };
        }
    }
}
