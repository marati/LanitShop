namespace ShopClient.Server
{
    /// <summary>
    /// Сообщение, присылаемое после добавления магазина в БД, Id соответствует уникальному ключу в базе
    /// </summary>
    public class ShopMapping
    {
        public int Token { get; set; }
        public int Id { get; set; }
    }
}
