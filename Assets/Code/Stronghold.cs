using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stronghold : Damageable
{
    public List<GameObject> Soldiers = new List<GameObject>();
    public GameObject[] UnitTypes = new GameObject[3];
    public int Coins;
    public override void Die()
    {
        SceneManager.LoadScene(0);
    }    
    public void SpawnSoldier(GameObject Unit)
    {
        Soldiers.Add(Unit);
        Unit.GetComponent<SoldierBase>().Team = Team;
        Unit.GetComponent<SoldierBase>().HomeBase = gameObject;
        Coins -= Unit.GetComponent<SoldierBase>().Value;
    }
    public void SpawnBarbarian()
    {
        if (UnitTypes[0].GetComponent<SoldierBase>().Value >= Coins)
        {
            SpawnSoldier(Instantiate(UnitTypes[0], transform) as GameObject);
        }
    }
    public void SpawnCleric()
    {
        if (UnitTypes[0].GetComponent<SoldierBase>().Value >= Coins)
        {
            SpawnSoldier(Instantiate(UnitTypes[2], transform) as GameObject);
        }
    }
    public void SpawnRanger()
    {
        if (UnitTypes[0].GetComponent<SoldierBase>().Value >= Coins)
        {
            SpawnSoldier(Instantiate(UnitTypes[1], transform) as GameObject);
        }
    }
}
