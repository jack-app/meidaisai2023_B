using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RocketControl rocketControl = null!;
    [SerializeField] private PlanetManager planetManager = null!;
    [SerializeField] private SubCamera subCamera = null!;
    [SerializeField] private UIManager uiManager = null!;

    void Start()
    {
        Application.targetFrameRate = 60;
        planetManager.FirstSpawn();
    }

    private void Update()
    {
        uiManager.UIUpdate((long)rocketControl.nowPosition.magnitude, rocketControl.FuelAmount());
    }

    void FixedUpdate()
    {
        float rocketDistance = rocketControl.nowPosition.magnitude;
        bool _inOrbit = rocketControl.inOrbit;
        if (rocketControl.crash) return;
        if(!_inOrbit && rocketControl.InGravity()) planetManager.PlanetDestroy(rocketDistance);
        planetManager.PlanetSpawn(rocketDistance);
        planetManager.PlanetMove();
        rocketControl.RocketUpdate();
        subCamera.CameraUpdate();
    }

    public void StartGameFinish(long score)
    {
        StartCoroutine(GameFinish(score));
    }


    private IEnumerator GameFinish(long score)
    {
        long finalScore = score;
        yield return new WaitForSecondsRealtime(5f);
        uiManager.Result(finalScore);
    }
}
