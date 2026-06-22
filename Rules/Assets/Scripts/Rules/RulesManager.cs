using System.Collections.Generic;
using UnityEngine;

public class RulesManager : MonoBehaviour
{
    [SerializeField]
    private int maxRulesBudget;
    private int minRulesBudget = 0;
    public int rulesBudget;

    public void RegisterNewRule(int ruleCost, bool isActive, bool isDone)
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

    public void EndRule(GameObject gameObject)
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
}