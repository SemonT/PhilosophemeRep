using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FransisBacon : npc
{
    public override float F(float x)
    {
        return x * x;
    }
    public override void InteractionTree()
    {
        DialogueO();

        BlockO();
        Text("Приветос ребятос!");
        Text("Ух, каков молокососс!");
        string condition = Condition(RandomCondition);
        BlockC();

        BlockO();
        string option1 = Option(new string[1] { "Я нажал секретную кнопку!" });
        BlockC();

        if (option1 == "Я нажал секретную кнопку!")
        {
            BlockO();
            Text("Хочешь немного философии?");
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
                        Text("Лох!");
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
