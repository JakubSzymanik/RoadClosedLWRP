using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuParticleManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem pointGainedParticles;
    Queue<Vector2> pointGainedSpawnPoints;

    public static MenuParticleManager instance;

    private void Awake()
    {
        instance = this;
        pointGainedSpawnPoints = null;
    }

    public void RequestPointGainedParticles(Vector2 pos)
    {
        if(pointGainedSpawnPoints == null)
        {
            pointGainedSpawnPoints = new Queue<Vector2>();
            pointGainedSpawnPoints.Enqueue(pos);
            StartCoroutine(LaunchParticles());
        } else
        {
            pointGainedSpawnPoints.Enqueue(pos);
        }
    }

    private IEnumerator LaunchParticles()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        yield return null;
        while(pointGainedSpawnPoints.Count > 0)
        {
            pointGainedParticles.transform.position = 
                Camera.main.ScreenToWorldPoint( (Vector3)pointGainedSpawnPoints.Dequeue() ) + Vector3.forward;
            pointGainedParticles.Play();
            yield return wait;
        }
    }
}
