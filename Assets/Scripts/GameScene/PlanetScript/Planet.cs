using System;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public float mass;
    public float orbitLevel1;
    private System.Random random = new System.Random();
    public long orbitRadius { get; private set; } // destroy�Ŏg��

    // �C���X�^���X�������Ƃ��ɌĂяo�����
    public int Initialize(int orbitRadius)
    {
        this.orbitRadius = orbitRadius;
        int randomDeg = random.Next(360);
        this.transform.position = new Vector3(orbitRadius * (float)Math.Cos(randomDeg * Math.PI / 180), 0, this.orbitRadius * (float)Math.Sin(randomDeg * Math.PI / 180));
        return 60;
    }

    // update����Ăяo�����
    // �ʒu��ς���
    public void Move()
    {
        float rotateAngle = 1f / (10 * (orbitRadius / 1000f + 1f));
        this.transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, rotateAngle);
    }

    public void Destrroy()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
}