using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;

namespace ShopServer.Client
{
    public class ShopEntity
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }
        public String PhoneNumber { get; set; }
        public String Email { get; set; }

        /// <summary>
        /// Ip клиента
        /// </summary>
        public String IpAddress { get; set; }
        /// <summary>
        /// Listener Port на клиенте
        /// </summary>
        public int Port { get; set; }
    }

    class ShopEntityContext : DbContext
    {
        public ShopEntityContext()
            : base("DbConnection")
        {
        }

        public DbSet<ShopEntity> Shops { get; set; }

        void HandleValidationException(DbEntityValidationException e)
        {
            foreach (var entityError in e.EntityValidationErrors)
            {
                Console.WriteLine("При добавлении сущности \"{0}\" были обнаружены следующие ошибки:",
                    entityError.Entry.Entity.GetType().Name/*, eve.Entry.State*/
                );

                foreach (var error in entityError.ValidationErrors)
                {
                    Console.WriteLine("Свойство: \"{0}\", Ошибка: \"{1}\"",
                        error.PropertyName, error.ErrorMessage
                    );
                }
            }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                HandleValidationException(e);
                throw;
            }
            catch (DbUpdateException)
            {
                Console.WriteLine("Возникла ошибка при обновлении базы данных.");
                throw;
            }
        }
    }
}
