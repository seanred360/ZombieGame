using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : TurnManager
{
    public EnemyManager m_enemyManager;

    protected override void Awake()
    {
        base.Awake();
        m_enemyManager = gameObject.GetComponent<EnemyManager>();
    }

    private void Update()
    {
        if (InputEnabled)
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
                KillPlayers(other.gameObject.GetComponent<House>());
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

    void KillPlayers(House house)
    {
        m_gameManager.m_audioManager.PlaySFX(0);
        foreach (Player player in house.m_ReceivedPlayers)
        {
            player.health -= 1;
            Debug.Log(player.gameObject.name + " -1 health");
        }
    }

    // override the TurnManager's FinishTurn
    public override void FinishTurn()
    {
        // tell the GameManager the PlayerTurn is complete
        base.FinishTurn();
        m_isTurnComplete = true;
        Debug.Log("finished turn");
        m_gameManager.StartNextRound();
        gameObject.SetActive(false);
    }
}
