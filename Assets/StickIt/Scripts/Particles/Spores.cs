using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Spores : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.Particle[] particles;
    ParticleDatas[] particlesDatas;
    ParticleSystem.EmissionModule emission;

    Player[] losers;

    bool isKillerState;

    int[] savePlayerId;

    public float durationParticleToPlayer;
    
    // Start is called before the first frame update
    void Start()
    {
        Initialize();      
        emission = ps.emission;
        StartCoroutine(TestStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartEmitting()
    {
        emission.enabled = true;
    }

    void StopEmitting()
    {
        emission.enabled = false;
    }

    private void LateUpdate()
    {
        if (isKillerState)
        {
            float a = 1;
            for (int i = 0 ; i < particles.Length ; i++)
            {
                Vector3 particlePos = transform.TransformPoint(particles[i].position);
                Vector3 playerPos = MultiplayerManager.instance.players[savePlayerId[i]].transform.position;
                float dist = (playerPos - particlePos).magnitude;

                a = (Time.time - particlesDatas[i].time) * particlesDatas[i].initDist/2;
                print(a);
                Vector3 newPos = Vector3.Lerp(particlesDatas[i].initPos, losers[particlesDatas[i].IdPlayerTarget].transform.position, a );

                particles[i].position = transform.InverseTransformPoint(newPos);

                

            }
            if (a >= 1)
            {
                ps.Clear();
                particles = null;
                particlesDatas = null;
                isKillerState = false;
            } else  ps.SetParticles(particles, particles.Length);

        }
        
    }


    public void KillLosers(List<Player> players)
    {
        StopEmitting();
        losers = players.ToArray();
        particles = new ParticleSystem.Particle[ps.particleCount];
        int numParticlesAlive = ps.GetParticles(particles);
        
        for (int i = 0; i < numParticlesAlive; i++)
        {

            particles[i].startColor = Color.red;
            particles[i].velocity = Vector3.zero;

        }

        var velOverLT = ps.velocityOverLifetime;
        velOverLT.enabled = false;
        var noise = ps.noise;
        noise.enabled = false;
        var subEmit = ps.subEmitters;
        subEmit.enabled = false;
        ps.SetParticles(particles, numParticlesAlive);
        StartCoroutine(CallLaunchParticlesKiller());
    }

    public void LaunchParticlesKiller()
    {
        savePlayerId = new int[particles.Length]; // Va permettre de sauvegarder pour chaque l'id du player vise.
        particlesDatas = new ParticleDatas[particles.Length];
        for (int i = 0; i < particles.Length; i++)
        {
            int rand = Random.Range(0, losers.Length);
            savePlayerId[i] = rand;

            //Vector3 posPlayer = players[rand].transform.position;
            //Vector3 dir = (posPlayer - transform.TransformPoint(particles[i].position)).normalized;
            //dir.y = -dir.y;
            //dir = transform.TransformDirection(dir);

            //particles[i].velocity =  dir *  50;
            Vector3 initPos = transform.TransformPoint(particles[i].position);
            Vector3 posPlayer = losers[rand].transform.position;
            ParticleDatas pDatas = new ParticleDatas(ref initPos, rand, (initPos - posPlayer).magnitude, Time.time);
            particlesDatas[i] = pDatas;
            
            isKillerState = true;


        }
        ps.SetParticles(particles, particles.Length);
    }

    void Initialize()
    {
            ps = GetComponent<ParticleSystem>();
    }

    IEnumerator TestStart()
    {
        yield return new WaitForSeconds(3);
        KillLosers(MultiplayerManager.instance.players);
    }

    IEnumerator CallLaunchParticlesKiller()
    {
        yield return new WaitForSeconds(1);
        LaunchParticlesKiller();
    }
}

public struct ParticleDatas
{
    public Vector3 initPos;
    public int IdPlayerTarget;
    public float initDist;
    public float time;

    public ParticleDatas(ref Vector3 _initPos, int _idTarget, float _initDist, float _time)
    {
        initPos = _initPos;
        IdPlayerTarget = _idTarget;
        initDist = _initDist;
        time = _time;
    }
}