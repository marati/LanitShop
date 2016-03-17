namespace ShopClient.Server
{
    /// <summary>
    /// Отсылается серверу для получения информации о магазине
    /// Используется, если в App.config сохранён Id магазина
    /// В ответ приходит сообщение ShopEntity
    /// </summary>
    public struct ShopInfo
    {
        public int Id { get; set; }
    }
}
