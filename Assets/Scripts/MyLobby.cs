using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Netcode;

public class MyLobby : MonoBehaviour
{
    [SerializeField] private TMP_InputField m_playerNameField;
    [SerializeField] private TMP_InputField m_lobbyName;
    [SerializeField] private GameObject m_lobbyList;
    [SerializeField] private GameObject m_lobbyNamePrefab;
    [SerializeField] private GameObject m_mainLobby;
    [SerializeField] private TMP_Text m_mainLobbyTitleText;
    [SerializeField] private GameObject m_playerArea;
    [SerializeField] private TMP_Text m_playerNamePrefab;
    [SerializeField] private Button m_StartGameBtn;

    private string m_playerName;
    private Lobby CurrentLobby;

    public string PlayerID;

    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
           //Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        PlayerID = AuthenticationService.Instance.PlayerId;

        
    }

    private void Update()
    {
        HandleLobbyPoll();
        if(CurrentLobby != null)
        {
            m_StartGameBtn.gameObject.SetActive(CurrentLobby.HostId == PlayerID);
        }
       
    }

    private float timer = 3f;


    private async void HandleLobbyPoll()
    {
        if(CurrentLobby != null)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                timer = 3f;

                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(CurrentLobby.Id);

                foreach(Transform ele in m_playerArea.transform)
                {
                    Destroy(ele.gameObject);
                }

                foreach (var ele in lobby.Players)
                {
                    var inst = Instantiate(m_playerNamePrefab, m_playerArea.transform);
                    inst.text = ele.Data["PlayerName"].Value;
                }
            }
        }
    }

    public void ChooseName()
    {
        m_playerName = m_playerNameField.text;
    }

    public void ValidateLobby()
    {
        CreateLobby(m_lobbyName.text);
    }

    public void ShowListOfLobbies()
    {
        ListLobbies();
    }

    private async void CreateLobby(string lobbyName)
    {
        try
        {
            CreateLobbyOptions createLobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,m_playerName) }
                    }
                }
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2, createLobbyOptions);
            CurrentLobby = lobby;

            m_mainLobby.SetActive(true);
            m_mainLobbyTitleText.text = lobbyName;

            foreach (var ele in lobby.Players)
            {
                var inst = Instantiate(m_playerNamePrefab, m_playerArea.transform);
                inst.text = ele.Data["PlayerName"].Value;
            }

            NetworkManager.Singleton.StartHost();

            //Debug.Log("Created lobby: " + lobby.Name + " " + lobby.MaxPlayers);
        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
    }

   

    private async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            foreach(Transform child in m_lobbyList.transform)
            {
                Destroy(child.gameObject);
            }

            foreach(var lobby in queryResponse.Results)
            {
                var inst = Instantiate(m_lobbyNamePrefab, m_lobbyList.transform);
                inst.GetComponent<TMP_Text>().text = lobby.Name;
                inst.GetComponent<LobbyName>().SetName(lobby.Id, lobby.Name);
                
            }

        }
        catch (LobbyServiceException e) 
        {
            print(e);
        }
    }

    public async void JoinLobby(string lobbyNameId,string lobbyName)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        {"PlayerName",new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,m_playerName) }
                    }
                }
            };

            Lobby lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyNameId, options);
            CurrentLobby = lobby;

            m_mainLobby.SetActive(true);
            m_mainLobbyTitleText.text = lobbyName;
           
            foreach (var ele in lobby.Players)
            {
                var inst = Instantiate(m_playerNamePrefab, m_playerArea.transform);
                inst.text = ele.Data["PlayerName"].Value;
            }

            NetworkManager.Singleton.StartClient();

        }
        catch (LobbyServiceException e)
        {
            print(e);
        }
        
    }

   

    

  

   

   



}
