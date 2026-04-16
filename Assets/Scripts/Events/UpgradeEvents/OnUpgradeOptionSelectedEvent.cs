using Rewards.StatRewards;

namespace Events.UpgradeEvents
{
    public class OnUpgradeOptionSelectedEvent : IEvent
    {
        public RewardOption Option { get; }

        public OnUpgradeOptionSelectedEvent(RewardOption option)
        {
            Option = option;
        }
    }
}
