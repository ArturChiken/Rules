using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    [Header("Budget settings")]
    [SerializeField]
    private int maxRulesBudget;
    private int minRulesBudget = 0;
    private int rulesBudget;

    [Header("SpawnSettings")]
    [SerializeField]
    private float timeBeforeSpawn;
    [SerializeField]
    private float spawnCoolDown;
    [SerializeField]
    private GameObject[] spawners;

    private bool stopSpawning = false;

    private void Start()
    {
        rulesBudget = maxRulesBudget;

        InvokeRepeating("BudgetManagement", timeBeforeSpawn, spawnCoolDown);
    }

    public void RegisterNewRule(int ruleCost)
    {
        if (rulesBudget -  ruleCost >= minRulesBudget)
        {
            rulesBudget -= ruleCost;
            Debug.Log("New rule has registered");
            Debug.Log($"It costs {ruleCost}");
            Debug.Log($"We have {rulesBudget} budget left");
        }
        else
        {
            Debug.Log("New rule can't be registered");
            Debug.Log("Lack of budget");
            Debug.Log($"Our budget is {rulesBudget}, the rule cost is {ruleCost}");
        }
    }

    public void SuccessRule(GameObject gameObject)
    {
        //RulesTypes rule = gameObject.GetComponent<RulesTypes>();
        int ruleCost = gameObject.GetComponent<RulesTypes>().ruleCost;

        if (rulesBudget + ruleCost <= maxRulesBudget)
        {
            rulesBudget += ruleCost;
            Debug.Log("Case 1");
        }
        else if (rulesBudget + ruleCost > maxRulesBudget)
        {
            rulesBudget = maxRulesBudget;
            Debug.Log("Case 2");
        }

        Debug.Log("Rule has been deleted");
        Debug.Log($"Our budget is {rulesBudget} now, the rule gave us {ruleCost}");
        Destroy(gameObject);
    }

    public void FailedRule(GameObject gameObject)
    {
        int ruleCost = gameObject.GetComponent<RulesTypes>().ruleCost;

        if (rulesBudget + ruleCost <= maxRulesBudget)
        {
            rulesBudget += ruleCost;
            Debug.Log("Case 1");
        }
        else if (rulesBudget + ruleCost > maxRulesBudget)
        {
            rulesBudget = maxRulesBudget;
            Debug.Log("Case 2");
        }

        Debug.Log("Rule has been deleted");
        Debug.Log($"Our budget is {rulesBudget} now, the rule gave us {ruleCost}");
        Destroy(gameObject);
    }

    private void BudgetManagement()
    {
        //spawners[UnityEngine.Random.Range(0, spawners.Length - 1)].GetComponent<RuleSpawnerBase>().DoYourWork();
        RuleSpawnerBase spawner;
        int randomInt = UnityEngine.Random.Range(0, spawners.Length - 1);
        spawner = spawners[randomInt].GetComponent<RuleSpawnerBase>();

        randomInt = UnityEngine.Random.Range(0, spawner.GetRuleList().Length - 1);
        if (spawner.GetRuleList()[randomInt].GetComponent<RulesTypes>().ruleCost >= rulesBudget)
        {
            Debug.Log("We can spawn a rule");
            Debug.Log($"Our budget is {rulesBudget}");

            if (stopSpawning)
            {
                stopSpawning = false;
                InvokeRepeating("BudgetManagement", timeBeforeSpawn, spawnCoolDown);
            }

            TryToSpawnRule(spawner, randomInt);
        }
        else
        {
            Debug.Log("Can't spawn a rule!");
            Debug.Log($"Lack of the budget ({rulesBudget})");

            CancelInvoke("BudgetManagement");

            stopSpawning = true;
            BudgetManagement();
        }
    }

    private void TryToSpawnRule(RuleSpawnerBase spawner, int ruleNumber)
    {
        spawner.SpawnARule(ruleNumber);
    }
}