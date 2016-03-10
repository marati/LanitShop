using System;

namespace ShopServer
{
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
}
