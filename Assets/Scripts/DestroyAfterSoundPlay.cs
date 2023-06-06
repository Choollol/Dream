using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSoundPlay : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
