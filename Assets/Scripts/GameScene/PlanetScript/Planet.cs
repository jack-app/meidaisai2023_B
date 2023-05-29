using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass;
    public float orbitLevel1;
    private Vector3 prePosition;
    private Vector3 nowPosition;
    public Vector3 plaDelta;

    // インスタンス化したときに呼び出される
    public Planet(int orbitRadius, int radius)
    {
    }

    // updateから呼び出される
    // 位置を変える
    void PlanetMove()
    {
    }
}
