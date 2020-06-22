using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReneDescartes : npc
{
    public static ReneDescartes instance { get; private set; }

    public GameObject club;
    public GameObject cucumber1;
    public GameObject cucumber2;
    public GameObject[] manyVegetables;

    public bool trigger1 = false;
    public bool trigger2 = false;
    public bool trigger3 = false;

    public override float F(float x)
    {
        return Mathf.Pow(x, 2f) / 4f;
    }
    public override void InteractionTree()
    {
        DialogueO();

        BlockO();
        Text("Здравствуй, странствующий человек!");
        Text("Если ты попал сюда, то у тебя определённо возникли некоторые экзистенциальные вопросы.");
        Text("Здесь ты можешь найти все ответы, встретившись с ними лицом к лицу.");
        BlockC();

        BlockO();
        Text("Ты сразу поймёшь, что здесь представляют собой вопросы. Они будут тебя старательно докучать и пытаться сломить.");
        Text("Для того, чтобы с ними справиться, тебе нужен подходящий инструмент, оружие.");
        Text("Поэтому я дам тебе в помощь базовую вещь, которая является первым шагом к познанию - сомнение.");
        BlockC();

        BlockO();
        Execution(GiveClub);
        BlockC();

        BlockO();
        Text("Это твой критерий истинности. Применяй его ко всему, что вызывает или являет вопросы.");
        Text("Запомни, что истиным является то, в чём нельзя сомневаться!");
        Text("Теперь дерзай, но будь внимателен!");
        BlockC();

        if (trigger1)
        {
            BlockO();
            Text("Я вижу, что ты разбираешься с рядовыми вопросами, однако так и не приблизился к сути.");
            Text("Попробуй взгянуть на свои действия сверху. Есть вещь, которую ты не подвергал сомнению.");
            Text("Я тебе дам два предмета. Воспользуйся ими правильно.");
            BlockC();

            BlockO();
            Execution(GiveCucumbers);
            BlockC();

            if (trigger2)
            {
                BlockO();
                string option1 = Option(new string[1] { "Я попытался усомниться в том, что могу сомневаться и... произошло нечто странное... Почему так произошло?" });
                BlockC();

                if (option1 == "Я попытался усомниться в том, что могу сомневаться и... произошло нечто странное... Почему так произошло?")
                {
                    BlockO();
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
                                Text("Ты прошёл рубикон. Теперь тебе пора искать позитивные суждения.");
                                BlockC();

                                BlockO();
                                Execution(GiveManyVegetables);
                                Execution(TeleportToEnd);
                                BlockC();

                                BlockO();
                                Text("Будь ПРОДУКТИВНЫМ!!");
                                BlockC();

                                if (trigger3)
                                {
                                    BlockO();
                                    string option3 = Option(new string[1] { "Бог есть!" });
                                    BlockC();

                                    if (option3 == "Бог есть!")
                                    {
                                        BlockO();
                                        Text("Ты нашёл то, что искал! Теперь ты можешь идти...");
                                        BlockC();
                                    }
                                }
                                break;
                            }
                        default: break;
                    }
                }
            }
        }

        DialogueC();
    }

    private void Awake()
    {
        if (!instance) instance = this;
    }
    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }
    void GiveClub()
    {
        club.SetActive(true);
        club.transform.SetParent(null, true);
    }
    void GiveCucumbers()
    {
        cucumber1.SetActive(true);
        cucumber1.transform.SetParent(null, true);
        cucumber2.SetActive(true);
        cucumber2.transform.SetParent(null, true);
    }
    void GiveManyVegetables()
    {
        foreach (GameObject go in manyVegetables)
        {
            go.SetActive(true);
            go.transform.SetParent(null, true);
        }
    }
    void TeleportToEnd()
    {
        
    }
    //    string KillCondition()
    //    {
    //        if (Input.GetKeyDown(KeyCode.N))
    //        {
    //            return "N";
    //        }
    //        return "";
    //    }
}
