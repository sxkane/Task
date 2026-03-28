using System;
using UnityEngine;

namespace Rewards.StatRewards
{
    [Serializable]
    public class RewardOption
    {
        public string title;
        public string description;
        public Sprite icon;

        public StatReward reward;
    }
}