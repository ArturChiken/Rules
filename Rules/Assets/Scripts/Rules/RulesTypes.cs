using UnityEngine;

public class RulesTypes : MonoBehaviour
{
    public int ruleCost;

    public bool isActive = false;
    public bool isDone = false;

    private RulesManager rulesManager;

    private void Start()
    {
        rulesManager = GameObject.Find("Managers").GetComponent<RulesManager>();
        rulesManager.RegisterNewRule(ruleCost, isActive, isDone);
    }

    public void FinishRule()
    {
        if (isDone)
        {
            Debug.Log($"The rule had: active is {isActive}, done is {isDone}");
            rulesManager.EndRule(gameObject);
        }
    }

    public void FindRulesManager()
    {
        rulesManager = GameObject.Find("Managers").GetComponent<RulesManager>();
    }

    public RulesManager GetRulesManager()
    {
        return rulesManager;
    }
}
