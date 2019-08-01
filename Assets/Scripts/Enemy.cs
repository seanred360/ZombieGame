using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TurnManager
{
    EnemyManager m_enemyManager;

    // global flag for enabling and disabling user input
    bool m_inputEnabled = false;
    public bool InputEnabled { get { return m_inputEnabled; } set { m_inputEnabled = value; } }

    protected override void Awake()
    {
        base.Awake();
        m_enemyManager = gameObject.GetComponent<EnemyManager>();
    }

    private void Update()
    {
        if (m_inputEnabled)
        {
            m_enemyManager.m_canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "House")
        {
            if (other.gameObject.GetComponent<House>().m_ReceivedPlayers.Count > 0)
            {
                KillPlayers();
            }
            FinishTurn();
        }
    }

    // play the Enemy's turn routine
    public void StartPlayTurn()
    {
        Debug.Log(gameObject.name + "'s turn");
        m_enemyManager.PlayTurn();
    }

    void KillPlayers()
    {
        m_gameManager.m_audioManager.PlaySFX(0);
    }

    // override the TurnManager's FinishTurn
    public override void FinishTurn()
    {
        // tell the GameManager the PlayerTurn is complete
        base.FinishTurn();
        m_isTurnComplete = true;
        Debug.Log("finished turn");
        gameObject.SetActive(false);
    }
}
