using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;

[System.Serializable]
public enum Turn
{
    Player,
    Enemy
}
public class GameManager : MonoBehaviour
{
    // reference to PlayerManager
    public Player m_player1;

    public List<Enemy> m_enemies;
    public List<Player> m_players;
    public List<House> m_houses;

    public Transform[] m_spawnPoints;
    public Transform[] m_enemySpawnPoints;
    public AudioManager m_audioManager;
    public GameObject m_roundPrefab;
    public NavMeshSurface m_surface;

    float roundLocation;
    Vector3 m_cameraTarget;

    Camera cam,zombieCam;
    bool m_canMoveCamera;

    Turn m_currentTurn = Turn.Player;
    public Turn CurrentTurn { get { return m_currentTurn; } }

    // has the user pressed start?
    bool m_hasLevelStarted = false;
    public bool HasLevelStarted { get { return m_hasLevelStarted; } set { m_hasLevelStarted = value; } }

    // have we begun gamePlay?
    bool m_isGamePlaying = false;
    public bool IsGamePlaying { get { return m_isGamePlaying; } set { m_isGamePlaying = value; } }

    // have we met the game over condition?
    bool m_isGameOver = false;
    public bool IsGameOver { get { return m_isGameOver; } set { m_isGameOver = value; } }

    // have the end level graphics finished playing?
    bool m_hasLevelFinished = false;
    public bool HasLevelFinished { get { return m_hasLevelFinished; } set { m_hasLevelFinished = value; } }

    // delay in between game stages
    public float delay = 1f;

    // events invoked for StartLevel/PlayLevel/EndLevel coroutines
    public UnityEvent setupEvent;
    public UnityEvent startLevelEvent;
    public UnityEvent playLevelEvent;
    public UnityEvent endLevelEvent;
    public UnityEvent loseLevelEvent;


    void Awake()
    {
        m_enemies = (Object.FindObjectsOfType<Enemy>() as Enemy[]).ToList();
        m_houses = (Object.FindObjectsOfType<House>() as House[]).ToList();
        m_audioManager = (Object.FindObjectOfType<AudioManager>().GetComponent<AudioManager>());
        cam = Camera.main;
        zombieCam = GameObject.FindGameObjectWithTag("ZombieCamera").GetComponent<Camera>();
        zombieCam.gameObject.SetActive(false);
        m_cameraTarget = new Vector3(0f, 10f, -9);
    }

    void Start()
    {
        StartCoroutine("RunGameLoop");
    }

    void Update()
    {
        if (m_canMoveCamera)
        {
            float step = 5f * Time.deltaTime; // calculate distance to move
            cam.transform.position = Vector3.MoveTowards(cam.transform.position, m_cameraTarget, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(cam.transform.position, m_cameraTarget) < 0.001f)
            {
                // Swap the position of the cylinder.
                cam.transform.position = m_cameraTarget;
                m_canMoveCamera = false;
            }
        }
    }
    // run the main game loop, separated into different stages/coroutines
    IEnumerator RunGameLoop()
    {
        yield return StartCoroutine("StartLevelRoutine");
        yield return StartCoroutine("PlayLevelRoutine");
        yield return StartCoroutine("EndLevelRoutine");
    }

    // the initial stage after the level is loaded
    IEnumerator StartLevelRoutine()
    {
        Debug.Log("SETUP LEVEL");
        if (setupEvent != null)
        {
            setupEvent.Invoke();
        }

        Debug.Log("START LEVEL");
        m_player1.InputEnabled = false;

        while (!m_hasLevelStarted)
        {
            //show start screen
            // user presses button to start
            // HasLevelStarted = true
            yield return null;
        }

        // trigger events when we press the StartButton
        if (startLevelEvent != null)
        {
            startLevelEvent.Invoke();
        }
    }

    // gameplay stage
    IEnumerator PlayLevelRoutine()
    {
        Debug.Log("PLAY LEVEL");
        m_isGamePlaying = true;
        yield return new WaitForSeconds(delay);
        m_player1.InputEnabled = true;

        // trigger any events as we start playing the level
        if (playLevelEvent != null)
        {
            playLevelEvent.Invoke();
        }

        while (!m_isGameOver)
        {
            // pause one frame
            yield return null;

            // check for level win condition
            m_isGameOver = IsWinner();

            // check for the lose condition
        }
        // Debug.Log("WIN! ==========================");
    }

    public void LoseLevel()
    {
        StartCoroutine(LoseLevelRoutine());
    }

    // trigger the "lose" condition
    IEnumerator LoseLevelRoutine()
    {
        // game is over
        m_isGameOver = true;

        // wait for a short delay then...
        yield return new WaitForSeconds(1.5f);

        // ...invoke loseLoveEvent
        if (loseLevelEvent != null)
        {
            loseLevelEvent.Invoke();
        }

        // pause for two seconds and then restart the level
        yield return new WaitForSeconds(2f);

        Debug.Log("LOSE! =============================");

        RestartLevel();
    }

    // end stage after gameplay is complete
    IEnumerator EndLevelRoutine()
    {
        Debug.Log("END LEVEL");
        m_player1.InputEnabled = false;

        // run events when we end the level
        if (endLevelEvent != null)
        {
            endLevelEvent.Invoke();
        }

        // show end screen
        while (!m_hasLevelFinished)
        {
            // user presses button to continue

            // HasLevelFinished = true
            yield return null;
        }

        // reload the current scene
        RestartLevel();
    }

    // restart the current level
    void RestartLevel()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // attach to StartButton, triggers PlayLevelRoutine
    public void PlayLevel()
    {
        m_hasLevelStarted = true;
    }

    // has the player reached the goal node?
    bool IsWinner()
    {
        return false;
    }

    // switch to Player turn
    void PlayPlayerTurn()
    {
        m_currentTurn = Turn.Player;

        foreach (Player player in m_players)
        {
            if (player != null && !player.IsTurnComplete)
            {
                player.PlayTurn();
                break;
            }
        }
    }

    // switch to Enemy turn
    void PlayEnemyTurn()
    {
        m_currentTurn = Turn.Enemy;

        foreach (Enemy enemy in m_enemies)
        {
            if (enemy != null)
            {
                enemy.IsTurnComplete = false;

                enemy.StartPlayTurn();
             
                m_audioManager.PlaySFX(1);
            }
        }
    }

    // have all of the players completed their turns?
    bool AllPlayerTurnsComplete()
    {
        foreach (Player player in m_players)
        {
            if (!player.IsTurnComplete)
            {
                return false;
            }
        }
        Debug.Log("all players finished");
        return true;
    }

    // have all of the enemies completed their turns?
    bool AllEnemyTurnsComplete()
    {
        foreach (Enemy enemy in m_enemies)
        {
            if (!enemy.IsTurnComplete)
            {
                return false;
            }
        }
        Debug.Log("all enemies finished");
        return true;
    }

    // switch between Player and Enemy Turns
    public void UpdateTurn()
    {
        Debug.Log("update turn");
            bool allPlayerTurnsComplete = AllPlayerTurnsComplete();
            bool allEnemyTurnsComplete = AllEnemyTurnsComplete();

            if (allPlayerTurnsComplete)
            {
                PlayEnemyTurn();
                Debug.Log(" all player turns complete, play enemy turn");
            }
            else
            {
                PlayPlayerTurn();
                Debug.Log("next player turn");
            }
    }

    public void StartNextRound()
    {
        StartCoroutine(StartNextRoundRoutine());
    }

    IEnumerator StartNextRoundRoutine()
    {
        Debug.Log("Start next round");
        yield return new WaitForSeconds(2f);
        ChangeCamera();
        m_cameraTarget += new Vector3(0f, 0f, 22);
        roundLocation += 22;
        m_canMoveCamera = true;

        for (int i = 0; i < m_spawnPoints.Length; i++)
        {
            m_spawnPoints[i].transform.position += new Vector3(0f, 0f, 22f);
        }

        for (int i = 0; i < m_enemySpawnPoints.Length; i++)
        {
            m_enemySpawnPoints[i].transform.position += new Vector3(0f, 0f, 22f);
        }

        for (int i = 0; i < m_houses.Count; i++)
        {
            m_houses[i].gameObject.SetActive(false);
        }
        m_houses.Clear();

        Instantiate(m_roundPrefab, new Vector3 (0f, 0f, roundLocation),Quaternion.identity);
        m_surface.BuildNavMesh();
        m_houses = (Object.FindObjectsOfType<House>() as House[]).ToList();

        for (int i = 0; i < m_players.Count; i++)
        {
            //m_players[i].transform.position = m_spawnPoints[i].transform.position;
            m_players[i].gameObject.SetActive(true);
            m_players[i].agent.SetDestination(m_spawnPoints[i].transform.position);
        }

        for (int i = 0; i < m_enemies.Count; i++)
        {
            //m_enemies[i].transform.position = m_enemySpawnPoints[i].transform.position;
            m_enemies[i].gameObject.SetActive(true);
            m_enemies[i].m_enemyManager.agent.SetDestination(m_enemySpawnPoints[i].transform.position);
        }
        yield return new WaitForSeconds(3f);
        foreach(Player player in m_players){player.InputEnabled = false; player.IsTurnComplete = false; }
        foreach (Enemy enemy in m_enemies) { enemy.InputEnabled = false; enemy.IsTurnComplete = false; }
        m_player1.PlayTurn();
    }

    void ChangeCamera()
    {
        if (cam.gameObject.activeSelf == false)
        {
            cam.gameObject.SetActive(true);
            zombieCam.gameObject.SetActive(false);
        }
        else
        {
            zombieCam.gameObject.SetActive(true);
            cam.gameObject.SetActive(false);
        }
    }
}
