using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public bool isRegen = true;

    public float maxHealth = 100f;
    public float maxStamina;

    public float staminaRegenTime = 7.5f;
    public float staminaRegenTimer = 2f;
    public float healthRegenTime = 60f;
    public float healthRegenTimer = 10f;

    private float tempStaminaTimer = 0f;
    private float tempHealthTimer = 0f;

    public float curHealth;
    public float curStamina;
    public float staminaDrain = 15f; //затрата стамины в секунду 

    public float stamnaDrainCoff = 1.25f;

    public float armour = 0f;
    public float armorCoff = 0.25f;

    public float deathTime = 20f;
    

    public void HealthChange(float amount)
    {
        tempHealthTimer = healthRegenTimer;

        if (amount < 0 && Mathf.Abs(amount) > armorCoff * armour)
        {
            curHealth += amount + armorCoff * armour;
            curStamina += stamnaDrainCoff * amount;
        }
        else if (amount < 0 && Mathf.Abs(amount) <= armorCoff * armour)
        {
            curStamina += stamnaDrainCoff * amount;
        }
        else if (amount >= 0) curHealth += amount;

        if (curHealth <= 0) Death();
        else if (curHealth > maxHealth) curHealth = maxHealth;
        if (curStamina < 0) curStamina = 0f;

        maxStamina = curHealth;
    }

    private void Death()
    {
        isRegen = false;
        if(GetComponent<Player>() != null)
        {
            SceneManager.LoadScene("DeathScene");
        }

        GameManager.instance.creatures.Remove(gameObject);
        GetComponent<Inscription>()?.Reply();
        Animator animator = GetComponent<Animator>();
        if (animator)
        {
            animator.SetBool("Alive", false);
            AI ai = GetComponent<AI>();
            if (ai)
            {
                ai.Disable();
                ai.enabled = false;
            }
        }
        //Destroy(transform.gameObject, deathTime);
    }

    // float amount в Update() будет тратиться за секунду
    public void StaminaDrain(float amount, bool inUpdate)
    {
        tempStaminaTimer = staminaRegenTimer;

        if (inUpdate)
        {
            curStamina -= amount * Time.deltaTime;
        }
        else
        {
            curStamina -= amount;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        curHealth = maxHealth;
        maxStamina = maxHealth;
        curStamina = maxStamina;

        // Если старт для каждого объекта свой
        GameManager.instance.creatures.Add(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (curStamina > maxStamina) curStamina = maxStamina;
        if (isRegen)
        {
            if (curHealth < maxHealth && tempHealthTimer <= 0.1f)
            {
                curHealth += (maxHealth / healthRegenTime) * Time.deltaTime;
                if (curHealth > maxHealth) curHealth = maxHealth;
                maxStamina = curHealth;
            }
            if (curStamina < maxStamina && tempStaminaTimer <= 0.1f)
            {
                curStamina += (maxStamina / staminaRegenTime) * Time.deltaTime;
            }

            if (tempHealthTimer > 0) tempHealthTimer -= Time.deltaTime;
            if (tempStaminaTimer > 0) tempStaminaTimer -= Time.deltaTime;
        }
    }
}
