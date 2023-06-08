using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static bool rocketTrajectoryExist = true;
    public static bool planetTrajectoryExist = true;
    public static bool visualGravityExist = true;
    public static bool visualOrbitExist = true;

    [SerializeField] private RocketControl rocketControl = null!;
    [SerializeField] private PlanetManager planetManager = null!;
    [SerializeField] private SubCamera subCamera = null!;
    [SerializeField] private UIManager uiManager = null!;
    private long score;
    private AudioSource gameBgm;
    private AudioSource rocketMoveAudio;

    void Start()
    {
        Application.targetFrameRate = 60;
        rocketControl.gameObject.GetComponent<TrailRenderer>().enabled = rocketTrajectoryExist;
        AudioSource[] audioSources = rocketControl.gameObject.GetComponents<AudioSource>();
        gameBgm = audioSources[0];
        rocketMoveAudio = audioSources[1];
    }

    private void Update()
    {
        score = score > (long)rocketControl.nowPosition.magnitude ? score : (long)rocketControl.nowPosition.magnitude;
        uiManager.UIUpdate(score, rocketControl.FuelAmount(), rocketControl.delta.magnitude, rocketControl.chargePower);
    }

    void FixedUpdate()
    {
        float rocketDistance = rocketControl.nowPosition.magnitude;
        bool _inOrbit = rocketControl.inOrbit;
        if (rocketControl.crash) return;
        if(!_inOrbit && !rocketControl.InGravity()) planetManager.PlanetDestroy(rocketDistance);
        planetManager.PlanetSpawn(rocketDistance);
        planetManager.PlanetMove();
        rocketControl.RocketUpdate();
        subCamera.CameraUpdate();
    }

    public void StartGameFinish(int CauseOfDeath, float Time, float MaxSpeed, int PlanetCount, int SpCount)
    {
        StartCoroutine(GameFinish(CauseOfDeath, Time, MaxSpeed, PlanetCount, SpCount));
    }


    private IEnumerator GameFinish(int CauseOfDeath, float Time, float MaxSpeed, int PlanetCount, int SpCount)
    {
        gameBgm.Stop();
        yield return new WaitForSecondsRealtime(4f);
        uiManager.Result(score, CauseOfDeath, Time, MaxSpeed, PlanetCount, SpCount);
    }

    public void RocketMoveAudio()
    {
        rocketMoveAudio.Play();
    }
}
