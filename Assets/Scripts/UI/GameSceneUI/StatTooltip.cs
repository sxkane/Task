using TMPro;
using UnityEngine;

namespace UI.GameSceneUI
{
    public class StatTooltip : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        public void Show(string newText)
        {
            text.text = newText;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            text.text = "";
            gameObject.SetActive(false);
        }
    }
}