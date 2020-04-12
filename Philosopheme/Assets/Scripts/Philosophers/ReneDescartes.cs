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

        //BlockO();
        //Text("Здравствуй, странствующий человек!");
        //Text("Если ты попал сюда, то у тебя определённо возникли вопросы.");
        //Text("Здесь ты сможешь развеять их, встретившись с ними лицом к лицу.");
        //BlockC();
        //BlockO();
        //Text("Ты сразу поймёшь, что здесь являют собой вопросы. Они будут тебя старательно докучать.");
        //Text("Однако, обычные люди попадая сюда, не получая никакой помощи обычно оказываются быстро поверженными и сдаются.");
        //Text("Поэтому я дам тебе в помощь базовый инструмент - сомнение как критерий истинности.");
        //BlockC();

        BlockO();
        Execution(GiveClub);
        Execution(GiveM9);
        BlockC();

        BlockO();
        Text("Запомни, что истиным является то, в чём нельзя сомневаться!");
        Text("Теперь дерзай, но будь осторожен. Я жду тебя с правильным вопросом.");
        string condition = Condition(RandomCondition);
        BlockC();

        BlockO();
        string option1 = Option(new string[1] { "Я нажал секретную кнопку!" });
        BlockC();

        if (option1 == "Я нажал секретную кнопку!")
        {
            BlockO();
            Text("Молодец! Хочешь немного философии?");
            string option2 = Option(new string[2] { "Да", "Нет" });
            BlockC();
            switch (option2)
            {
                case "Да":
                    {
                        BlockO();
                        Text("Красава!");
                        BlockC();
                        break;
                    }
                case "Нет":
                    {
                        BlockO();
                        Text("Казуальщика ответ!");
                        BlockC();
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
    string RandomCondition()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            return "M";
        }
        return "";
    }
}
