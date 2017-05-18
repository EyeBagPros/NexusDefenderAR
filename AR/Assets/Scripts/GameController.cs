using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    PLAY,
    TRANSITION_TO_PLAY,
    PAUSE,
    TRANSITION_TO_PAUSE,
    END,
    MENU
}

public class GameController : MonoBehaviour
{
    // UI
    public GameObject startButton;
    public GameObject startPanel;
    public GameObject resumeButton;
    public GameObject pauseButton;
    public GameObject restartButton;
    public GameObject pausePanel;
    public GameObject warning;
    public GameObject deathPanel;
    public Text EndCount;
    public Text killCountLabel;
    public Image nexusLife;

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
    private int killCount;

    // Enemy Spawning
    private List<EnemyController> enemies;
    public EnemySpawner enemySpawner;
    private float spawnTimer;
    private float spawnFrequency;

    // Enemy Stats
    private float enemyBaseHealth = 500f;
    private float enemyBaseAttackSpeed = 1f;
    private float enemyBaseSpeed = 25f;
    private float enemyBaseDamage = 10f;

    // Player
    private float damage;

    // Turrets
    public TurretController[] turrets;
    private float turretBaseDamage = 100f;
    private float turretBaseAttackSpeed = 10f;
    private float turretBaseRange = 100f;

    // Projectiles & Explosions
    private float projectileSpeed;
    public ProjectileSpawner projectileSpawner;
    public ExplosionSpawner explosionSpawner;

    // Tracking resources
    private Dictionary<string, bool> trackingImages;
    private string card0;
    private string card1;
    private string card2;
    private string card3;
    public GameObject track1;
    public GameObject track2;
    public GameObject track3;
    
    // Camera
    public Camera cam;


    void Start ()
    {
        enemies = new List<EnemyController>();

        card0 = "cartaUNO";
        card1 = "virus1";
        card2 = "body";
        card3 = "kit";
        trackingImages = new Dictionary<string, bool>();
        trackingImages.Add(card0, false);
        trackingImages.Add(card1, false);
        trackingImages.Add(card2, false);
        trackingImages.Add(card3, false);

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
                    if (nexusHealth > 0f)
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
                    if (trackingImages[card0])
                    {
                        warning.SetActive(false);
                    }

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
        startPanel.SetActive(true);
        resumeButton.SetActive(false);
        pauseButton.SetActive(false);
        restartButton.SetActive(false);
        deathPanel.SetActive(false);
        nexusLife.transform.parent.gameObject.SetActive(false);
        nexusLife.fillAmount = 1f;

        // Clean enemies
        foreach (EnemyController enemy in enemies)
            Destroy(enemy.gameObject);
        enemies.Clear();
        
        trackingImages[card0] = false;
        trackingImages[card1] = false;
        trackingImages[card2] = false;
        trackingImages[card3] = false;

        enemyBaseHealth = 500f;
        enemyBaseAttackSpeed = 1f;
        enemyBaseSpeed = 15f;

        turretBaseDamage = 100f;
        turretBaseAttackSpeed = 2.2f;
        turretBaseRange = 100f;

        damage = 300f;

        currentWave = 0;
        projectileSpeed = 25f;
        nexusHealth = nexusMaxHealth = 1000f;
        spawnTimer = 100f;
        spawnFrequency = 1f;


        // Reset turrets
        foreach (TurretController turret in turrets)
            turret.SetAllAttributes(this, turretBaseDamage, turretBaseAttackSpeed, turretBaseRange);
    }


    private void HandleLoop()
    {
        // handle cards
        if (!trackingImages[card0])
        {
            Pause();
            warning.SetActive(true);
        }

        if (trackingImages[card1])
        {
            float originalY = turrets[0].transform.localPosition.y;
            turrets[0].transform.position = track1.transform.position;
            Vector3 pos = turrets[0].transform.localPosition;
            pos.y = originalY;
            turrets[0].transform.localPosition = pos;
        }

        if (trackingImages[card2])
        {
            float originalY = turrets[1].transform.localPosition.y;
            turrets[1].transform.position = track2.transform.position;
            Vector3 pos = turrets[1].transform.localPosition;
            pos.y = originalY;
            turrets[1].transform.localPosition = pos;
        }

        if (trackingImages[card3])
        {
            float originalY = turrets[2].transform.localPosition.y;
            turrets[2].transform.position = track3.transform.position;
            Vector3 pos = turrets[2].transform.localPosition;
            pos.y = originalY;
            turrets[2].transform.localPosition = pos;
        }


        if(Input.touchCount > 0)
        {
            Vector3 aim = cam.transform.forward;
            aim.x = Input.GetTouch(0).position.x;
            aim.y = Input.GetTouch(0).position.y;

            ProjectileController proj = projectileSpawner.SpawnProjectile(cam.transform, aim, scale);
            proj.SetAttributes(this, projectileSpeed, null, damage);
        }
        else if(Input.GetMouseButtonDown(0))
        {
            ProjectileController proj = projectileSpawner.SpawnProjectile(cam.transform, Input.mousePosition, scale);
            proj.SetAttributes(this, projectileSpeed, null, damage);
        }





        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnFrequency)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }
    }

    // Enemy Spawn ===================================================

    private void SpawnEnemy()
    {
        float distance = 80f;
        float nexusRadius = Vector3.Distance(nexus.transform.position, nexusEdge.transform.position);
        float angle = Mathf.Rad2Deg * Random.Range(0f, Mathf.PI * 2f);

        //angle = 0f;

        EnemyController enemy = enemySpawner.SpawnEnemy(distance, nexusRadius, scale, angle);
        enemy.SetAtt(enemyBaseHealth, enemyBaseAttackSpeed, enemyBaseSpeed, enemyBaseDamage);
        enemies.Add(enemy);
    }

    // Projectiles & Explosions ===================================================

    public void SpawnProjectile(Transform shooter, EnemyController target, float d)
    {
        ProjectileController proj = projectileSpawner.SpawnProjectile(shooter, target);
        proj.SetAttributes(this, projectileSpeed, target, d);
    }

    public void SpawnExplosion(Transform t)
    {
        explosionSpawner.SpawnExplosion(t);
    }

    public void AddKill()
    {
        killCount += 1;
        killCountLabel.text = killCount.ToString();
    }

    public void HitNexus(float d)
    {
        nexusHealth -= d;
        nexusLife.fillAmount = nexusHealth / nexusMaxHealth;

        if(nexusHealth <= 0f)
        {
            pauseButton.SetActive(false);
            killCountLabel.transform.parent.gameObject.SetActive(false);
            deathPanel.SetActive(true);
            EndCount.text = killCount.ToString();
            currentState = GameState.MENU;
        }
    }

    public void RemoveEnemy(EnemyController ec)
    {
        enemies.Remove(ec);
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
        bool cardActive;
        if (trackingImages.TryGetValue(name, out cardActive))
        {
            if(cardActive != state)
            {
                trackingImages[name] = state;
            }
        }
        else
        {
            Debug.Log("Card" + name + "is unregistered");
        }
    }

    public EnemyController GetClosestTarget(Transform turret, float range)
    {
        EnemyController ret = null;

        float closestDistance = range;
        float distance = 0f;

        foreach (EnemyController enemy in enemies)
        {
            if (enemy.Targetable())
            {
                distance = Vector3.Distance(enemy.transform.position, turret.position);

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
        nexusLife.transform.parent.gameObject.SetActive(true);
        currentState = GameState.TRANSITION_TO_PLAY;
        tickSkipped = false;

        killCount = 0;
        killCountLabel.text = killCount.ToString();
    }

    public void Pause()
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        restartButton.SetActive(true);
        pausePanel.SetActive(true);
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

    public void Exit()
    {
        Application.Quit();
    }
}
