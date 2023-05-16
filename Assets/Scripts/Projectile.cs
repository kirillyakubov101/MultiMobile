using A7Tam.Core;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private float m_speed = 2f;
    [SerializeField] private float m_damage = 10f;
    [SerializeField] private float m_selfDestroyTime = 5f;
    [SerializeField] private SpriteRenderer m_sprite;

    private Vector2 m_shootDirection;
    private bool m_hasInit = false;
    private bool m_despawn = false;

    

    private void Update()
    {
        if (!m_hasInit) { return; }
        transform.Translate(m_shootDirection * Time.deltaTime * 2f);
        
    }

    [ClientRpc]
    public void InitDirectionClientRpc(Vector2 direction)
    {
        if (!IsOwner) { return; }
        m_hasInit = true;
        m_shootDirection = direction;
        Invoke(nameof(CallDespawnServerRpc), m_selfDestroyTime);
    }

    [ServerRpc]
    private void CallDespawnServerRpc()
    {
        GetComponent<NetworkObject>().Despawn(true);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerNetwork player = collision.GetComponent<PlayerNetwork>();
        if (player != null && player.OwnerClientId != OwnerClientId)
        {
            m_sprite.enabled = false;
            player.TakeDamage(m_damage);
            Invoke(nameof(CallDespawnServerRpc), 2f);
            
        }
    }
}
