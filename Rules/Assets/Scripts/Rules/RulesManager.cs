using System.Collections.Generic;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    private static RulesManager instance;
    public static RulesManager Instance => instance;

    private Dictionary<GameObject, RuleData> activeRules = new Dictionary<GameObject, RuleData>();

    private int currentRuleID = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RuleRegistration(GameObject gameObject, bool isActive, bool isDone, int ruleID)
    {
        if (ruleID == 0)
        {
            ruleID = currentRuleID++;
        }

        if (!activeRules.ContainsKey(gameObject))
        {
            activeRules.Add(gameObject, new RuleData
            {
                gameObject = gameObject,
                isActive = isActive,
                isDone = isDone,
                ruleID = ruleID

            });
            Debug.Log($"Правило {ruleID} зарегистрировано для {gameObject.name}");
        }
    }

    public void RuleInfo(GameObject gameObject)
    {
        var rule = activeRules[gameObject];
        Debug.Log($"Правило {rule.ruleID}, {rule.isActive}, {rule.isDone}, {gameObject.name}");
    }

    public void RuleUpdate(GameObject gameObject, bool isActive, bool isDone,  int ruleID = 0)
    {
        if (activeRules.ContainsKey(gameObject))
        {
            if (isActive && isDone)
            {
                activeRules[gameObject].isDone = isDone;
                activeRules[gameObject].isActive = isActive;
                RuleDestroy(gameObject);
            }
        }
    }

    private void RuleDestroy(GameObject gameObject)
    {
        var rule = activeRules[gameObject];
        if (rule.isActive && rule.isDone)
        {
            Debug.Log($"Rule {rule.ruleID} has been deleted!");

            Destroy(gameObject);
            activeRules.Remove(gameObject);
        }
    }

    private class RuleData
    {
        public GameObject gameObject;
        public bool isActive;
        public bool isDone;
        public int ruleID;
    }
}