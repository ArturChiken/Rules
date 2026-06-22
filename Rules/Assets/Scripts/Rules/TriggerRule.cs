using UnityEngine;

public class TriggerRule : RulesTypes
{
    protected virtual void Start()
    {
        NewRule(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в триггер!");
            isDone = true;

            RulesManager.Instance.RuleInfo(gameObject);
            RulesManager.Instance.RuleUpdate(gameObject, isActive, isDone);
        }
    }
}
