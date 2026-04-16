namespace Data.Text
{
    public static class UpgradeTextBuilder
    {
        public static string BuildTitle(Rewards.StatRewards.StatReward reward)
        {
            if (reward == null) return string.Empty;
            return StatNameMapper.GetName(reward.type);
        }
    
        public static string BuildDescription(Rewards.StatRewards.StatReward reward)
        {
            if (reward == null) return string.Empty;
            return StatTextBuilder.BuildLine(reward.value, reward.type);
        }
    }
}