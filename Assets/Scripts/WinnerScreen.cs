using TMPro;
using Unity.Netcode;
using UnityEngine;

public class WinnerScreen : NetworkBehaviour
{
    [SerializeField] private TMP_Text m_coinText;
    [SerializeField] private TMP_Text m_id;


    [ClientRpc]
    public void InitScreenClientRpc(int coins, ulong id)
    {
        m_coinText.text = coins.ToString();
        m_id.text = id.ToString();
    }
}
