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

    // �C���X�^���X�������Ƃ��ɌĂяo�����
    public Planet(int orbitRadius, int radius)
    {
    }

    // update����Ăяo�����
    // �ʒu��ς���
    void PlanetMove()
    {
    }
}
