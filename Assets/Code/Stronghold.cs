using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
public class Stronghold : Damageable
{
    [Header("Control Settings")]
    public bool PlayerControlled = false;

    [Header("Soldier Management")]
    public List<GameObject> Soldiers = new List<GameObject>();
    public GameObject[] UnitTypes = new GameObject[3]; 

    [Header("Economy")]
    public int Coins = 0;
    public int CoinsPerSecond = 25;

    [Header("Enemy")]
    public Stronghold EnemyStronghold;

    [Header("AI Weights (Genetic Learning)")]
    public float W_barbarian = 0.5f;
    public float W_ranger    = 0.3f;
    public float W_cleric    = 0.2f;

    private float spawnTimer = 0f;
    private float nextSpawnTime = 0f;
    public Transform SpawnPoint;

    private bool initialized = false;

    // =========================================================
    // INITIALIZATION
    // =========================================================

    private void Awake()
    {
        LoadWeightsFromMemory();
        ValidateUnitTypes();
    }

    private void Start()
    {
        ResetSpawnTimer();
        initialized = true;
    }

    private void LoadWeightsFromMemory()
    {
        if (PlayerPrefs.HasKey("W_barbarian"))
        {
            W_barbarian = PlayerPrefs.GetFloat("W_barbarian");
            W_ranger    = PlayerPrefs.GetFloat("W_ranger");
            W_cleric    = PlayerPrefs.GetFloat("W_cleric");
        }
    }

    private void SaveWeightsToMemory()
    {
        PlayerPrefs.SetFloat("W_barbarian", W_barbarian);
        PlayerPrefs.SetFloat("W_ranger", W_ranger);
        PlayerPrefs.SetFloat("W_cleric", W_cleric);
        PlayerPrefs.Save();
    }

    private void ValidateUnitTypes()
    {
        for (int i = 0; i < UnitTypes.Length; i++)
        {
            if (UnitTypes[i] == null)
            {
                Debug.LogError($"Stronghold '{name}' UnitTypes[{i}] is NOT assigned!");
            }
            else if (UnitTypes[i].GetComponent<SoldierBase>() == null)
            {
                Debug.LogError($"UnitTypes[{i}] on '{name}' does NOT contain SoldierBase.");
            }
        }
    }

    // =========================================================
    // UPDATE LOOP
    // =========================================================

    private void Update()
    {
        if (!initialized)
            return;

        Coins += Mathf.RoundToInt(CoinsPerSecond * Time.deltaTime);

        if (PlayerControlled)
            return; // Skip AI logic completely

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

    // =========================================================
    // PLAYER-CONTROLLED INPUT
    // =========================================================

    public void PlayerSpawnBarbarian() => SpawnBarbarian();
    public void PlayerSpawnRanger()    => SpawnRanger();
    public void PlayerSpawnCleric()    => SpawnCleric();

    // =========================================================
    // SPAWNING
    // =========================================================

    public void SpawnSoldier(GameObject unit)
    {
        unit.transform.position = SpawnPoint.position;
        unit.transform.rotation = SpawnPoint.rotation;
        unit.transform.SetParent(null);

        Soldiers.Add(unit);

        var soldier = unit.GetComponentInChildren<SoldierBase>();
        soldier.Team = Team;
        soldier.HomeBase = gameObject;

        Coins -= soldier.Value;

        // start performance tracking
        StartCoroutine(EvaluateSpawnPerformance(soldier));
    }

    private IEnumerator EvaluateSpawnPerformance(SoldierBase soldier)
    {
        float startSelf = Health;
        float startEnemy = EnemyStronghold.Health;

        yield return new WaitForSeconds(12f);

        float selfChange = Health - startSelf;
        float enemyChange = startEnemy - EnemyStronghold.Health;

        if (soldier is Barbarian)
        {
            if (enemyChange > 0) W_barbarian += 0.03f;
            if (selfChange < 0) W_barbarian -= 0.02f;
        }
        else if (soldier is Ranger)
        {
            if (enemyChange > 0) W_ranger += 0.03f;
            if (selfChange < 0) W_ranger -= 0.02f;
        }
        else if (soldier is Claric)
        {
            if (selfChange > 0) W_cleric += 0.03f;
            if (enemyChange <= 0) W_cleric -= 0.02f;
        }

        NormalizeWeights();
        SaveWeightsToMemory();
    }

    private bool CanAfford(int index)
    {
        return Coins >= UnitTypes[index].GetComponent<SoldierBase>().Value;
    }

    public void SpawnBarbarian()
    {
        if (CanAfford(0))
            SpawnSoldier(Instantiate(UnitTypes[0]));
    }
    public void SpawnRanger()
    {
        if (CanAfford(1))
            SpawnSoldier(Instantiate(UnitTypes[1]));
    }
    public void SpawnCleric()
    {
        if (CanAfford(2))
            SpawnSoldier(Instantiate(UnitTypes[2]));
    }

    // =========================================================
    // AI SYSTEM
    // =========================================================

    private void AI_SpawnLogic()
    {
        int cheapest = GetCheapestUnitCost();
        if (Coins < cheapest)
            return;

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
        int costB = UnitTypes[0].GetComponent<SoldierBase>().Value;
        int costR = UnitTypes[1].GetComponent<SoldierBase>().Value;
        int costC = UnitTypes[2].GetComponent<SoldierBase>().Value;

        return Mathf.Min(costB, costR, costC);
    }

    private void NormalizeWeights()
    {
        float total = W_barbarian + W_ranger + W_cleric;

        if (total == 0)
        {
            W_barbarian = 0.5f;
            W_ranger = 0.3f;
            W_cleric = 0.2f;
            total = 1;
        }

        W_barbarian /= total;
        W_ranger /= total;
        W_cleric /= total;
    }

    public override void Die()
    {
        SaveWeightsToMemory();
        SceneManager.LoadScene(0);
    }
}

