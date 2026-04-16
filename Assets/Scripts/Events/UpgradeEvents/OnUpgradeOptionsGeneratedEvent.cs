using System.Collections.Generic;
using Rewards.StatRewards;

namespace Events.UpgradeEvents
{
    public class OnUpgradeOptionsGeneratedEvent : IEvent
    {
        public List<RewardOption> Options { get; }
        public int RemainingSelections { get; }

        public OnUpgradeOptionsGeneratedEvent(List<RewardOption> options, int remainingSelections)
        {
            Options = options;
            RemainingSelections = remainingSelections;
        }
    }
}
