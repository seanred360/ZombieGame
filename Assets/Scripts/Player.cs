using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : TurnManager
{
    PlayerManager m_playerManager;
    public int playerNumber;

    // global flag for enabling and disabling user input
    bool m_inputEnabled = false;
    public bool InputEnabled { get { return m_inputEnabled; } set { m_inputEnabled = value; } }

    protected override void Awake()
    {
        base.Awake();
        m_playerManager = gameObject.GetComponent<PlayerManager>();
    }

    private void Update()
    {
        if (m_inputEnabled)
        {
            m_playerManager.m_canMove = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "House")
            FinishTurn();
    }

    public void PlayTurn()
    {
        m_inputEnabled = true;
        m_playerManager.m_canMove = true;
        Debug.Log(gameObject.name + "'s turn");
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
