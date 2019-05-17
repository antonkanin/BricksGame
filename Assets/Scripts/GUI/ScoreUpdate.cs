using TMPro;
using UnityEngine;

namespace GUI
{
    public class ScoreUpdate : MonoBehaviour
    {
        // Start is called before the first frame update
        public IntType scoreSO;
        public TMP_Text scoreText;

        private void Update()
        {
            scoreText.text = scoreSO.value.ToString();
        }
    }
}
