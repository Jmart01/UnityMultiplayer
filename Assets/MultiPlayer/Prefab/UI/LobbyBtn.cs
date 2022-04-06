using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyBtn : MonoBehaviour
{
    string lobbyID;
    [SerializeField] LobbyManager lobbyManager;
    // Start is called before the first frame update
    void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
        Button btn = GetComponent<Button>();
        if(btn != null)
        {
            btn.onClick.AddListener(JoinLobby);
        }
    }

    private void JoinLobby()
    {
        if(lobbyManager != null)
        {
            lobbyManager.JoinLobby(lobbyID);
        }
        else
        {
            Debug.Log("lobby manager is null");
        }
    }

    public void Init(string id, string name)
    {
        lobbyID = id;
        TextMeshProUGUI Text = GetComponentInChildren<TextMeshProUGUI>();
        if(Text != null)
        {
            Text.text = name;
        }
    }
}
