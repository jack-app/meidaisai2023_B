using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Planet planetPrefab = null!;
    private int minSpawnableRange = 300;

    void Start()
    {
        while (minSpawnableRange <= 1000)
        {
            Planet planet = Instantiate(planetPrefab);
        }
    }

    void Update()
    {
        
    }
}
