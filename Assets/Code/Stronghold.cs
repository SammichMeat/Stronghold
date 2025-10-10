using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Stronghold : MonoBehaviour
{
    public int Health;
    public int MaxHealth;
    public string Team;
    public List<GameObject> Soldiers = new List<GameObject>();
    public int[] UnitWeights = new int[3];
    public GameObject[] UnitTypes = new GameObject[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnSoldier()
    {

    }
}
