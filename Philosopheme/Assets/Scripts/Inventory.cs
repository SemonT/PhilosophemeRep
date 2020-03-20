using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Vector3 firstItemPos;

    [HideInInspector] public bool isOpened;
    List<Item> items;
    Item currentItem;
    AnimationClip[] currentAnims;
    Player player;

    List<GameObject> trimObjectsList;
    Transform centerTransform;
    float normalDelta;
    float radius;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        items = new List<Item>();
        isOpened = false;
        player = Player.instance;

        centerTransform = new GameObject().transform;
        normalDelta = firstItemPos.magnitude;
        radius = 0;

        trimObjectsList = new List<GameObject>();
    }

    private void Update()
    {
        bool openInventoryKey = Input.GetKeyDown(KeyCode.Tab);
        bool useCurrentItemKey1 = Input.GetKeyDown(KeyCode.Mouse0);
        bool useCurrentItemKey2 = Input.GetKeyDown(KeyCode.Mouse1);
        bool releaseCurrentItemKey = Input.GetKeyDown(KeyCode.R);
        if (isOpened)
        {
            Vector3 delta = centerTransform.position - player.gameObject.transform.position;
            if (delta.magnitude > radius || openInventoryKey)
            {
                CloseInventory();
            }
            foreach (Item item in items)
            {
                if (item != currentItem) item.gameObject.transform.RotateAround(item.transform.position, Vector3.one, Time.deltaTime * 30);
            }
        }
        else
        {
            if (openInventoryKey)
            {
                OpenInventory();
            }
        }
        if (currentAnims != null)
        {
            if (useCurrentItemKey1 && currentAnims.Length > 0) UseCurrentItem(currentAnims[0]);
            if (useCurrentItemKey2 && currentAnims.Length > 1) UseCurrentItem(currentAnims[1]);
        }
        if (releaseCurrentItemKey) ReleaseCurrentItem();
    }
    public void OpenInventory()
    {
        isOpened = true;

        centerTransform.SetPositionAndRotation(player.gameObject.transform.position, player.gameObject.transform.rotation);

        //GameManager.instance.mainLight.gameObject.SetActive(false);

        Vector3 pos = firstItemPos;
        for (int i = 0; i < items.Count; i++)
        {
            //trimObjectsList.Add(obj);
            GameObject itemObject = items[i].gameObject;
            if (itemObject != currentItem?.gameObject)
            {
                itemObject.SetActive(true);
                itemObject.transform.parent = centerTransform;
                itemObject.transform.localPosition = pos;
                itemObject.transform.localRotation = Random.rotation;

                radius = pos.magnitude;

                pos += Vector3.Cross(pos, Vector3.up).normalized * normalDelta;
            }
        }
    }
    public void CloseInventory()
    {
        isOpened = false;

        foreach (Item item in items)
        {
            if (item != currentItem)
            {
                item.transform.parent = null;
                item.gameObject.SetActive(false);
            }
        }
        //GameManager.instance.mainLight.gameObject.SetActive(true);

        foreach (GameObject o in trimObjectsList) Destroy(o);
        trimObjectsList.Clear();
    }
    public void PickupItem(Item item)
    {
        items.Add(item);
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Collider>().isTrigger = true;
        item.gameObject.SetActive(false);
    }
    public void TakeItem(Item item)
    {
        DropCurrentItem();
        currentItem = item;
        currentItem.gameObject.transform.parent = player.armTransform;
        currentItem.gameObject.transform.localPosition = -currentItem.handle.localPosition;
        currentItem.gameObject.transform.localRotation = Quaternion.LookRotation(currentItem.forwardPointer.localPosition);
        currentItem.GetComponent<Collider>().enabled = false;

        currentAnims = new AnimationClip[currentItem.actions.Length];
        int actionI = 0;
        foreach (Item.Action a in currentItem.actions)
        {
            foreach (AnimationClip clip in player.clips)
            {
                if (clip.name == a.animationName)
                {
                    currentAnims[actionI] = clip;
                }
            }
            actionI++;
        }
    }
    void DropCurrentItem()
    {
        if (currentItem)
        {
            currentItem.gameObject.transform.parent = null;
            currentItem.GetComponent<Collider>().enabled = true;
            currentAnims = null;
            currentItem = null;
        }
    }
    void ReleaseCurrentItem()
    {
        if (currentItem)
        {
            items.Remove(currentItem);
            currentItem.GetComponent<Rigidbody>().isKinematic = false;
            currentItem.GetComponent<Collider>().isTrigger = false;
            DropCurrentItem();
        }
    }
    public void UseCurrentItem(AnimationClip anim)
    {
        if (currentItem && currentItem.isUsable && !isOpened)
        {
            currentItem.Use(anim);
            if (player.animator && anim) player.animator.SetTrigger(anim.name);
        }
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
