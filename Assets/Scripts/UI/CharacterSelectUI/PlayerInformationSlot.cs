using TMPro;
using UnityEngine;

namespace UI.CharacterSelectUI
{
    public class PlayerInformationSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerInfoText;

        public void ShowPlayer(string playerName, string playerInfo)
        {
            playerNameText.text = playerName;
            playerInfoText.text = playerInfo;
        }

        public void ShowRandomPlayer()
        {
            playerNameText.text = "???";
            playerInfoText.text = "";
        }
    }
}