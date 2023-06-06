using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;

    private Camera mainCamera;
    private PlayerController playerController
    {
        get { return GameObject.Find("Player").GetComponent<PlayerController>(); }
    }
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (gameManager.world != GameManager.World.Doors)
        {
            transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y, transform.position.z);
            if (transform.position.x - mainCamera.orthographicSize * mainCamera.aspect < gameManager.cameraBounds[0])
            {
                transform.position = new Vector3(gameManager.cameraBounds[0] + mainCamera.orthographicSize * mainCamera.aspect,
                    transform.position.y, transform.position.z);
            }
            else if (transform.position.x + mainCamera.orthographicSize * mainCamera.aspect > gameManager.cameraBounds[1])
            {
                transform.position = new Vector3(gameManager.cameraBounds[1] - mainCamera.orthographicSize * mainCamera.aspect,
                    transform.position.y, transform.position.z);
            }
            if (transform.position.y - mainCamera.orthographicSize < gameManager.cameraBounds[2])
            {
                transform.position = new Vector3(transform.position.x, gameManager.cameraBounds[2] + mainCamera.orthographicSize, transform.position.z);
            }
            else if (transform.position.y + mainCamera.orthographicSize > gameManager.cameraBounds[3])
            {
                transform.position = new Vector3(transform.position.x, gameManager.cameraBounds[3] - mainCamera.orthographicSize, transform.position.z);
            }
        }
    }
}
