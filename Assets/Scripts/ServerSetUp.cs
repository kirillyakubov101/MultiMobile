using UnityEngine;
using Unity.Netcode;

public class ServerSetUp : MonoBehaviour
{
    public void StartAsServerHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartAsClient()
    {
        NetworkManager.Singleton.StartClient();
    }
}
