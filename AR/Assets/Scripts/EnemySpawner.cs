using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // Enemy Prefab
    public GameObject baseEnemy;

    /* Instanciates Enemy
    distance is the distance from the nexus the enemy spawns
    nexusRadius is the width of the nexus from wich to set the target to move and attack
    scale is used to standarize values
    angle determines where from the nexus the enemy spawns the distance given*/
    public GameObject SpawnEnemy(float distance, float nexusRadius, float scale, float angle)
    {
        GameObject copy = Instantiate(baseEnemy, baseEnemy.transform.position, baseEnemy.transform.rotation, transform);
        copy.SetActive(true);

        EnemyController ec = copy.GetComponentInChildren<EnemyController>();
        ec.SetTarget(nexusRadius, scale, angle);

        GameObject goblin = ec.gameObject;
        goblin.transform.Rotate(0f, angle, 0f);
        goblin.transform.Translate(Vector3.forward * distance * scale);
        goblin.transform.Rotate(0f, 180, 0f);

        return copy;
    }
}
