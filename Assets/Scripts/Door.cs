using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{
    public GameManager.World otherWorld;

    private float doorSpeed = 1f;

    private GameObject left;
    private GameObject right;

    private bool doOpenDoor;
    private bool doCloseDoor;

    private GameManager gameManager;
    private AudioManager audioManager;
    void Start()
    {
        left = transform.Find("Left").gameObject;
        right = transform.Find("Right").gameObject;

        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }
    private void FixedUpdate()
    {
        
    }
    void Update()
    {

        if (doOpenDoor && left.transform.localScale.x > 0)
        {
            left.transform.localScale -= new Vector3(doorSpeed * Time.deltaTime, 0, 0);
            left.transform.localPosition -= new Vector3(doorSpeed / 2 * Time.deltaTime, 0, 0);
            right.transform.localScale -= new Vector3(doorSpeed * Time.deltaTime, 0, 0);
            right.transform.localPosition += new Vector3(doorSpeed * Time.deltaTime / 2, 0, 0);
        }
        else if (doCloseDoor && left.transform.localScale.x <= 0.2f)
        {
            left.transform.localScale += new Vector3(doorSpeed * Time.deltaTime, 0, 0);
            left.transform.localPosition += new Vector3(doorSpeed * Time.deltaTime / 2, 0, 0);
            right.transform.localScale += new Vector3(doorSpeed * Time.deltaTime, 0, 0);
            right.transform.localPosition -= new Vector3(doorSpeed * Time.deltaTime / 2, 0, 0);
        }
        if (left.transform.localScale.x < 0)
        {
            left.transform.localScale = new Vector3(0, left.transform.localScale.y, 0);
            left.transform.localPosition = new Vector3(-0.2f, 0, 0);
            right.transform.localScale = new Vector3(0, right.transform.localScale.y, 0);
            right.transform.localPosition = new Vector3(0.2f, 0, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioManager.PlaySound("Door Open");
            gameManager.newWorld = otherWorld;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doOpenDoor = true;
            doCloseDoor = false;
            gameManager.isDoorOpen = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            doOpenDoor = false;
            doCloseDoor = true;
            audioManager.PlaySound("Door Close");
        }
    }
}
