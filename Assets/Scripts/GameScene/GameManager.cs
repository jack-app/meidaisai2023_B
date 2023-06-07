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
        uiManager.UIUpdate(score, rocketControl.FuelAmount(), rocketControl.delta.magnitude);
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

    public void StartGameFinish()
    {
        StartCoroutine(GameFinish());
    }


    private IEnumerator GameFinish()
    {
        yield return new WaitForSecondsRealtime(5f);
        gameBgm.Stop();
        uiManager.Result(score);
    }

    public void RocketMoveAudio()
    {
        rocketMoveAudio.Play();
    }
}
