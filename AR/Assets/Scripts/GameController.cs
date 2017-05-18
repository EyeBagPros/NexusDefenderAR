using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PLAY,
    TRANSITION_TO_PLAY,
    PAUSE,
    TRANSITION_TO_PAUSE,
    TARGET_NOT_FOUND,
    MENU
}

public class GameController : MonoBehaviour
{
    // UI
    public GameObject startButton;
    public GameObject resumeButton;
    public GameObject pauseButton;
    public GameObject restartButton;

    // General Values
    public GameState currentState;
    private bool tickSkipped;
    private float scale;

    // Nexus
    public GameObject nexus;
    public GameObject nexusEdge;
    private float nexusHealth;
    private float nexusMaxHealth;

    //Waves
    private int currentWave;

    // Enemy Spawning
    private List<GameObject> enemies;
    public EnemySpawner spawner;
    private float spawnTimer;
    private float spawnFrequency;

    // Player
    private float damage;

    // Turrets
    public TurretController[] turrets;
    private float turretBaseDamage = 10f;
    private float turretBaseAttackSpeed = 1f;
    private float turretBaseRange = 50f;

    // Projectiles & Explosions
    private float projectileSpeed;
    public GameObject projectileBase;
    public GameObject projectileHolder;
    public GameObject explosionBase;
    public GameObject explosionHolder;

    // Tracking resources
    private Dictionary<string, bool> trackingImages;
    private string mainCard;
    private string card1;

    // Camera
    public Camera cam;


    void Start ()
    {
        enemies = new List<GameObject>();

        mainCard = "cartaUNO";
        card1 = "carta2";
        trackingImages = new Dictionary<string, bool>();
        trackingImages.Add(mainCard, false);
        trackingImages.Add(card1, false);
        
        Restart();
}
	
	void Update ()
    {
        scale = transform.lossyScale.y;
        //Debug.Log(scale);

        switch (currentState)
        {
            case GameState.PLAY:
                {
                    HandleLoop();
                    break;
                }
            case GameState.TRANSITION_TO_PLAY:
                {
                    if (tickSkipped)
                        currentState = GameState.PLAY;
                    else
                        tickSkipped = !tickSkipped;

                    break;
                }
            case GameState.PAUSE:
                {
                    break;
                }
            case GameState.TRANSITION_TO_PAUSE:
                {
                    if (tickSkipped)
                        currentState = GameState.PAUSE;
                    else
                        tickSkipped = !tickSkipped;

                    break;
                }
            case GameState.MENU:
                {
                    break;
                }
            default: return;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            SpawnExplosion(other.transform);
            Destroy(other.gameObject);
        }
    }

    private void Restart()
    {
        // Reset UI to menu
        currentState = GameState.MENU;
        startButton.SetActive(true);
        resumeButton.SetActive(false);
        pauseButton.SetActive(false);
        restartButton.SetActive(false);

        // Clean enemies
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
        enemies.Clear();

        // Reset turrets
        foreach (TurretController turret in turrets)
            turret.SetAllAttributes(this, turretBaseDamage, turretBaseAttackSpeed, turretBaseRange);


        damage = 10f;

        currentWave = 0;

        nexusHealth = nexusMaxHealth = 100f;
        spawnTimer = 100f;
        spawnFrequency = 20f;
    }


    private void HandleLoop()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnFrequency)
        {
            spawnTimer = 0f;

            float distance = 80f;
            float nexusRadius = Vector3.Distance(nexus.transform.position, nexusEdge.transform.position);
            float angle = Mathf.Rad2Deg * Random.Range(0f, Mathf.PI * 2f);

            angle = 0f;

            enemies.Add(spawner.SpawnEnemy(distance, nexusRadius, scale, angle));
        }
    }


    // Projectiles ===================================================

    public void SpawnProjectile(Transform shooter, GameObject target, float d)
    {
        GameObject proj = Instantiate(projectileBase, projectileHolder.transform, false);

        proj.transform.position = shooter.position;

        if (target == null) // player shot
        {
            proj.transform.rotation = cam.transform.rotation;
        }
        else // turret shot
        {
            Vector3 pos = proj.transform.position;
            pos += proj.transform.up * proj.transform.localScale.y;
            proj.transform.position = pos;
        }

        ProjectileController pc = proj.GetComponent<ProjectileController>();
        pc.SetAttributes(this, projectileSpeed, target, d);
    }

    public void SpawnExplosion(Transform t)
    {
        //Instantiate(explosionBase, explosionHolder.transform, false);
    }


    // Utility ===================================================

    public GameState GetState()
    {
        return currentState;
    }

    public float GetScale()
    {
        return scale;
    }

    public void UpdateTrackingImageStates(string name, bool state)
    {
        //Debug.Log("GAMECONTROLLER RECIEVED: " + name + " was: " + state);

        bool cardActive;
        if (trackingImages.TryGetValue(name, out cardActive))
        {
            
        }
        else
        {
            Debug.Log("Card" + name + "is unregistered");
        }
    }

    public GameObject GetClosestTarget(Transform turret, float range)
    {
        Debug.Log("REQUEST NEW TARGET");
        GameObject ret = null;

        float closestDistance = range;
        float distance = 0f;

        foreach (GameObject enemy in enemies)
        {
            EnemyController ec = enemy.GetComponentInChildren<EnemyController>();

            if(ec == null)
                Debug.Log("TARGET HAS NO CONTROLLER: ");

            if (ec != null && ec.Targetable())
            {
                distance = Vector3.Distance(enemy.transform.position, turret.position);
                Debug.Log("TARGETABLE TARGET AT: " + distance.ToString());

                if (distance <= closestDistance)
                {
                    ret = enemy;
                    closestDistance = distance;
                }
            }
        }

        return ret;
    }

    // UI ===================================================

    public void StartGame()
    {
        startButton.SetActive(false);
        pauseButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PLAY;
        tickSkipped = false;
}

    public void Pause()
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        restartButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PAUSE;
        tickSkipped = false;
    }

    public void Resume()
    {
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PLAY;
        tickSkipped = false;
    }

    public void Reset()
    {
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseButton.SetActive(false);
        startButton.SetActive(true);
        tickSkipped = false;
        Restart();
    }
}
