using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class InGameUI : MonoBehaviour
{
    [SerializeField] Button StartHostBtn;
    [SerializeField] Button StartServerBtn;
    [SerializeField] Button StartClientBtn;
    void Start()
    {
        StartHostBtn.onClick.AddListener(StartHost);
        StartServerBtn.onClick.AddListener(StartServer);
        StartClientBtn.onClick.AddListener(StartClient);
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
        NetworkManager.Singleton.StartHost();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
