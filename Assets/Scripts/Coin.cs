using Unity.Netcode;
using UnityEngine;
using A7Tam.Core;
using Unity.Services.Lobbies.Models;

namespace A7Tam.Collectables
{
    public class Coin : NetworkBehaviour
    {
        private PlayerNetwork player;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.transform.TryGetComponent(out PlayerNetwork player))
            {
                if(this.player != null) { return; }

                this.player = player;
                TakeCoin();
            }
        }

        private void TakeCoin()
        {
            player.AddCoin();
            NetworkObject.Destroy(gameObject);
        }
    }
}


