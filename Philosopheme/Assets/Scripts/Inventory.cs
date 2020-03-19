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
        bool useCurrentItemKey = Input.GetKeyDown(KeyCode.Mouse0);
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
                if (item != currentItem) item.gameObject.transform.RotateAround(item.gameObject.transform.position, Vector3.one, Time.deltaTime * 30);
            }
        }
        else
        {
            if (openInventoryKey)
            {
                OpenInventory();
            }
        }
        if (useCurrentItemKey) UseCurrentItem();
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
        currentItem.gameObject.transform.localPosition = Vector3.zero;
        currentItem.gameObject.transform.rotation = player.armTransform.rotation;
        currentItem.GetComponent<Collider>().enabled = false;
    }
    void DropCurrentItem()
    {
        if (currentItem)
        {
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
            currentItem.GetComponent<Rigidbody>().isKinematic = false;
            currentItem.GetComponent<Collider>().isTrigger = false;
            DropCurrentItem();
        }
    }
    public void UseCurrentItem()
    {
        if (!isOpened) currentItem?.Use();
    }

    void OnDestroy()
    {
        if (instance == this) instance = null;
    }
}
