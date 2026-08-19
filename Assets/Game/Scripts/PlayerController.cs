using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2.5f;
    public float runSpeed = 5.5f;
    public float turnSpeed = 12f;
    public int maxHealth = 100;
    public float attackRange = 2f;
    public int attackDamage = 50;
    public float attackRate = 0.9f;
    public float jumpForce = 5f;

    public int health;

    CharacterController controller;
    Animator anim;
    Transform cam;
    float nextAttack;
    float fallSpeed;
    bool dead;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        cam = Camera.main.transform;
        health = maxHealth;
    }

    void Update()
    {
        if (dead)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cam.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = cam.right;
        right.y = 0f;
        right.Normalize();

        Vector3 dir = forward * v + right * h;
        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        bool running = Input.GetKey(KeyCode.LeftShift);
        bool attacking = Time.time < nextAttack - attackRate + 0.6f;

        if (dir.sqrMagnitude > 0.01f && !attacking)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
            controller.Move(dir * (running ? runSpeed : walkSpeed) * Time.deltaTime);
            anim.SetFloat("Speed", running ? 1f : 0.5f, 0.1f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        }

        if (controller.isGrounded)
        {
            if (fallSpeed < 0f)
                fallSpeed = -2f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                fallSpeed = jumpForce;
                anim.SetTrigger("Jump");
            }
        }
        else
        {
            fallSpeed += Physics.gravity.y * Time.deltaTime;
        }

        controller.Move(Vector3.up * fallSpeed * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttack)
        {
            nextAttack = Time.time + attackRate;
            anim.SetTrigger("Attack");
            Invoke(nameof(Hit), 0.35f);
        }
    }

    void Hit()
    {
        if (dead)
            return;

        Collider[] targets = Physics.OverlapSphere(transform.position + transform.forward + Vector3.up, attackRange);
        foreach (Collider c in targets)
        {
            Zombie z = c.GetComponent<Zombie>();
            if (z != null)
                z.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (dead)
            return;

        health -= amount;

        if (health <= 0)
        {
            health = 0;
            dead = true;
            anim.SetTrigger("Die");
            GameManager.instance.GameOver();
        }
    }
}
