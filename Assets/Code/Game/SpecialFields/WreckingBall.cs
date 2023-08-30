using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBall : SpecialField
{
    [SerializeField] private Transform ball;
    [SerializeField] private ParticleSystem dustParticles;

    public override void OnUse()
    {
        StopAllCoroutines();
        StartCoroutine(Wreck());
    }

    void Start()
    {
        this.Command = new Destroy(CommandType.Destroy, this);
    }

    private IEnumerator Wreck()
    {
        while(ball.position.y != 0)
        {
            ball.localPosition = Vector3.MoveTowards(ball.localPosition, Vector3.zero, 40 * Time.deltaTime);
            yield return null;
        }
        sound.Play();
        dustParticles.Play();
        while (ball.position.y != 10)
        {
            ball.localPosition = Vector3.MoveTowards(ball.localPosition, Vector3.up * 10, 5 * Time.deltaTime);
            yield return null;
        }
    }
}
