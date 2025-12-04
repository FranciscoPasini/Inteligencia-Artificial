using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Attack Settings")]
    public float attackRadius = 4f;
    public float attackCooldown = 1.5f;
    public LayerMask npcLayer;

    private float cooldownTimer = 0f;

    // FSM
    private PlayerState currentState;

    // Attack effect (LineRenderer)
    private LineRenderer lr;
    private float effectTime = 0.25f;
    private float effectTimer = 0f;

    void Start()
    {
        // LineRenderer config
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.positionCount = 40;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.enabled = false;

        ChangeState(new PlayerIdleState(this));
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        // Effect update
        if (lr.enabled)
        {
            effectTimer -= Time.deltaTime;
            DrawCircle(Mathf.Lerp(attackRadius, 0, effectTimer / effectTime));

            if (effectTimer <= 0)
                lr.enabled = false;
        }

        // FSM update
        currentState.Update();

        // Attack input
        if (Input.GetKeyDown(KeyCode.Space) && cooldownTimer <= 0)
        {
            cooldownTimer = attackCooldown;
            ChangeState(new PlayerAttackState(this));
        }
    }

    public void ChangeState(PlayerState state)
    {
        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    // ============================
    // ATTACK LOGIC
    // ============================
    public void PerformRadialAttack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, npcLayer);

        foreach (Collider hit in hits)
        {
            NPCTree npc = hit.GetComponent<NPCTree>();
            if (npc != null)
                npc.Die();
        }
    }

    // ============================
    // VISUAL EFFECT
    // ============================
    public void PlayAttackEffect()
    {
        lr.enabled = true;
        effectTimer = effectTime;
        DrawCircle(attackRadius);
    }

    private void DrawCircle(float radius)
    {
        int segments = lr.positionCount;
        float angle = 0f;

        for (int i = 0; i < segments; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            lr.SetPosition(i, new Vector3(
                transform.position.x + x,
                transform.position.y + 0.2f,
                transform.position.z + z
            ));

            angle += 360f / segments;
        }
    }
}
