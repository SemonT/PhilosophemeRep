using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public delegate bool ItemFilterCheck(Item i);
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
    static public bool isOpened;
    static public bool isDeployed;
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
        bool hotkey1 = Input.GetKeyDown(KeyCode.Alpha1);
        bool hotkey2 = Input.GetKeyDown(KeyCode.Alpha2);
        bool hotkey3 = Input.GetKeyDown(KeyCode.Alpha3);

        bool openInventoryKey = Input.GetKeyDown(KeyCode.Tab);
        bool releaseCurrentItemKey = Input.GetKeyDown(KeyCode.G);

        bool mouse0Key = Input.GetKey(KeyCode.Mouse0);
        bool mouse1Key = Input.GetKey(KeyCode.Mouse1);
        bool rKey = Input.GetKey(KeyCode.R);

        bool mouse0KeyDown = Input.GetKeyDown(KeyCode.Mouse0);
        bool mouse1KeyDown = Input.GetKeyDown(KeyCode.Mouse1);
        bool rKeyDown = Input.GetKeyDown(KeyCode.R);

        bool mouse0KeyUp = Input.GetKeyUp(KeyCode.Mouse0);
        bool mouse1KeyUp = Input.GetKeyUp(KeyCode.Mouse1);
        bool rKeyUp = Input.GetKeyUp(KeyCode.R);
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
            Item it = null;
            if (hotkey1)
            {
                bool ItemFilter(Item i)
                {
                    if (i is MeleWeapon item)
                        return true;
                    return false;
                }
                it = FindItem(ItemFilter);
            }
            else if (hotkey2)
            {
                bool ItemFilter(Item i)
                {
                    if (i is RangedWeapon item)
                        return true;
                    return false;
                }
                it = FindItem(ItemFilter);
            }
            else if (hotkey3)
            {
                bool ItemFilter(Item i)
                {
                    if (i is Throwable item)
                        return true;
                    return false;
                }
                it = FindItem(ItemFilter);
            }
            if (it != null)
            {
                HideCurrentItem();
                TakeItem(it);
            }
            if (openInventoryKey)
            {
                OpenInventory();
            }
        }
        currentItem?.Use(mouse0Key, mouse1Key, rKey, mouse0KeyDown, mouse1KeyDown, rKeyDown, mouse0KeyUp, mouse1KeyUp, rKeyUp);
        
        if (releaseCurrentItemKey) ReleaseCurrentItem();
    }
    public void OpenInventory()
    {
        isDeployed = false;
        isOpened = true;
        playerStartPosition = player.gameObject.transform.position;
        playerStartRotation = player.gameObject.transform.rotation;
        player.gameObject.transform.SetPositionAndRotation(centerTransform.position, centerTransform.rotation);

        void OnInventoryDeploy(GameObject o)
        {
            if (!isDeployed) isDeployed = true;
        }
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
                GameManager.instance.TranslatePositionObject(itemObject.transform, pos, 0.5f, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault, 0, OnInventoryDeploy);
                itemObject.transform.localRotation = Random.rotation;

                radius = pos.magnitude;

                pos += Vector3.Cross(pos, Vector3.up).normalized * normalDelta;
            }
        }

        planeTransform.gameObject.SetActive(true);
        light1.gameObject.SetActive(true);
        light2.gameObject.SetActive(true);
        planeTransform.localScale = planeInitialScale * radius;
        light1.gameObject.transform.localPosition = light1InitialPosition * radius;
        light2.gameObject.transform.localPosition = light2InitialPosition * radius;
        light1.range = light1InitialRange * radius;
        light2.range = light2InitialRange * radius;

        GameManager.TurnOffMainLights();
        Lamp.TurnOffAllLamps();
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

        planeTransform.gameObject.SetActive(false);
        light1.gameObject.SetActive(false);
        light2.gameObject.SetActive(false);

        GameManager.TurnOnMainLights();
        Lamp.TurnOnAllLamps();
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
        currentItem.gameObject.transform.localPosition = -currentItem.handleBasis.localPosition;
        //currentItem.gameObject.transform.localRotation = currentItem.handleBasis.localRotation;
        currentItem.gameObject.transform.localRotation = Quaternion.identity * new Quaternion(currentItem.handleBasis.localRotation.x, currentItem.handleBasis.localRotation.y, currentItem.handleBasis.localRotation.z, -currentItem.handleBasis.localRotation.w);
        //currentItem.gameObject.transform.localRotation *= currentItem.handleBasis.localRotation;
        currentItem.GetComponent<Rigidbody>().isKinematic = true;
        currentItem.GetComponent<Collider>().enabled = false;
        currentItem.gameObject.SetActive(true);
        Inscription insr = currentItem.GetComponent<Inscription>();
        if (insr && insr.IsActive)
        {
            insr.Hide();
        }
    }
    public void HideCurrentItem()
    {
        Item item = currentItem;
        if (item)
        {
            ReleaseCurrentItem();
            PickupItem(item);
        }
    }
    public Item FindItem(ItemFilterCheck f)
    {
        foreach (Item item in items)
            if (f(item))
            {
                return item;
            }
        return null;
    }
    public Item PullItem(ItemFilterCheck f)
    {
        foreach (Item item in items)
            if (f(item))
            {
                RemoveItem(item);
                return item;
            }
        return null;
    }
    void RemoveItem(Item i)
    {
        items.Remove(i);
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
    void OnReach(GameObject o)
    {
        Destroy(o);
    }
    public void ReleaseCurrentItem()
    {
        if (currentItem)
        {
            items.Remove(currentItem);
            if (isOpened)
            {
                currentItem.gameObject.transform.parent = centerTransform;
                GameManager.instance.TranslatePositionObject(currentItem.gameObject.transform, light1.gameObject.transform.localPosition + Vector3.up * normalDelta, light1.gameObject.transform.localPosition.magnitude, GameManager.PositionTranslationObject.maxSpeedDefault, GameManager.PositionTranslationObject.errorDefault, 0, OnReach);
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
    
    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
