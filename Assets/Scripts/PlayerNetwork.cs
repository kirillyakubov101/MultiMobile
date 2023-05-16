using System;
using Unity.Netcode;
using UnityEngine;


namespace A7Tam.Core
{
    public class PlayerNetwork : NetworkBehaviour
    {
        [SerializeField] private Projectile m_projectile;
        [SerializeField] private Transform m_shootPoint;
        [SerializeField] private GameObject m_Canvases;
        [SerializeField] private GameObject m_clientSideLogic;
      

        [SerializeField] private Joystick m_joystick;
        [SerializeField] private float m_moveSpeed = 10f;
        [SerializeField] private float m_rotationSpeed = 10f;

        public event Action<float> OnTakeDamage;
        public event Action<int> OnTakeCoin;

        private GameManager m_gameManager;

        private NetworkVariable<int> m_coins = new NetworkVariable<int>
            (
            value: 0,
            readPerm: NetworkVariableReadPermission.Everyone,
            writePerm: NetworkVariableWritePermission.Server
            );

        public int GetCoins() => m_coins.Value;

        public override void OnNetworkSpawn()
        {
            if(!IsOwner)
            {
                Destroy(m_Canvases);
                Destroy(m_clientSideLogic);
            }
            m_gameManager = FindObjectOfType<GameManager>();
            m_gameManager.PopulateList(OwnerClientId);
        }

        private void Update()
        {
            if (!IsOwner) return;
            
            float deltaX, deltaY;
            deltaX = m_joystick.Horizontal;
            deltaY = m_joystick.Vertical;

            Vector2 direction = new Vector2(deltaX, deltaY).normalized;
            transform.Translate(direction * Time.deltaTime * m_moveSpeed, Space.World);

            if (direction.magnitude > 0)
            {
               Quaternion desiredRotation = Quaternion.LookRotation(Vector3.forward,direction); 
               transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, Time.deltaTime * m_rotationSpeed);
                
            }
        }

        public void AddCoin()
        {
            if (!IsOwner) { return; }
            int coins = m_coins.Value + 1;
            OnTakeCoin?.Invoke(coins);
            CallForCoinUpdateServerRpc();
            
        }

        public void TakeDamage(float damage)
        {
            OnTakeDamage?.Invoke(damage);
        }

        public void Die()
        {
            if (!IsOwner) return;
            m_gameManager.RemoveFromList(OwnerClientId);
            CallForDeathServerRpc();
        }

        [ServerRpc]
        private void CallForDeathServerRpc()
        {
            NotifyPlayerWin(m_gameManager.GetLastId());
            //death
        } 

        private void NotifyPlayerWin(ulong id)
        {
            if (!IsOwner) { return; }
            m_gameManager.SpawnWinScreenServerRpc(id);
        }

        [ServerRpc]
        public void CallForCoinUpdateServerRpc()
        {
            //logic is done only on the server
            m_coins.Value++;
        }

        public void Shoot()
        {
            if (!IsOwner) { return; }
            ShootServerRpc(m_shootPoint.position, m_shootPoint.up);
        }

        [ServerRpc]
        private void ShootServerRpc(Vector3 position,Vector2 direction)
        {
            //Spawn projectile on the server
            Projectile instance = Instantiate(m_projectile, position, Quaternion.identity);
           
            if (instance.TryGetComponent(out NetworkObject networkObject))
            {
                networkObject.SpawnWithOwnership(OwnerClientId);
            }

            //Assign the direction of the shooting to the client
            instance.InitDirectionClientRpc(direction);
        }

       









    }

}
