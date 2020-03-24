using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Transform centerTransform;
    public Transform planeTransform;
    public Light light1;
    public Light light2;
    public Vector3 firstItemPos;

    Vector3 planeInitialScale;
    Vector3 light1InitialPosition;
    Vector3 light2InitialPosition;
    float light1InitialRange;
    float light2InitialRange;

    List<Item> items;
    Player player;
    Animator animator;
    Vector3 playerStartPosition;
    Quaternion playerStartRotation;
    [HideInInspector] public bool isOpened;
    Item currentItem;

    float normalDelta;
    float radius;

    void Awake()
    {
        if (instance == null) instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        animator = player.gameObject.GetComponent<Animator>();
        items = new List<Item>();
        isOpened = false;

        normalDelta = firstItemPos.magnitude;
        radius = 0;

        planeTransform.gameObject.SetActive(false);
        light1.gameObject.SetActive(false);
        light2.gameObject.SetActive(false);
        planeInitialScale = planeTransform.localScale;
        light1InitialPosition = light1.gameObject.transform.localPosition;
        light2InitialPosition = light2.gameObject.transform.localPosition;
        light1InitialRange = light1.range;
        light2InitialRange = light2.range;
    }

    private void Update()
    {
        bool openInventoryKey = Input.GetKeyDown(KeyCode.Tab);
        bool releaseCurrentItemKey = Input.GetKeyDown(KeyCode.G);
        bool mouse0Key = Input.GetKeyDown(KeyCode.Mouse0);
        bool mouse1Key = Input.GetKeyDown(KeyCode.Mouse1);
        bool rKey = Input.GetKeyDown(KeyCode.R);
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
        if (mouse0Key || mouse1Key || rKey) currentItem?.Use(mouse0Key, mouse1Key, rKey);
        
        if (releaseCurrentItemKey) ReleaseCurrentItem();
    }
    public void OpenInventory()
    {
        isOpened = true;
        playerStartPosition = player.gameObject.transform.position;
        playerStartRotation = player.gameObject.transform.rotation;
        player.gameObject.transform.SetPositionAndRotation(centerTransform.position, centerTransform.rotation);

        Vector3 pos = firstItemPos;
        for (int i = 0; i < items.Count; i++)
        {
            //trimObjectsList.Add(obj);
            GameObject itemObject = items[i].gameObject;
            if (itemObject != currentItem?.gameObject)
            {
                itemObject.SetActive(true);
                itemObject.transform.parent = centerTransform;
                itemObject.transform.localPosition = Vector3.zero;
                GameManager.instance.TranslatePositionObject(itemObject.transform, pos, 0.5f);
                itemObject.transform.localRotation = Random.rotation;

                radius = pos.magnitude;

                pos += Vector3.Cross(pos, Vector3.up).normalized * normalDelta;
            }
        }

        GameManager.instance.mainLight.gameObject.SetActive(false);
        planeTransform.gameObject.SetActive(true);
        light1.gameObject.SetActive(true);
        light2.gameObject.SetActive(true);
        planeTransform.localScale = planeInitialScale * radius;
        light1.gameObject.transform.localPosition = light1InitialPosition * radius;
        light2.gameObject.transform.localPosition = light2InitialPosition * radius;
        light1.range = light1InitialRange * radius;
        light2.range = light2InitialRange * radius;
    }
    public void CloseInventory()
    {
        isOpened = false;
        player.gameObject.transform.SetPositionAndRotation(playerStartPosition, playerStartRotation);

        foreach (Item item in items)
        {
            if (item != currentItem)
            {
                item.transform.parent = null;
                item.gameObject.SetActive(false);
            }
        }

        GameManager.instance.mainLight.gameObject.SetActive(true);
        planeTransform.gameObject.SetActive(false);
        light1.gameObject.SetActive(false);
        light2.gameObject.SetActive(false);
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
        currentItem.SetUsable(animator);
        currentItem.gameObject.transform.parent = player.armTransform;
        currentItem.gameObject.transform.localPosition = -currentItem.handle.localPosition;
        currentItem.gameObject.transform.localRotation = Quaternion.LookRotation(currentItem.forwardPointer.localPosition);
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.GetComponent<Collider>().enabled = false;
    }
    void DropCurrentItem()
    {
        if (currentItem)
        {
            currentItem.SetUnusable();
            currentItem.gameObject.transform.parent = null;
            currentItem.GetComponent<Collider>().enabled = true;
            currentItem = null;
        }
    }
    void ReleaseCurrentItem()
    {
        if (currentItem)
        {
            items.Remove(currentItem);
            if (isOpened)
            {
                currentItem.gameObject.transform.parent = centerTransform;
                GameManager.instance.TranslatePositionObject(currentItem.gameObject.transform, light1.gameObject.transform.localPosition + Vector3.up * normalDelta, light1.gameObject.transform.localPosition.magnitude);
                Destroy(currentItem.gameObject, light1.gameObject.transform.localPosition.magnitude);
            }
            else
            {
                currentItem.gameObject.transform.parent = null;
                currentItem.GetComponent<Collider>().enabled = true;
                currentItem.GetComponent<Collider>().isTrigger = false;
                currentItem.GetComponent<Rigidbody>().isKinematic = false;
            }
            currentItem = null;
        }
    }
    //public void UseCurrentItem(AnimationClip anim)
    //{
    //    if (currentItem && currentItem.isUsable)
    //    {
    //        currentItem.Use(anim);
    //        if (player.animator && anim) player.animator.SetTrigger(anim.name);
    //    }
    //}

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
