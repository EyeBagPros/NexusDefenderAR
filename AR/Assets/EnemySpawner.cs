using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject baseEnemy;

    public GameObject SpawnEnemy(float nexusRadius, float scale, float angle)
    {
        GameObject copy = Instantiate(baseEnemy, baseEnemy.transform.position, baseEnemy.transform.rotation, transform);
        copy.SetActive(true);

        EnemyController ec = copy.GetComponentInChildren<EnemyController>();
        GameObject goblin = ec.gameObject;

        //goblin.transform.position = tmp;

        Vector3 tmp;
        tmp.x = tmp.y = tmp.z = 0f;

        goblin.transform.Translate(Quaternion.AngleAxis(angle, Vector3.up) * (Vector3.forward * nexusRadius * scale));


        //Vector2 randPos = Random.insideUnitCircle * nexusRadius;




        //ec.SetTarget(copy.transform.position + ( * nexusRadius * scale));

        Debug.Log(angle);

        return copy;
    }
}
