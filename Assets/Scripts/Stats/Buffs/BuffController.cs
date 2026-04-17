using System.Collections.Generic;

namespace Stats.Buffs
{
    public sealed class BuffController
    {
        private readonly IBuffStatSource _target;
        private readonly List<BuffInstance> _activeBuffs = new();

        public IReadOnlyList<BuffInstance> ActiveBuffs => _activeBuffs;

        public BuffController(IBuffStatSource target)
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

        public BuffInstance ApplyBuff(BuffData data, object source = null)
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

            var buff = new BuffInstance(data, source);
            ApplyModifiers(buff);
            _activeBuffs.Add(buff);
            return buff;
        }

        public void RemoveBuff(BuffInstance buff)
        {
            if (buff == null)
                return;

            _target.RemoveModifiersFromSource(buff);
            _activeBuffs.Remove(buff);
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

        private void ApplyModifiers(BuffInstance buff)
        {
            StatModifierApplicator.ApplyModifiers(_target, buff.Data.Modifiers, buff);
        }

        private BuffInstance FindExisting(BuffData data, object source)
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
