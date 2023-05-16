using A7Tam.Core;
using TMPro;
using UnityEngine;

public class PlayerCoinsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text m_coinText;
    [SerializeField] private PlayerNetwork m_player;

    private void OnEnable()
    {
        m_player.OnTakeCoin += UpdateCoinText;
    }

    private void OnDestroy()
    {
        m_player.OnTakeCoin -= UpdateCoinText;
    }

    private void UpdateCoinText(int amount)
    {
        m_coinText.text = amount.ToString();
    }

}
