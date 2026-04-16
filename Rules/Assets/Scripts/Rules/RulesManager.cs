using UnityEngine;

public class RulesManager : MonoBehaviour
{
    public void RuleData(bool isActive, bool isDone, GameObject gameObject, int number = 0)
    {
        Debug.Log("Сюда пришло");
        Debug.Log($"Правило {number}, {isActive}, {isDone}");
    }

    public void RuleUpdate(bool isActive, bool isDone, GameObject gameObject, int number = 0)
    {
        if (isActive && isDone)
            RuleDestroy(gameObject, number);
    }

    public void RuleDestroy(GameObject gameObject, int number)
    {
        Destroy(gameObject);
        Debug.Log($"Rule {number} has been deleted!");
    }
}