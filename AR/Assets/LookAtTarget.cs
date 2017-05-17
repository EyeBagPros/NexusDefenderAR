using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    private GameObject target = null;
	
	void Update ()
    {
        if(target != null)
            transform.LookAt(target.transform.position);
	}

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
    }
}
