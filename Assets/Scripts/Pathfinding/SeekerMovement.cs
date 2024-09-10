using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;


public class SeekerMovement : NetworkBehaviour
{
    private Coroutine moveToTargetCoroutine;

    private NetworkVariable<Vector3> transformVar = new NetworkVariable<Vector3>();

    public NodeGrid grid;

    public float smoothTime = 0.3F;

    private Vector3 velocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            moveToTargetCoroutine = StartCoroutine(moveToTarget());
           // SetMyValueClientRpc(this.transform.position);
        }

        grid = GameObject.Find("A*").GetComponent<NodeGrid>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            //UpdateSeekerClientRpc(this.transform);
            //   SetMyValueClientRpc(this.transform.position);
            //transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
        if (!IsOwner)
        {
            this.transform.position = transformVar.Value;
        }


        
    }
    private IEnumerator moveToTarget()
    {
        yield return new WaitForEndOfFrame();

        while(grid.path == null) 
            yield return null;
        Vector3 oldPosition = grid.path[grid.path.Count - 1].worldPosition;

        //loop that moves to each node in path towards target
        foreach (Node n in grid.path)
        {
            //if seeker reaches target, stop
            if (grid.path.Count < 2)
            {
                Debug.Log("ACHEI VOCE");
                StopCoroutine(moveToTargetCoroutine);
                yield return null;
            }

            float totalDuration = 1f;

            // Calcula o tempo de início
            float startTime = Time.time;

            while (Time.time < startTime + totalDuration)
            {
                float elapsedTime = Time.time - startTime;

                float normalizedTime = elapsedTime / totalDuration;

                transform.position = Vector3.Lerp(transform.position, n.worldPosition + new Vector3(0, 0.4f, 0), normalizedTime);

                yield return null;

                if (grid.path[grid.path.Count - 1].worldPosition != oldPosition)
                {
                    break;
                }

                
            }

            /*for (float timer = 0; timer < 1; timer += 0.01f)
            {
                //if target moves, escape loop
                if (grid.path[grid.path.Count - 1].worldPosition != oldPosition)
                {
                    break;
                }

                //move to next node
                transform.position = Vector3.Lerp(transform.position, n.worldPosition + new Vector3(0, 0.4f, 0), timer);

                yield return new WaitForEndOfFrame();
            }*/
        }

        //if target moves, restart loop
        moveToTargetCoroutine = StartCoroutine(moveToTarget());
    }


    [ClientRpc]
    public void SetMyValueClientRpc(Vector3 newValue)
    {
        transformVar.Value = newValue; // This will sync automatically
    }
}
