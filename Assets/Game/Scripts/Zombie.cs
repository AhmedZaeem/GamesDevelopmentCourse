using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float speed = 1.6f;
    public float turnSpeed = 6f;
    public float attackRange = 1.7f;
    public float attackRate = 2.2f;
    public int damage = 8;
    public int health = 100;

    CharacterController controller;
    Animator anim;
    PlayerController player;
    float nextAttack;
    bool dead;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        player = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (dead || player == null)
            return;

        Vector3 toPlayer = player.transform.position - transform.position;
        toPlayer.y = 0f;
        float distance = toPlayer.magnitude;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayer), turnSpeed * Time.deltaTime);

        if (distance > attackRange)
        {
            controller.Move(toPlayer.normalized * speed * Time.deltaTime);
            anim.SetBool("Walking", true);
        }
        else
        {
            anim.SetBool("Walking", false);

            if (Time.time >= nextAttack)
            {
                nextAttack = Time.time + attackRate;
                anim.SetTrigger("Attack");
                Invoke(nameof(Hit), 0.5f);
            }
        }

        controller.Move(Vector3.up * -9.81f * Time.deltaTime);
    }

    void Hit()
    {
        if (dead || player == null)
            return;

        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange + 0.6f)
            player.TakeDamage(damage);
    }

    public void TakeDamage(int amount)
    {
        if (dead)
            return;

        health -= amount;

        if (health <= 0)
        {
            dead = true;
            anim.SetTrigger("Die");
            controller.enabled = false;
            GameManager.instance.AddKill();
            Destroy(gameObject, 5f);
        }
    }
}
