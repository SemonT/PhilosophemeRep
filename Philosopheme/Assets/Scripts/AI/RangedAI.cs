using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ИИ дальнего боя подходит к вам на дистанцию радиуса действия оружия/скилла и стреляет/кидает в вас фаерболы и прочее говно
// При движении пусть прыгает UPD: сделаю через анимации прыжок
public class RangedAI : AI
{
    public GameObject fireball;
    public Transform firepoint;
    public float accuracy = 35f;
    public float force = 45f;   

    protected float fireTime;
    protected float fireTimer;

    protected override void Start()
    {
        base.Start();
        fireTime = anime.GetCurrentAnimatorClipInfo(0).Length;
        fireTimer = fireTime;
        print("Время анимации " + fireTime);
    }

    protected override void Update()
    {
        base.Update();
        if (ranged)
        {
            if(fireTimer <= 0)
            {
                GameObject ball = Instantiate(fireball, firepoint);
                firepoint.DetachChildren();

                Player player = Player.instance;
                Rigidbody bullet = ball.GetComponent<Rigidbody>();

                /*
                float velocity = (force / bullet.mass) * Time.deltaTime;
                float time = curDist / velocity;

                Vector3 newPos = player.speed * time;
                Vector3 target = (firepoint.position - newPos);

                Debug.DrawLine(player.transform.position, player.speed, Color.cyan, 1.5f);
                Debug.DrawLine(firepoint.position, newPos, Color.blue, 1.5f);
                Debug.DrawLine(transform.position, target, Color.green, 1.5f);
                */
                Vector3 target = (player.transform.position - firepoint.position);
                bullet.AddForce(target * force);

                fireTimer = fireTime;
            }
            fireTimer -= Time.deltaTime;
        }
        else fireTimer = fireTime;

     //   print(fireTimer);
    }
}
