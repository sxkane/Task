using System;

namespace Player
{
    public class PlayerRuntimeData
    {
        public int Level { get; private set; }
        public int Coins { get; private set; }
        public int Experience { get; private set; }

        public int NeedExperience => GetNeedExp(Level);
        
        public event Action<int> OnCoinsChanged;
        public event Action<int, int> OnExpChanged; 
        public event Action<int> OnLevelUp;
        
        private int GetNeedExp(int level)
        {
            return (level + 1) * (level + 1);
        }
        
        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }
        
        public void AddExperience(int amount)
        {
            if (amount <= 0) return;

            Experience += amount;

            while (Experience >= GetNeedExp(Level))
            {
                Experience -= GetNeedExp(Level);
                Level++;
                OnLevelUp?.Invoke(Level);
            }

            OnExpChanged?.Invoke(Experience, NeedExperience);
        }
    }
}