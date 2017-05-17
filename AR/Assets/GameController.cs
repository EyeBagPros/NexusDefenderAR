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
    public GameObject startButton;
    public GameObject resumeButton;
    public GameObject pauseButton;
    public GameObject restartButton;

    public GameState currentState;
    private float scale;

    private List<GameObject> enemies;

    public EnemySpawner spawner;
    public GameObject nexus;
    private float nexusRadius = 2f;
    private float spawnTimer = 0f;
    private float spawnFrequency = 1f;


    void Start ()
    {
        enemies = new List<GameObject>();
        enemies.Clear();

        currentState = GameState.MENU;
        startButton.SetActive(true);
        resumeButton.SetActive(false);
        pauseButton.SetActive(false);
        restartButton.SetActive(false);
    }
	
	void Update ()
    {
        scale = transform.lossyScale.y;
        //Debug.Log(scale);

        switch (currentState)
        {
            case GameState.PLAY:
                {
                    spawnTimer += Time.deltaTime;
                    if(spawnTimer >= spawnFrequency)
                    {
                        spawnTimer = 0f;
                        enemies.Add(spawner.SpawnEnemy(nexusRadius, scale, Mathf.Rad2Deg * Random.Range(0f, Mathf.PI * 2f)));
                    }
                    
                    break;
                }
            case GameState.TRANSITION_TO_PLAY:
                {
                    currentState = GameState.PLAY;
                    break;
                }
            case GameState.PAUSE:
                {
                    break;
                }
            case GameState.TRANSITION_TO_PAUSE:
                {
                    currentState = GameState.PAUSE;
                    break;
                }
            case GameState.MENU:
                {
                    break;
                }
            default: return;
        }
	}



    private void Restart()
    {
        currentState = GameState.MENU;

        // clean enemies
        foreach (GameObject enemy in enemies)
            Destroy(enemy);
        enemies.Clear();

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

    // UI ===================================================

    public void StartGame()
    {
        startButton.SetActive(false);
        pauseButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PLAY;
        spawnTimer = 0f;
}

    public void Pause()
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(true);
        restartButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PAUSE;
    }

    public void Resume()
    {
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseButton.SetActive(true);
        currentState = GameState.TRANSITION_TO_PLAY;
    }

    public void Reset()
    {
        restartButton.SetActive(false);
        resumeButton.SetActive(false);
        pauseButton.SetActive(false);
        startButton.SetActive(true);
        Restart();
    }
}
