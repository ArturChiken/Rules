using UnityEngine;

public class TriggerRule : RulesTypes
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("The player entered in the trigger!");
            isDone = true;
            
            isActive = false;

            FinishRule();
        }
    }
}
