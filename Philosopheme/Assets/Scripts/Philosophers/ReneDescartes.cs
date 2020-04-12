using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReneDescartes : npc
{
    public GameObject club;
    public GameObject m9;

    public override float F(float x)
    {
        return Mathf.Pow(x, 2f) / 4f;
    }
    public override void InteractionTree()
    {
        DialogueO();

        BlockO();
        Text("Здравствуй, странствующий человек!");
        Text("Если ты попал сюда, то у тебя определённо возникли вопросы.");
        Text("Здесь ты сможешь развеять их, встретившись с ними лицом к лицу.");
        BlockC();

        BlockO();
        Text("Ты сразу поймёшь, что здесь являют собой вопросы. Они будут тебя старательно докучать.");
        Text("Однако, обычные люди попадая сюда, не получая никакой помощи обычно оказываются быстро поверженными и сдаются.");
        Text("Поэтому я дам тебе в помощь базовый инструмент - сомнение как критерий истинности.");
        BlockC();

        BlockO();
        Execution(GiveClub);
        BlockC();

        BlockO();
        Text("Запомни, что истиным является то, в чём нельзя сомневаться!");
        Text("Теперь дерзай, но будь осторожен. Я жду тебя с правильным вопросом.");
        string condition = Condition(DeathCondition);
        BlockC();

        BlockO();
        string option1 = Option(new string[1] { "Я попытался усомниться в том, что могу сомневаться и... произошло нечто странное... Почему так проихошло?" });
        BlockC();

        if (option1 == "Я попытался усомниться в том, что могу сомневаться и... произошло нечто странное... Почему так проихошло?")
        {
            BlockO();
            Text("Ты наткнулся на парадокс, друг мой.");
            Text("Подумай сам: можешь ли ты сомневаться в том, что ты можешь сомневаться, если для этого нужен сам факт сомнения?");
            
            string option2 = Option(new string[2] { "Да", "Нет" });
            BlockC();
            switch (option2)
            {
                case "Да":
                    {
                        BlockO();
                        Text("ШУЕ!");
                        BlockC();
                        break;
                    }
                case "Нет":
                    {
                        BlockO();
                        Text("Совершенно верно! В противном случае получается противоречие.");
                        Text("А если утверждение вызывает противоречие, то обратное утверждение берётся за истину - запомни!");
                        Text("Ты прошёл рубикон.");
                        BlockC();

                        BlockO();
                        Text("Хоть, я не Аристотель, но я дарую тебе мощный инструмент - логику.");
                        BlockC();

                        BlockO();
                        Execution(GiveM9);
                        BlockC();

                        BlockO();
                        Text("Тебе пора уходить от принципа сомнения и переходить к поискам всех вытекающих.");
                        Text("Я жду тебя с ответом на вопрос о существовании сущности более высокого порядка.");
                        string condition2 = Condition(KillCondition);
                        BlockC();

                        BlockO();
                        string option3 = Option(new string[1] { "Бог есть!" });
                        BlockC();

                        if (option3 == "Бог есть!")
                        {
                            BlockO();
                            Text("Ты нашёл то, что искал! Теперь ты можешь идти...");
                            BlockC();
                        }

                        break;
                    }
                default: break;
            }
        }

        DialogueC();
    }

    void GiveClub()
    {
        club.SetActive(true);
    }
    void GiveM9()
    {
        m9.SetActive(true);
    }
    string DeathCondition()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            return "M";
        }
        return "";
    }
    string KillCondition()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            return "M";
        }
        return "";
    }
}
