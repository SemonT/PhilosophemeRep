using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FransisBacon : npc
{
    public override float F(float x)
    {
        return x * x / 2;
    }
    public override void InteractionTree()
    {
        DialogueO();

        BlockO();
        Text("Здравствуй, человек, говорящий с портретом!");
        Text("Я умён, а ты не очень");
        BlockC();

        BlockO();
        Text("Поэтому я буду ждать, пока ты нажмёшь на секретную кнопку (M)");
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
                        Execution(RandomFunc);
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

    void RandomFunc()
    {
        print("RandomFunc works!");
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
