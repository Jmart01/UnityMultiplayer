using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Threading.Tasks;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button StartHostBtn;
    [SerializeField] Button StartServerBtn;
    [SerializeField] Button StartClientBtn;
    [SerializeField] GameObject LobbyPanel;
    [SerializeField] LobbyBtn lobbyBtnPrefab;
    [SerializeField] Button RefreshLobbyBtn;
    void Start()
    {
        StartHostBtn.onClick.AddListener(StartHost);
        StartServerBtn.onClick.AddListener(StartServer);
        StartClientBtn.onClick.AddListener(StartClient);
        RefreshLobbyBtn.onClick.AddListener(RefreshLobbyBtnClicked);
    }

    private void RefreshLobbyBtnClicked()
    {
        RefreshLobbyList();
    }

    private async Task RefreshLobbyList()
    {
        Task<QueryResponse> task = FindObjectOfType<LobbyManager>().QueryLobbies();
        QueryResponse response = await task;
        foreach(Lobby lobby in response.Results)
        {
            
            LobbyBtn button = Instantiate(lobbyBtnPrefab, LobbyPanel.transform);
            button.Init(lobby.Id, lobby.Name);
            if (response.Results.Contains(lobby))
            {
                response.Results.Remove(lobby);
                Destroy(button);
            }
        }
    }
    private void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    private void StartHost()
    {
        FindObjectOfType<LobbyManager>().HostLobby("Fun lobby");
        //NetworkManager.Singleton.StartHost();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
