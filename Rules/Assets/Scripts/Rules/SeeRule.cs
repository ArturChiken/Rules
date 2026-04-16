using UnityEngine;

public class SeeRule : RulesTypes
{
    [Header("Настройки луча")]
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected bool showDebugRay = true;

    [Header("Насртойка правила")]
    [SerializeField] protected float lookDuration = 1f;
    [SerializeField] protected float lookDistance = 10f;
    protected float lookTimer = 0f;

    protected Camera playerCamera;

    protected virtual void Start()
    {
        NewRule(gameObject);

        FindPlayerCamera();
    }

    protected virtual void Update()
    {
        if (IsPlayerLookingAtObject())
        {
            OnPlayerLookStart();
        }
        else
        {
            OnPlayerLookStop();
        }
    }

    protected virtual void FindPlayerCamera()
    {
        // Ищем камеру по тегу
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCamera = player.GetComponentInChildren<Camera>();
            if (playerCamera == null)
                playerCamera = Camera.main;
        }

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    protected virtual bool IsPlayerLookingAtObject()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (showDebugRay)
        {
            Debug.DrawRay(ray.origin, ray.direction * lookDistance, Color.red);
        }

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, lookDistance, layerMask))
        {
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                return true;
            }
        }

        return false;
    }

    protected virtual void OnPlayerLookStart()
    {
        Debug.Log("Player is looking at the Object");

        lookTimer += Time.deltaTime;
        Debug.Log($"{lookTimer}");
        if (lookTimer >= lookDuration && !isDone)
        {
            Debug.Log("The rule is done");
            isDone = true;

            RulesManager.Instance.RuleUpdate(gameObject, isActive, isDone, ruleID);
        }
    }

    protected virtual void OnPlayerLookStop()
    {
        if (lookTimer > 0)
        {
            lookTimer = 0;
        }

        Debug.Log("Player isn't looking at the Object");
    }
}
