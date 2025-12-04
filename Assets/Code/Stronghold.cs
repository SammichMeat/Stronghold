using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Stronghold : Damageable
{
    [Header("Soldier Management")]
    public List<GameObject> Soldiers = new List<GameObject>();
    public GameObject[] UnitTypes = new GameObject[3]; 
    // 0 = Barbarian, 1 = Ranger, 2 = Cleric

    [Header("Economy")]
    public int Coins = 0;
    public int CoinsPerSecond = 25;   // <--- ADDED: prevents coins from never increasing

    [Header("Enemy")]
    public Stronghold EnemyStronghold;

    [Header("AI Weights")]
    private float W_barbarian = 0.5f;
    private float W_ranger = 0.3f;
    private float W_cleric = 0.2f;

    private float spawnTimer = 0f;
    private float nextSpawnTime = 0f;
    public Transform SpawnPoint;

    private bool initialized = false;

    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        ValidateUnitTypes();
    }

    private void Start()
    {
        ResetSpawnTimer();
        initialized = true;
    }

    private void ValidateUnitTypes()
    {
        for (int i = 0; i < UnitTypes.Length; i++)
        {
            if (UnitTypes[i] == null)
            {
                Debug.LogError($"❌ Stronghold '{name}' UnitTypes[{i}] is NOT assigned in Inspector.");
            }
            else if (UnitTypes[i].GetComponent<SoldierBase>() == null)
            {
                Debug.LogError($"❌ UnitTypes[{i}] on '{name}' does NOT contain a SoldierBase component.");
            }
        }
    }

    // =========================================================
    // UPDATE LOOP
    // =========================================================

    private void Update()
    {
        if (!initialized) 
            return; // prevents Update running before Start()

        // Passive income
        Coins += Mathf.RoundToInt(CoinsPerSecond * Time.deltaTime);

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

    // =========================================================
    // SPAWNING
    // =========================================================

    public void SpawnSoldier(GameObject unit)
    {
        // Set soldier position to the spawn point
        unit.transform.position = SpawnPoint.position;
        unit.transform.rotation = SpawnPoint.rotation;

        // Parent soldier under the spawn point object
        unit.transform.SetParent(SpawnPoint);

        Soldiers.Add(unit);

        var soldier = unit.GetComponentInChildren<SoldierBase>();
        soldier.Team = Team;
        soldier.HomeBase = gameObject;

        Coins -= soldier.Value;
    }

    public void SpawnBarbarian()
    {
        var cost = UnitTypes[0].GetComponent<SoldierBase>().Value;
        if (Coins >= cost)
        {
            var unit = Instantiate(UnitTypes[0]);
            SpawnSoldier(unit);
        }
    }

    public void SpawnRanger()
    {
        var cost = UnitTypes[1].GetComponent<SoldierBase>().Value;
        if (Coins >= cost)
        {
            var unit = Instantiate(UnitTypes[1]);
            SpawnSoldier(unit);
        }
    }

    public void SpawnCleric()
    {
        var cost = UnitTypes[2].GetComponent<SoldierBase>().Value;
        if (Coins >= cost)
        {
            var unit = Instantiate(UnitTypes[2]);
            SpawnSoldier(unit);
        }
    }

    private bool CanAfford(int index)
    {
        return Coins >= UnitTypes[index].GetComponent<SoldierBase>().Value;
    }

    // =========================================================
    // AI SYSTEM
    // =========================================================

    private void AI_SpawnLogic()
    {
        int cheapest = GetCheapestUnitCost();
        if (Coins < cheapest)
            return;

        AdjustWeightsBasedOnGameState();
        NormalizeWeights();

        float roll = Random.value;

        if (roll < W_barbarian)
            SpawnBarbarian();
        else if (roll < W_barbarian + W_ranger)
            SpawnRanger();
        else
            SpawnCleric();
    }

    private int GetCheapestUnitCost()
    {
        // Validate again just in case (prevents crashes)
        for (int i = 0; i < UnitTypes.Length; i++)
        {
            if (UnitTypes[i] == null)
            {
                Debug.LogError($"❌ Stronghold UnitTypes[{i}] is NULL at runtime!");
                return int.MaxValue;
            }
        }

        int costB = UnitTypes[0].GetComponent<SoldierBase>().Value;
        int costR = UnitTypes[1].GetComponent<SoldierBase>().Value;
        int costC = UnitTypes[2].GetComponent<SoldierBase>().Value;

        return Mathf.Min(costB, costR, costC);
    }

    private void AdjustWeightsBasedOnGameState()
    {
        float selfHP = Health / MaxHealth;
        float enemyHP = EnemyStronghold != null 
                        ? EnemyStronghold.Health / EnemyStronghold.MaxHealth 
                        : 1f;

        // Low health → more offense 
        if (selfHP < 0.30f)
        {
            W_barbarian += 0.1f;
            W_cleric = Mathf.Max(0.05f, W_cleric - 0.1f);
        }

        // Enemy nearly dead → more ranger damage
        if (enemyHP < 0.40f)
        {
            W_ranger += 0.1f;
            W_barbarian = Mathf.Max(0.05f, W_barbarian - 0.05f);
            W_cleric = Mathf.Max(0.05f, W_cleric - 0.05f);
        }
    }

    private void NormalizeWeights()
    {
        float total = W_barbarian + W_ranger + W_cleric;

        if (total <= 0)
        {
            // Emergency reset (should never happen)
            W_barbarian = 0.5f;
            W_ranger = 0.3f;
            W_cleric = 0.2f;
            total = 1f;
        }

        W_barbarian /= total;
        W_ranger    /= total;
        W_cleric    /= total;
    }
}
