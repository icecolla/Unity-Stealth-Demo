﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float m_health = 100.0f;
    public float m_resetAfterDeath = 5.0f; // pause to see player's death animation
    public AudioClip m_deathClip;

    private Animator m_anim;
    private PlayerMovement m_playerMovement;
    private HashIDs m_hash;
    private LastPlayerSighting m_lastPlayerSighting;
    private float m_timer;
    private bool m_playerDead;

    private AudioSource m_footstepsAudio;

    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_hash = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<HashIDs>();
        m_lastPlayerSighting = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();

        m_footstepsAudio = GetComponent<AudioSource>();
    }

	void Update ()
    {
        if (m_health <= 0.0f)
        {
            if (!m_playerDead)
            {
                PlayerDying();
            }
            else
            {
                PlayerDead();
                LevelReset();
            }
        }
	}

    void PlayerDying()
    {
        m_playerDead = true;

        m_anim.SetBool(m_hash.deadBool, m_playerDead);
        AudioSource.PlayClipAtPoint(m_deathClip, transform.position);
    }

    void PlayerDead()
    {
        //if (m_anim.GetCurrentAnimatorStateInfo(0).nameHash == m_hash.dyingState) {
        if (m_anim.GetCurrentAnimatorStateInfo(0).fullPathHash == m_hash.dyingState)
        {
            m_anim.SetBool(m_hash.deadBool, false);
        }

        // prevents player from moving once "death" is true.
        m_anim.SetFloat(m_hash.speedFloat, 0.0f);
        m_playerMovement.enabled = false;

        // reset the last global sighting position of the player so the alarms will switch off
        m_lastPlayerSighting.m_position = m_lastPlayerSighting.m_resetPosition;

        // stops the footsteps sounds
        m_footstepsAudio.Stop();
    }

    void LevelReset()
    {
        m_timer += Time.deltaTime;

        // only fades the scene once the timer is done for the "death" animation
        if (m_timer >= m_resetAfterDeath)
        {
           SceneManager.LoadScene(0);
        }
    }

    public void TakeDamage(float amount)
    {
        m_health -= amount;
    }
}