using UnityEngine;

public class RulesTypes : MonoBehaviour
{
    public bool isActive;
    public bool isDone;
    public int ruleID;

    public void NewRule(GameObject gameObject)
    {
        if (RulesManager.Instance != null)
        {
            RulesManager.Instance.RuleRegistration(gameObject, isActive, isDone, ruleID);
        }
    }
}
