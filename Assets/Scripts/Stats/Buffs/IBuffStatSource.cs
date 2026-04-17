namespace Stats.Buffs
{
    public interface IBuffStatSource
    {
        bool TryGetStat(string statKey, out Stat stat);
        void RemoveModifiersFromSource(object source);
    }
}
