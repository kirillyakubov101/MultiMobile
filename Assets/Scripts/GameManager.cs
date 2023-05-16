using A7Tam.Core;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private WinnerScreen m_winnerScreenPrefab;

    private List<ulong> m_Players = new List<ulong>();

    private NetworkVariable<int> m_gainedCoins = new NetworkVariable<int>();

    public void PopulateList(ulong id)
    {
        m_Players.Add(id);
    }

    public void RemoveFromList(ulong id)
    {
        m_Players.Remove(id);
    }

    public ulong GetLastId()
    {
        return m_Players[0];
    }

    [ServerRpc]
    public void SpawnWinScreenServerRpc(ulong clientId)
    {
        if (!IsServer) { return; }

        var networkObject = NetworkManager.ConnectedClients[clientId].PlayerObject;
        m_gainedCoins.Value = networkObject.GetComponent<PlayerNetwork>().GetCoins();

        var inst = Instantiate(m_winnerScreenPrefab);
        if(inst.TryGetComponent(out NetworkObject obj))
        {
            obj.Spawn();
        }

        inst.InitScreenClientRpc(m_gainedCoins.Value, clientId);
    }
}
