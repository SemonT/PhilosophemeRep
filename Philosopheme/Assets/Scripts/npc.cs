using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class npc : Interactable
{
    public delegate void NPCupdate(bool leftKey, bool rightKey);
    public static NPCupdate npcUpdateContainer;

    public Sprite sprite;
    public float spriteSize = 0.15f;
    public float spriteVerticalShift = 0.6f;
    public float spriteHorizontalShift = 0f;
    public float charactersSize = 0.05f;
    public float textHorizontalIntervalMultiplier = 1f;
    public float textVerticalIntervalMultiplier = 2f;
    public float centerLocalDistance = 1f;
    public float centerVerticalShift = 0.2f;
    public float skipDistance = 0.4f;
    public float shiftSpeed = 0.7f;

    bool _active = false;
    bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            if (value)
            {
                isInteractable = false;
                letterMatrix.CreateDialogue();
            }
            else
            {
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
                        GameObject obj = new GameObject();
                        letters.Add(obj);
                        obj.transform.parent = lineO.transform;
                        float x = cursorX;
                        float y = 0;
                        float z = block.matrix.npc.F(x);
                        obj.transform.localPosition = new Vector3(x, y, z);
                        obj.transform.rotation = Quaternion.LookRotation(obj.transform.position - block.matrix.cameraTransform.position);
                        obj.AddComponent<MeshRenderer>();
                        TextMesh mesh = obj.AddComponent<TextMesh>();
                        mesh.anchor = TextAnchor.MiddleCenter;
                        mesh.characterSize = block.matrix.npc.charactersSize;
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
                blockO.parent = matrix.matrixO;
                float shift = 0;
                foreach (Block b in matrix.blocks)
                {
                    shift += b.size;
                }
                blockO.localPosition = new Vector3(0, -shift, 0);
                blockO.rotation = matrix.matrixO.rotation;

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
                    float a = Vector3.Angle(matrix.cameraTransform.forward, l.lineO.position - matrix.cameraTransform.position);
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

                foreach (GameObject o in line.letters)
                {
                    float newX = o.transform.localPosition.x + x;
                    float newY = o.transform.localPosition.y;
                    float newZ = matrix.npc.F(newX);
                    o.transform.localPosition = new Vector3(newX, newY, newZ);
                    o.transform.rotation = Quaternion.LookRotation(o.transform.position - matrix.cameraTransform.position);
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
        npc npc;
        List<Block> blocks;
        Transform matrixO;

        public Transform playerTransform;
        Transform cameraTransform;
        Transform portraitTransform;
        Transform lightTransform;
        public Vector3 center;
        Vector3 forwardDirection;
        Vector3 npcInitialPos;

        public LetterMatrix(npc npc)
        {
            this.npc = npc;
            blocks = new List<Block>();
            matrixO = new GameObject().transform;
            playerTransform = Player.instance.transform;
            cameraTransform = GameManager.instance.cam.transform;

            GameObject lightObj = new GameObject();
            lightTransform = lightObj.transform;
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Spot;
            light.spotAngle = 90;
            light.range = 15f;
            light.color = new Color(0.9f, 0.9f, 1);
            lightObj.SetActive(false);

            GameObject portraitObj = new GameObject();
            portraitTransform = portraitObj.transform;
            portraitTransform.localScale = new Vector3(npc.spriteSize, npc.spriteSize, npc.spriteSize);
            SpriteRenderer spriteRenderer = portraitObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = npc.sprite;
            portraitObj.SetActive(false);
        }
        public void CreateDialogue()
        {
            center = playerTransform.position;
            forwardDirection = (npc.transform.position - center).normalized;
            forwardDirection.y = npc.centerVerticalShift;
            forwardDirection *= npc.centerLocalDistance;

            matrixO.SetPositionAndRotation(center + forwardDirection, Quaternion.LookRotation(new Vector3(-forwardDirection.x, 0, -forwardDirection.z)));
            npcInitialPos = npc.gameObject.transform.position;
            npc.gameObject.transform.position = new Vector3(matrixO.position.x, npc.gameObject.transform.position.y, matrixO.position.z);

            portraitTransform.gameObject.SetActive(true);
            portraitTransform.position = center + forwardDirection;
            portraitTransform.position += Vector3.up * npc.spriteVerticalShift;
            portraitTransform.rotation = Quaternion.LookRotation(portraitTransform.position - cameraTransform.position);
            portraitTransform.position += Vector3.forward * npc.spriteHorizontalShift;

            lightTransform.gameObject.SetActive(true);
            lightTransform.SetPositionAndRotation(center + new Vector3(0, 1f, 0), Quaternion.LookRotation(Vector3.down));

            GameManager.instance.cam.fieldOfView = 110;
            GameManager.instance.mainLight.gameObject.SetActive(false);
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
                float a = Vector3.Angle(cameraTransform.forward, b.blockO.position - cameraTransform.position);
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


            npc.gameObject.transform.position = npcInitialPos;

            portraitTransform.gameObject.SetActive(false);
            lightTransform.gameObject.SetActive(false);

            GameManager.instance.cam.fieldOfView = GameManager.instance.camDefaultFieldOfView;
            GameManager.instance.mainLight.gameObject.SetActive(true);
        }
    }
    LetterMatrix letterMatrix;

    // Start is called before the first frame update
    void Start()
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
        if (!letterMatrix.ignore && !letterMatrix.currentBlockBuilded)
        {
            executeCounter++;
            if (executesMadeCounter < executeCounter)
            {
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

            float playerDistance = (letterMatrix.center - letterMatrix.playerTransform.position).magnitude;
            if (centerLocalDistance > playerDistance)
            {
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
    
}
