namespace Nesteo.Server.Data.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; }
    }
}
