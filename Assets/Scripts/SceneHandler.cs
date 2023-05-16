using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SceneHandler : NetworkBehaviour
{
    public void CallLoadScene()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        else
        {
            SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

  
}
