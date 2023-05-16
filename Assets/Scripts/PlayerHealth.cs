using A7Tam.Core;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private PlayerNetwork m_player;
    [SerializeField] private Image m_healthBarImage;


    private float m_health = 100f;

    private void OnEnable()
    {
        m_player.OnTakeDamage += UpdateHealth;
    }

    private void OnDestroy()
    {
        m_player.OnTakeDamage -= UpdateHealth;
    }

    private void UpdateHealth(float value)
    {
        if (IsDead()) { return; }

        m_health = Mathf.Max(0f, m_health - value);
        m_healthBarImage.fillAmount = m_health / 100f;
        if (IsDead())
        {
            m_player.Die();
        }
    }

    private bool IsDead()
    {
        return m_health <= 0f;
    }
}
