using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerMovement : MonoBehaviour
{
    private Coroutine moveToTargetCoroutine;

    public NodeGrid grid;
    // Start is called before the first frame update
    void Start()
    {
        moveToTargetCoroutine = StartCoroutine(moveToTarget());
    }

    // Update is called once per frame
    void Update()
    {
        
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
                Debug.Log("PORQUEEEEE");

            }
            for (float timer = 0; timer < 1; timer += 0.01f)
            {
                //if target moves, escape loop
                if (grid.path[grid.path.Count - 1].worldPosition != oldPosition)
                {
                    break;
                }

                //move to next node
                transform.position = Vector3.Lerp(transform.position, n.worldPosition + new Vector3(0, 0.4f, 0), timer);

                yield return new WaitForEndOfFrame();
            }
        }

        //if target moves, restart loop
        moveToTargetCoroutine = StartCoroutine(moveToTarget());
    }
}
