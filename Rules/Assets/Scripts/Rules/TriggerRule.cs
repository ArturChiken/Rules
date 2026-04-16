using UnityEngine;

public class TriggerRule :  RulesTypes
{

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Игрок вошел в триггер!");
            isDone = true;

            rulesManager.RuleData(isActive, isDone, gameObject);
            rulesManager.RuleUpdate(isActive, isDone, gameObject);
        }
    }
}
