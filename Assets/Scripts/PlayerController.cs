using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    Animator anim;
    InputAction movement, turning, shooting;
    [SerializeField]
    GameObject bulletPrefab;
    public UIManager playerUI;
    int score = 0;


    private void Awake()
    {
        InputActionsGame inputsGame = new InputActionsGame();
        movement = inputsGame.Game.Movement;
        turning = inputsGame.Game.Turning;
        shooting = inputsGame.Game.Shooting;
    }
    private void OnEnable()
    {
        movement.Enable();
        turning.Enable();
        shooting.Enable();
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        transform.position = new Vector3 (Random.Range(-5,5), 0, Random.Range(-5,5));
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        transform.Translate(0, 0, 
            movement.ReadValue<float>()*5*Time.deltaTime);
        transform.Rotate(0,
            turning.ReadValue<float>()*180*Time.deltaTime,0);

        if (movement.ReadValue<float>() != 0 )
        {
            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }

        if (shooting.WasPressedThisFrame())
        {
            ShootBulletServerRpc();

        }

    }

    [ServerRpc]
    private void ShootBulletServerRpc()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward, transform.rotation);
        NetworkObject networkObject = bullet.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        bullet.GetComponent<BulletController>().player = this;
    }

    [ClientRpc]
    public void AddPointsClientRpc(int points)
    {
        if(!IsOwner) return;
        score += points;
        UIManager.instance.UpdateScore(score);
    }
}
