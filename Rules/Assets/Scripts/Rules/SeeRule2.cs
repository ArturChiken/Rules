using UnityEngine;

public class SeeRule2 : SeeRule
{
    private void Start()
    {
        FindPlayerCamera();

        NewRule(gameObject);
    }
}
