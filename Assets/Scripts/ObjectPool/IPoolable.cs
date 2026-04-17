namespace ObjectPool
{
    public interface IPoolable
    {
        void OnSpawned();
        void OnDespawned();
    }
}
