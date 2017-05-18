using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public GameObject baseProjectile;
    
    public ProjectileController SpawnProjectile(Transform shooter, EnemyController target)
    {
        GameObject copy = Instantiate(baseProjectile, shooter.transform.position, baseProjectile.transform.rotation, transform);
        copy.SetActive(true);

        if(target != null) // turret shot
        {
            Vector3 pos = copy.transform.position;
            pos += shooter.up * 12f;
            copy.transform.position = pos;
        }

        return copy.GetComponentInChildren<ProjectileController>();
    }
}
