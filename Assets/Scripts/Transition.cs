using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition : MonoBehaviour
{

    public GameObject top;
    public GameObject bottom;

    public GameManager gameManager;

    public Camera mainCamera;

    private float speed;
    void Start()
    {
    }

    void Update()
    {
        transform.localScale = Vector3.one * mainCamera.orthographicSize;
        speed = 2 * mainCamera.orthographicSize;
    }
    public IEnumerator StartTransition()
    {
        gameObject.SetActive(true);
        gameManager.isInTransit = true;
        while (top.transform.localPosition.y > 1)
        {
            top.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
            bottom.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
            yield return null;
        }
        yield break;
    }
    public IEnumerator EndTransition()
    {
        while (top.transform.localPosition.y < 2.1f)
        {
            top.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
            bottom.transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
            yield return null;
        }
        gameObject.SetActive(false);
        gameManager.isInTransit = false;
        yield break;
    }
}
