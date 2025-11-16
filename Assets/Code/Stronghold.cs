using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stronghold : Damageable
{
    public List<GameObject> Soldiers = new List<GameObject>();
    public GameObject[] UnitTypes = new GameObject[3]; 
    // 0 = Barbarian, 1 = Ranger, 2 = Cleric

    public int Coins;

    public float spawnTimer = 0f;
    private float nextSpawnTime = 0f;

    private float W_barbarian = 0.5f;
    private float W_ranger = 0.3f;
    private float W_cleric = 0.2f;

    public Stronghold EnemyStronghold;

    void Start()
    {
        ResetSpawnTimer();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= nextSpawnTime)
        {
            AI_SpawnLogic();
            ResetSpawnTimer();
        }
    }

    private void ResetSpawnTimer()
    {
        spawnTimer = 0f;
        nextSpawnTime = Random.Range(10f, 20f);
    }

    public override void Die()
    {
        SceneManager.LoadScene(0);
    }

    public void SpawnSoldier(GameObject unit)
    {
        Soldiers.Add(unit);
        var soldier = unit.GetComponent<SoldierBase>();

        soldier.Team = Team;
        soldier.HomeBase = gameObject;

        Coins -= soldier.Value;
    }

    public void SpawnBarbarian()
    {
        if (Coins >= UnitTypes[0].GetComponent<SoldierBase>().Value)
        {
            SpawnSoldier(Instantiate(UnitTypes[0], transform));
        }
    }

    public void SpawnRanger()
    {
        if (Coins >= UnitTypes[1].GetComponent<SoldierBase>().Value)
        {
            SpawnSoldier(Instantiate(UnitTypes[1], transform));
        }
    }

    public void SpawnCleric()
    {
        if (Coins >= UnitTypes[2].GetComponent<SoldierBase>().Value)
        {
            SpawnSoldier(Instantiate(UnitTypes[2], transform));
        }
    }
    //AI logic, not sure if it works yet
    private void AI_SpawnLogic()
    {
        // Not enough coins for anything
        int cheapestUnit = GetCheapestUnitCost();
        if (Coins < cheapestUnit)
        {
            return;
        }

        AdjustWeightsBasedOnGameState();
        NormalizeWeights();

        float roll = Random.value; // 0–1

        if (roll < W_barbarian)
        {
            SpawnBarbarian();
        }
        else if (roll < W_barbarian + W_ranger)
        {
            SpawnRanger();
        }
        else
        {
            SpawnCleric();
        }
    }

    private int GetCheapestUnitCost()
    {
        int b = UnitTypes[0].GetComponent<SoldierBase>().Value;
        int r = UnitTypes[1].GetComponent<SoldierBase>().Value;
        int c = UnitTypes[2].GetComponent<SoldierBase>().Value;
        return Mathf.Min(b, r, c);
    }

    private void AdjustWeightsBasedOnGameState()
    {
        float selfHP = Health / MaxHealth;
        float enemyHP = EnemyStronghold != null 
            ? EnemyStronghold.Health / EnemyStronghold.MaxHealth
            : 1f;

        // Low health → focus Barbarian (fight back)
        if (selfHP < 0.30f)
        {
            W_barbarian += 0.1f;
            W_cleric -= 0.1f;
        }

        // Enemy base nearly dead → Rangers do clean-up
        if (enemyHP < 0.40f)
        {
            W_ranger += 0.1f;
            W_barbarian -= 0.05f;
            W_cleric -= 0.05f;
        }
    }

    private void NormalizeWeights()
    {
        float total = W_barbarian + W_ranger + W_cleric;
        W_barbarian /= total;
        W_ranger    /= total;
        W_cleric    /= total;
    }
}
