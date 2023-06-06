using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeController : MonoBehaviour
{
    public int targetFireCount;

    private GameManager gameManager;
    private AudioManager audioManager;
    private int fireCounter;
    private bool doComplete = true;
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    void Update()
    {
        if (fireCounter >= targetFireCount && doComplete)
        {
            gameManager.StartCoroutine(gameManager.CompleteWorld(GameManager.World.Forest));
            audioManager.StopSound("Fire Crackle");
            doComplete = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Fire"))
        {
            fireCounter++;
        }
    }
}
