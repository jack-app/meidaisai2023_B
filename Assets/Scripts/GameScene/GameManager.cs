using System;
using System.Collections;
using System.Collections.Generic;
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
        KeyManager.GetKeyInfo();
        rocketControl.gameObject.GetComponent<TrailRenderer>().enabled = rocketTrajectoryExist;
        AudioSource[] audioSources = rocketControl.gameObject.GetComponents<AudioSource>();
        gameBgm = audioSources[0];
        rocketMoveAudio = audioSources[1];
    }

    private void Update()
    {
        KeyManager.GetKeyInfo();
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

public static class KeyManager
{
    public static Key horizontal;
    public static Key vertical;
    public static Key space;
    public static Key c;
    public static Key f;
    public static Key p;
    public static Key r;
    public static Key i;
    public static Key s;
    public static Key lShift;
    public static Key rShift;

    public static void GetKeyInfo()
    {
        horizontal = new Key() { down = Input.GetButtonDown("Horizontal"), up = Input.GetButtonUp("Horizontal"), keep = Input.GetButton("Horizontal"), axis = Input.GetAxis("Horizontal") };
        vertical = new Key() { down = Input.GetButtonDown("Vertical"), up = Input.GetButtonUp("Vertical"), keep = Input.GetButton("Vertical"), axis = Input.GetAxis("Vertical") };
        space = new Key() { down = Input.GetButtonDown("Jump"), up = Input.GetButtonUp("Jump"), keep = Input.GetButton("Jump") };
        c = new Key() { down = Input.GetKeyDown(KeyCode.C), up = Input.GetKeyUp(KeyCode.C), keep = Input.GetKey(KeyCode.C) };
        f = new Key() { down = Input.GetKeyDown(KeyCode.F), up = Input.GetKeyUp(KeyCode.F), keep = Input.GetKey(KeyCode.F) };
        p = new Key() { down = Input.GetKeyDown(KeyCode.P), up = Input.GetKeyUp(KeyCode.P), keep = Input.GetKey(KeyCode.P) };
        r = new Key() { down = Input.GetKeyDown(KeyCode.R), up = Input.GetKeyUp(KeyCode.R), keep = Input.GetKey(KeyCode.R) };
        p = new Key() { down = Input.GetKeyDown(KeyCode.I), up = Input.GetKeyUp(KeyCode.I), keep = Input.GetKey(KeyCode.I) };
        r = new Key() { down = Input.GetKeyDown(KeyCode.S), up = Input.GetKeyUp(KeyCode.S), keep = Input.GetKey(KeyCode.S) };
        lShift = new Key() { down = Input.GetKeyDown((KeyCode.LeftShift)), up = Input.GetKeyUp((KeyCode.LeftShift)), keep = Input.GetKey((KeyCode.LeftShift)) };
        rShift = new Key() { down = Input.GetKeyDown((KeyCode.RightShift)), up = Input.GetKeyUp((KeyCode.RightShift)), keep = Input.GetKey((KeyCode.RightShift)) };
    }
}

public class Key
{
    public bool down = false;
    public bool up = false;
    public bool keep = false;
    public float axis = 0;
}