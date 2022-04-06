using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] UnityTransport transport;

    private async Task Start()
    {
        Debug.Log("Start Hosting, initializing unity gaming services");
        await UnityServices.InitializeAsync();
        Debug.Log("services are initialized");

        //sign in with Unity
        Debug.Log("logging in");
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Debug.Log("logged in");
    }

    public async Task HostLobby(string lobbyName)
    {
        //start a Relay server on your local machine
        Allocation allocation = await Relay.Instance.CreateAllocationAsync(2);
        //a join code is needed to join a relay server. a relay server is a server created on a local machine
        string joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log($"relay server is established with join code: {joinCode}");

        //kick start the lobby
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.Data = new Dictionary<string, DataObject>()
        {
            { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode)}
        };
        options.IsPrivate = false;
        Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(lobbyName, 2,options);
        Debug.Log($"Lobby created with name {lobby.Name}, and id: {lobby.Id}");
        StartCoroutine(PingLobby(lobby.Id));

        //populate transport info
        transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        //start the host
        NetworkManager.Singleton.StartHost();
    }

    internal async Task JoinLobby(string lobbyID)
    {
        JoinLobbyByIdOptions options = new JoinLobbyByIdOptions();
        Lobby lobbyJoined = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyID);
        string joinCode = lobbyJoined.Data["joinCode"].Value;
        Debug.Log($"lobby {lobbyJoined.Name} joined successfully with join code: {joinCode}");

        JoinAllocation allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        Debug.Log($"Relay server joined successfully at {allocation.RelayServer.IpV4}");

        transport.SetRelayServerData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        //start client
        NetworkManager.Singleton.StartClient();
    }

    private IEnumerator PingLobby(string lobbyId)
    {
        while(true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("pinging lobby");
            yield return new WaitForSeconds(10f);
        }
    }

    public Task<QueryResponse> QueryLobbies()
    {
        QueryLobbiesOptions queryLobbyOptions = new QueryLobbiesOptions();
        Task<QueryResponse> responseTask =  Lobbies.Instance.QueryLobbiesAsync(queryLobbyOptions);
        return responseTask;
    }
}
