using System;
using UnityEngine;

public class RuleSpawnerBase : MonoBehaviour
{
    [Header("Rules list")]
    [SerializeField]
    private GameObject[] rules;

    [Header("CoolDown")]
    [SerializeField]
    private float readyTime;

    public bool isActive = true;

    private bool isReady = true;

    public void SpawnARule(int ruleNumber)
    {
        if (isReady)
        {
            Instantiate(rules[ruleNumber]);
            isReady = false;
            Invoke("GetReady", readyTime);
        }
    }

    private void GetReady()
    {
        isReady = true;
    }

    public GameObject[] GetRuleList()
    {
        return rules;
    }
}
