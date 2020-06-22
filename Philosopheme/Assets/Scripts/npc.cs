using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class npc : Interactable
{
    public delegate void NPCupdate(bool leftKey, bool rightKey);
    public static NPCupdate npcUpdateContainer;
    public static bool isDialogueOpened = false;

    public Transform postamentTransform;
    public Transform portraitTransform;
    public Transform lightTransform;
    public Transform oTransform;
    public Transform orientationTransform;
    public GameObject letterPrefab;
    public GameObject room;
    public float charactersSize = 0.05f;
    public float textHorizontalIntervalMultiplier = 1f;
    public float textVerticalIntervalMultiplier = 2f;
    public float textReachTime = 0.2f;
    public float textDelayDelta = 0.03f;
    public float skipDistance = 0.4f;
    public float shiftSpeed = 0.7f;
    public float exitRadius = 2f;

    Light spotLight;
    Transform cameraTransform;
    bool _active = false;
    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            if (value)
            {
                isDialogueOpened = true;
                isInteractable = false;
                letterMatrix.CreateDialogue();
            }
            else
            {
                isDialogueOpened = false;
                isInteractable = true;
                letterMatrix.DestroyDialogue();
            }
            _active = value;
        }
    }
    int executeCounter;
    int executesMadeCounter = 0;
    public delegate void ExecutionContainer();
    public delegate string ContitionContainer();
    ContitionContainer currentConditionContainer;
    List<string> conditionsMade = new List<string>();
    int conditionCounter;
    int conditionsMadeCounter = 0;
    class LetterMatrix
    {
        public bool ignore = true;
        public int blockCounter;
        public int currentBlockIndex = 1;
        public Block currentBlock = null;
        public bool currentBlockBuilded = false;
        public bool currentBlockDone = false;
        public bool specialCondition = true;
        public List<string> optionChoises = new List<string>();
        public int optionCounter;
        public int choisesMadeCounter = 0;

        public class Block
        {
            public bool done = false;

            public class Line
            {
                public bool isOption = false;

                public Block block;

                public GameManager.PositionTranslationObject lastLetterTransit;
                public List<GameObject> letters;
                public Transform lineO;
                public float size;
                public float cursorX;

                public string stringLetters;

                public Line(Block block)
                {
                    this.block = block;

                    lineO = new GameObject().transform;
                    lineO.parent = block.blockO;
                    float shift = 0;
                    foreach (Line l in block.lines)
                    {
                        shift += l.size;
                    }
                    lineO.localPosition = new Vector3(0, -shift, 0);
                    lineO.rotation = block.blockO.rotation;

                    letters = new List<GameObject>();

                    size = block.matrix.npc.charactersSize * block.matrix.npc.textVerticalIntervalMultiplier;
                    cursorX = 0;
                }
                public void AddLetters(string text, Color color)
                {
                    stringLetters += text;
                    for (int i = 0; i < text.Length; i++)
                    {
                        float x = cursorX;
                        float y = 0;
                        float z = block.matrix.npc.F(x);
                        Vector3 targetPos = new Vector3(x, y, z);
                        GameObject obj = Instantiate(
                            block.matrix.npc.letterPrefab,
                            block.matrix.npc.gameObject.transform.position,
                            Quaternion.LookRotation(block.matrix.npc.orientationTransform.localPosition - targetPos),
                            lineO.transform);
                        letters.Add(obj);
                        GameManager.PositionTranslationObject o = new GameManager.PositionTranslationObject(obj.transform, targetPos, block.matrix.npc.textReachTime, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault, i * block.matrix.npc.textDelayDelta);
                        GameManager.instance.TranslatePositionObject(ref o);
                        if (i == text.Length - 1) lastLetterTransit = o;
                        TextMesh mesh = obj.GetComponent<TextMesh>();
                        mesh.text = text[i].ToString();
                        mesh.color = color;

                        cursorX -= block.matrix.npc.charactersSize * block.matrix.npc.textHorizontalIntervalMultiplier;
                    }

                }
                public void DestroyLetters()
                {
                    foreach (GameObject o in letters)
                    {
                        Destroy(o);
                    }
                    letters = null;
                }
            }

            public LetterMatrix matrix;

            public List<Line> lines;
            public Transform blockO;
            public float size;

            public Block(LetterMatrix matrix)
            {
                this.matrix = matrix;

                blockO = new GameObject().transform;
                blockO.parent = matrix.npc.oTransform;
                float shift = 0;
                foreach (Block b in matrix.blocks)
                {
                    shift += b.size;
                }
                blockO.localPosition = new Vector3(0, -shift, 0);
                blockO.rotation = matrix.npc.oTransform.rotation;

                lines = new List<Line>();

                size = 0;
            }
            public void DestroyLineAt(int index)
            {
                for (int i = index; i < lines.Count; i++)
                {
                    lines[i].lineO.position = new Vector3(lines[i].lineO.position.x, lines[i].lineO.position.y + lines[index].size, lines[i].lineO.position.z);
                }
                lines[index].DestroyLetters();
                Destroy(lines[index].lineO.gameObject);
                lines.RemoveAt(index);
            }
            public void DestroyLines()
            {
                foreach (Line l in lines)
                {
                    l.DestroyLetters();
                    Destroy(l.lineO.gameObject);
                }
            }
            public Line AddLine()
            {
                Line line = new Line(this);
                lines.Add(line);
                size += line.size;

                return line;
            }
            public Line FindNearestLine()
            {
                Line line = null;
                float minAngle = 180;
                foreach (Line l in lines)
                {
                    float a = Vector3.Angle(matrix.npc.cameraTransform.forward, l.lineO.position - matrix.npc.cameraTransform.position);
                    if (a < minAngle)
                    {
                        minAngle = a;
                        line = l;
                    }
                }
                return line;
            }
            public void Shift(float x)
            {
                Line line = FindNearestLine();
                if (line == null) return;

                if (line.lastLetterTransit != null && line.lastLetterTransit.isReached)
                {
                    foreach (GameObject letter in line.letters)
                    {
                        float newX = letter.transform.localPosition.x + x;
                        float newY = letter.transform.localPosition.y;
                        float newZ = matrix.npc.F(newX);
                        letter.transform.localPosition = new Vector3(newX, newY, newZ);
                        letter.transform.localRotation = Quaternion.LookRotation(letter.transform.localPosition - matrix.npc.orientationTransform.localPosition);
                    }
                    Transform tf = line.letters[0].transform;
                    Transform tl = line.letters[line.letters.Count - 1].transform;
                    if (tf.localPosition.x < -matrix.npc.skipDistance || tl.localPosition.x > matrix.npc.skipDistance)
                    {
                        int lineIndex;
                        if (line.isOption)
                        {
                            matrix.optionChoises.Add(line.stringLetters);
                            matrix.choisesMadeCounter++;
                            lineIndex = lines.Count - 1;
                        }
                        else
                        {
                            lineIndex = lines.IndexOf(line);
                        }
                        for (int i = 0; i <= lineIndex; i++) DestroyLineAt(0);
                        if (lines.Count == 0)
                        {
                            matrix.currentBlockDone = true;
                            matrix.DestroyBlockAt(0);
                        }
                    }
                }
            }
        }
        npc npc;
        List<Block> blocks;

        public Transform playerTransform;
        public Vector3 center;

        public LetterMatrix(npc npc)
        {
            this.npc = npc;
            blocks = new List<Block>();
            playerTransform = Player.instance.transform;
            npc.cameraTransform = GameManager.instance.cam.transform;
            npc.spotLight = npc.lightTransform.gameObject.GetComponent<Light>();
            npc.charactersSize = npc.letterPrefab.GetComponent<TextMesh>().characterSize * npc.letterPrefab.transform.localScale.x;

            npc.lightTransform.gameObject.SetActive(false);

            center = npc.portraitTransform.position;
            npc.portraitTransform.gameObject.SetActive(false);
        }
        public void CreateDialogue()
        {
            npc.portraitTransform.gameObject.SetActive(true);

            npc.lightTransform.gameObject.SetActive(true);

            GameManager.instance.cam.fieldOfView = 110;
            GameManager.TurnOffMainLights();
            Lamp.TurnOffAllLamps();
            npc.room.SetActive(true);
        }
        public Block AddBlock()
        {
            Block block = new Block(this);
            blocks.Add(block);
            return block;
        }
        public Block FindNearestBlock()
        {
            Block block = null;
            float minAngle = 180;
            foreach (Block b in blocks)
            {
                float a = Vector3.Angle(npc.cameraTransform.forward, b.blockO.position - npc.cameraTransform.position);
                if (a < minAngle)
                {
                    minAngle = a;
                    block = b;
                }
            }
            return block;
        }
        public void DestroyBlockAt(int index)
        {
            for (int i = index; i < blocks.Count; i++)
            {
                blocks[i].blockO.position = new Vector3(blocks[i].blockO.position.x, blocks[i].blockO.position.y + blocks[index].size, blocks[i].blockO.position.z);
            }
            blocks[index].DestroyLines();
            Destroy(blocks[index].blockO.gameObject);
            blocks.RemoveAt(index);
        }
        public void DestroyBlocks()
        {
            foreach (Block b in blocks)
            {
                b.DestroyLines();
                Destroy(b.blockO.gameObject);
            }
            blocks = new List<Block>();
        }
        public void DestroyDialogue()
        {
            DestroyBlocks();
            currentBlock = null;
            currentBlockBuilded = false;

            npc.portraitTransform.gameObject.SetActive(false);
            npc.lightTransform.gameObject.SetActive(false);

            GameManager.instance.cam.fieldOfView = GameManager.instance.camDefaultFieldOfView;
            GameManager.TurnOnMainLights();
            Lamp.TurnOnAllLamps();
            npc.room.SetActive(false);
        }
    }
    LetterMatrix letterMatrix;

    // Start is called before the first frame update
    public void Start()
    {
        letterMatrix = new LetterMatrix(this);
        npcUpdateContainer += UpdateWP;
    }
    public override void Interact()
    {
        Active = true;
    }
    public void DialogueO()
    {
        letterMatrix.ignore = true;
        letterMatrix.blockCounter = 0;
        letterMatrix.optionCounter = 0;
        executeCounter = 0;
        conditionCounter = 0;
    }
    public void DialogueC()
    {
        
    }
    public void BlockO()
    {
        letterMatrix.blockCounter++;
        if (letterMatrix.currentBlockIndex == letterMatrix.blockCounter)
        {
            letterMatrix.ignore = false;
            if (letterMatrix.currentBlock == null)
            {
                letterMatrix.currentBlockBuilded = false;
                letterMatrix.currentBlock = letterMatrix.AddBlock();
            }
        }
    }
    public void BlockC()
    {
        if (!letterMatrix.ignore)
        {
            if (letterMatrix.currentBlockDone && letterMatrix.specialCondition)
            {
                if (letterMatrix.currentBlock != null) letterMatrix.currentBlockIndex++;
                letterMatrix.currentBlock = null;
                letterMatrix.currentBlockBuilded = false;
                letterMatrix.currentBlockDone = false;
            }
            letterMatrix.currentBlockBuilded = true;
            letterMatrix.ignore = true;
        }
    }
    public void Text(string text)
    {
        if (!letterMatrix.ignore && !letterMatrix.currentBlockBuilded)
        {
            LetterMatrix.Block.Line line = letterMatrix.currentBlock.AddLine();
            line.AddLetters(text, Color.white);
        }
    }
    public string Option(string[] options)
    {
        letterMatrix.optionCounter++;
        if (!letterMatrix.ignore && !letterMatrix.currentBlockBuilded)
        {
            for (int i = 0; i < options.Length; i++)
            {
                LetterMatrix.Block.Line line = letterMatrix.currentBlock.AddLine();
                line.AddLetters(options[i], Color.cyan);
                line.isOption = true;
            }
        }
        if (letterMatrix.choisesMadeCounter < letterMatrix.optionCounter) return "";
        else return letterMatrix.optionChoises[letterMatrix.optionCounter - 1];
    }
    public void Execution(ExecutionContainer e)
    {
        if (!letterMatrix.ignore)
        {
            executeCounter++;
            if (/*executesMadeCounter < executeCounter*/true)
            {
                letterMatrix.currentBlockDone = true;
                e?.Invoke();
                executesMadeCounter++;
            }
        }
    }
    public string Condition(ContitionContainer c)
    {
        conditionCounter++;
        if (!letterMatrix.ignore)
        {
            if (!letterMatrix.currentBlockBuilded)
            {
                if (conditionsMadeCounter >= conditionCounter)
                {
                    letterMatrix.currentBlock = null;
                    letterMatrix.specialCondition = true;
                    letterMatrix.currentBlockIndex++;
                    letterMatrix.DestroyBlockAt(0);
                }
                else
                {
                    letterMatrix.specialCondition = false;
                    currentConditionContainer = c;
                }
            }
        }
        if (conditionsMadeCounter < conditionCounter) return "";
        else return conditionsMade[conditionCounter - 1];
    }
    void CheckCurrentCondition()
    {
        if (currentConditionContainer != null)
        {
            string r = currentConditionContainer.Invoke();
            if (r != "")
            {
                conditionsMadeCounter++;
                conditionsMade.Add(r);
                currentConditionContainer = null;
            }
        }
    }
    public abstract void InteractionTree();
    public abstract float F(float x);
    void UpdateWP(bool leftKey, bool rightKey)
    {
        CheckCurrentCondition();
        if (Active)
        {
            InteractionTree();

            if (spotLight.spotAngle / 2 > Vector3.Angle((Player.instance.gameObject.transform.position - lightTransform.position), lightTransform.forward))
            {
                portraitTransform.rotation = Quaternion.LookRotation(portraitTransform.position - cameraTransform.position);
                if (leftKey)
                {
                    letterMatrix.currentBlock?.Shift(Time.deltaTime * shiftSpeed);
                }
                if (rightKey)
                {
                    letterMatrix.currentBlock?.Shift(Time.deltaTime * -shiftSpeed);
                }
            }
            else
            {
                Active = false;
            }
        }
    }
    public void Teleport(Transform destination)
    {
        postamentTransform.position = destination.position;
        postamentTransform.rotation = destination.rotation;
    }
}
