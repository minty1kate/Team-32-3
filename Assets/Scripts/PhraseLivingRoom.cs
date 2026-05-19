using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class PhraseLivingRoom : MonoBehaviour
    {
        public GameObject subtitlePanel;
        public TextMeshProUGUI textUI;
        public string message = "Что это за существо?";

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                textUI.gameObject.SetActive(true);
                textUI.text = message;
                Invoke("HideText", 4f);
                Destroy(gameObject);
            }
        }
        
        void HideText() => textUI.gameObject.SetActive(false);
    }
}