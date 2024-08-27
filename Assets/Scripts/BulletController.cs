using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class BulletController : NetworkBehaviour
{
    float lifeTime = 5.0f;
    public PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().AddForce(transform.forward*10, ForceMode.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        if (lifeTime > 0)
            lifeTime -= Time.deltaTime;
        else
            NetworkObject.Destroy(gameObject);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            player.AddPointsClientRpc(1);
            NetworkObject.Destroy(gameObject);
        }
    }
}