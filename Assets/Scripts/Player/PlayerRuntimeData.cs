using System;
using UnityEngine;

namespace Player
{
    public class PlayerRuntimeData
    {
        public int Level { get; private set; }
        public int Coins { get; private set; }
        public int Experience { get; private set; }
        public int PendingUpgradeSelections { get; private set; }
        public int NeedExperience => GetNeedExp(Level);
        public int RefreshCost { get; private set; } = 1;
        
        public event Action<int> OnCoinsChanged;
        public event Action<int, int> OnExpChanged; 
        public event Action<int> OnLevelUp;
        public event Action<int> OnRefreshCostChanged;
        public event Action<int> OnPendingUpgradeSelectionsChanged;
        
        public void InitializeRun()
        {
            Level = 0;
            Coins = 0;
            Experience = 0;
            PendingUpgradeSelections = 0;
            RefreshCost = 1;

            OnCoinsChanged?.Invoke(Coins);
            OnExpChanged?.Invoke(Experience, NeedExperience);
            OnPendingUpgradeSelectionsChanged?.Invoke(PendingUpgradeSelections);
            OnRefreshCostChanged?.Invoke(RefreshCost);
        }

        public bool CanAfford(int amount)
        {
            return amount >= 0 && Coins >= amount;
        }

        public bool TrySpendCoins(int amount)
        {
            if (amount < 0 || Coins < amount)
                return false;

            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }

        public void ResetRefreshCost()
        {
            RefreshCost = 1;
            OnRefreshCostChanged?.Invoke(RefreshCost);
        }

        public void IncreaseRefreshCost(int amount = 1)
        {
            RefreshCost = Mathf.Max(0, RefreshCost + amount);
            OnRefreshCostChanged?.Invoke(RefreshCost);
        }
        
        private int GetNeedExp(int level)
        {
            return (level + 1) * (level + 1);
        }
        
        public void AddCoins(int amount)
        {
            if (amount <= 0)
                return;

            Coins += amount;
            OnCoinsChanged?.Invoke(Coins);
        }
        
        public void AddExperience(int amount)
        {
            if (amount <= 0)
                return;

            Experience += amount;

            while (Experience >= GetNeedExp(Level))
            {
                Experience -= GetNeedExp(Level);
                Level++;
                PendingUpgradeSelections++;
                OnLevelUp?.Invoke(Level);
                OnPendingUpgradeSelectionsChanged?.Invoke(PendingUpgradeSelections);
            }

            OnExpChanged?.Invoke(Experience, NeedExperience);
        }

        public bool HasPendingUpgradeSelections()
        {
            return PendingUpgradeSelections > 0;
        }

        public bool TryConsumePendingUpgradeSelection()
        {
            if (PendingUpgradeSelections <= 0)
                return false;

            PendingUpgradeSelections--;
            OnPendingUpgradeSelectionsChanged?.Invoke(PendingUpgradeSelections);
            return true;
        }
    }
}
