using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void RestartParticlePlayback()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Play();
        }
    }
}
