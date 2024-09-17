using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System;


public class UIManager : NetworkBehaviour
{
    public GameObject seekerPrefab;

    [SerializeField]
    private TextMeshProUGUI clientsCounter;

    [SerializeField]
    private TextMeshProUGUI scoreCounter;
    private NetworkVariable<int> playersNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
    public static UIManager instance;

    [SerializeField]
    private UnityTransport netTransport;

    [SerializeField]
    private TMP_InputField ipField;

    [SerializeField]
    private TMP_InputField portField;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        NetworkManager.Singleton.ConnectionApprovalCallback += ClientApprovalCheck;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;

        StartCoroutine(ClientCounter());
    }

    private IEnumerator ClientCounter()
    {
        while(true)
        {
            if (IsServer)
            {
                playersNum.Value = NetworkManager.Singleton.ConnectedClients.Count;
            }
            clientsCounter.text = playersNum.Value.ToString();
            yield return new WaitForSeconds(0.5f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartHost()
    {
        Debug.Log("START HOST BUTTON");
        netTransport.ConnectionData.Address = ipField.text;
        netTransport.ConnectionData.Port = (ushort)Int32.Parse(portField.text);
        NetworkManager.Singleton.StartHost();
    }
    public void StartClient()
    {
        Debug.Log("START CLIENT BUTTON");
        netTransport.ConnectionData.Address = ipField.text;
        netTransport.ConnectionData.Port = (ushort)Int32.Parse(portField.text);
        NetworkManager.Singleton.StartClient();
    }


    [ServerRpc]
    public void StartSeekerServerRpc()
    {
        GameObject seeker = Instantiate(seekerPrefab, new Vector3(UnityEngine.Random.Range(-5, 5), 0.5f, UnityEngine.Random.Range(-5, 5)),transform.rotation);
        NetworkObject networkObject = seeker.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
    }
    public void UpdateScore(int score)
    {
        UIManager.instance.scoreCounter.text = score.ToString();
    }
    
    private async void OnClientConnected(ulong clientId)

    {

        if (clientId == NetworkManager.Singleton.LocalClientId) //if the connected client is our instance we don't need to acknowledge that connection.

        {

            Debug.Log("Host Connected as Client");

            return;

        }

        Debug.Log("Client {clientId} Connected!");

    }

    private void ClientApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) //this only runs on the host/server

    {

        Debug.Log("ApprovalCheck Requested");

        response.Approved = true;

        response.CreatePlayerObject = true;

        // The Prefab hash value of the NetworkPrefab, if null the default NetworkManager player Prefab is used
        response.PlayerPrefabHash = null;

    }
}
