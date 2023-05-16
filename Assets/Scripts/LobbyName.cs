using UnityEngine;

public class LobbyName : MonoBehaviour
{
    
    private string id;
    private string lobbyName;

    public void SetName(string id,string lobbyName)
    {
        this.id = id;
        this.lobbyName = lobbyName;
    }

    public void EnterLobby()
    {
        FindObjectOfType<MyLobby>().JoinLobby(id, lobbyName); //bad way
    }
}
