using System;
using System.Text;
using System.Threading.Tasks;

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
        public int InsertShop(ShopData shop)
        {
            return 0;
        }

        public ShopMapping CreateResponse(int token, int id)
        {
            return new ShopMapping()
            {
                Token = token,
                Id = id
            };
        }
    }
}
