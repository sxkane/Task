using System.Collections.Generic;
using Stats;

namespace Enemy.Buffs
{
    public sealed class EnemyBuffController
    {
        private readonly EnemyStats _target;
        private readonly List<EnemyBuffInstance> _activeBuffs = new();

        public IReadOnlyList<EnemyBuffInstance> ActiveBuffs => _activeBuffs;

        public EnemyBuffController(EnemyStats target)
        {
            _target = target;
        }

        public void Tick(float deltaTime)
        {
            for (var i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[i];
                buff.Tick(deltaTime);

                if (!buff.IsExpired)
                    continue;

                _target.RemoveModifiersFromSource(buff);
                _activeBuffs.RemoveAt(i);
            }
        }

        public EnemyBuffInstance ApplyBuff(EnemyBuffData data, object source = null)
        {
            if (data == null)
                return null;

            var existing = FindExisting(data, source ?? data);
            if (existing != null)
            {
                if (data.RefreshDurationOnReapply)
                    existing.Refresh();

                return existing;
            }

            var buff = new EnemyBuffInstance(data, source);
            ApplyModifiers(buff);
            _activeBuffs.Add(buff);
            return buff;
        }

        public void RemoveBuffsFromSource(object source)
        {
            if (source == null)
                return;

            for (var i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[i];
                if (!Equals(buff.Source, source))
                    continue;

                _target.RemoveModifiersFromSource(buff);
                _activeBuffs.RemoveAt(i);
            }
        }

        public void Clear()
        {
            for (var i = 0; i < _activeBuffs.Count; i++)
                _target.RemoveModifiersFromSource(_activeBuffs[i]);

            _activeBuffs.Clear();
        }

        private void ApplyModifiers(EnemyBuffInstance buff)
        {
            var modifiers = buff.Data.Modifiers;
            for (var i = 0; i < modifiers.Count; i++)
            {
                var modifier = modifiers[i];
                var stat = _target.GetStat(modifier.statType);
                stat.AddModifier(new Modifier(modifier.value, modifier.modifierType, buff));
            }
        }

        private EnemyBuffInstance FindExisting(EnemyBuffData data, object source)
        {
            for (var i = 0; i < _activeBuffs.Count; i++)
            {
                var buff = _activeBuffs[i];
                if (buff.Data == data && Equals(buff.Source, source))
                    return buff;
            }

            return null;
        }
    }
}
