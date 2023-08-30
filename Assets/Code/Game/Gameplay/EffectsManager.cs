using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour
{
    [Header("Road Connection:")]
    [SerializeField] private int startingRoadConnectionEffectsCount;
    [SerializeField] private GameObject roadConnectionEffectTemplate;
    [Space]
    [Header("Level Completed:")]
    [SerializeField] private GameObject barrierDestroyPSTemplate;
    [SerializeField] private List<GameObject> roadBarrier;

    private List<ParticleSystem> roadConnectionEffects;
    private ParticleSystem barrierDestroyPS;

    public int RoadBarrierCount { get { return roadBarrier.Count; } }

    public static EffectsManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        barrierDestroyPS = Instantiate(barrierDestroyPSTemplate, this.transform).GetComponent<ParticleSystem>();

        roadConnectionEffects = new List<ParticleSystem>();
        for(int i = 0; i < startingRoadConnectionEffectsCount; i++)
        {
            roadConnectionEffects.Add(Instantiate(roadConnectionEffectTemplate, this.transform).GetComponent<ParticleSystem>());
        }
    }

    //level completion
    public void DestroyBarriers()
    {
        StartCoroutine(DestroyBarriersCO());
    }

    private IEnumerator DestroyBarriersCO()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(0.3f); //wait for 1 frame, until after 'level ended' code has been executed
        for (int i = 0; i < roadBarrier.Count; i++)
        {
            SoundManager.SoundManagerInstance.RequestSound(SoundType.Explosion); //sound
            barrierDestroyPS.transform.localPosition = roadBarrier[i].transform.localPosition;
            barrierDestroyPS.Play();
            roadBarrier[i].SetActive(false);
            yield return wait;
        }
    }

    //road connection
    public void InstantiateRoadConenctionEffect(Vector3 position, Quaternion rotation)
    {
        SoundManager.SoundManagerInstance.RequestSound(SoundType.Connection);
        //look for free object
        for(int i = 0; i < roadConnectionEffects.Count; i++)
        {
            if(roadConnectionEffects[i].gameObject.activeSelf == false)
            {
                roadConnectionEffects[i].transform.localPosition = position;
                roadConnectionEffects[i].transform.localRotation = rotation;
                roadConnectionEffects[i].Play();
                return;
            }
        }

        //if no free object - add one
        roadConnectionEffects.Add(Instantiate(roadConnectionEffectTemplate, this.transform).GetComponent<ParticleSystem>());
        roadConnectionEffects[roadConnectionEffects.Count - 1].transform.localPosition = position;
        roadConnectionEffects[roadConnectionEffects.Count - 1].transform.localRotation = rotation;
        roadConnectionEffects[roadConnectionEffects.Count - 1].Play();
    }
}
