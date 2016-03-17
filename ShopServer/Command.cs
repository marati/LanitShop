using System;

using ShopServer.Client;

namespace ShopServer
{
    public class ShopMapping
    {
        public int Token { get; set; }
        public int Id { get; set; }
    }

    class Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shop"></param>
        /// <returns>Id магазина в БД</returns>
        public int InsertShop(ShopEntity shop)
        {
            return 1;
        }

        public ShopEntity GetShopById(int shopId)
        {
            return new ShopEntity()
            {
                Name = "test",
                Address = "address test"
            };
        }

        public ShopMapping ShopEntityResponse(int token, int id)
        {
            return new ShopMapping()
            {
                Token = token,
                Id = id
            };
        }
    }
}
