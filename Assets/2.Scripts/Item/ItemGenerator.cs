using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    [SerializeField] private PlayerWeapon playerWeapon;

    [Space]
    public List<ItemPool> itemPool = new List<ItemPool>();
    public List<GameObject> generateditem = new List<GameObject>();

    public static ItemGenerator instance;

    private void Awake()
    {
        instance = this;
    }

    public void GenerateRandomItem(Vector2 position)
    {
        DeQueueItem(itemPool[Random.Range(0, itemPool.Count)].type, position);
    }

    public void EnQueueItem(ItemType type, GameObject resource)
    {
        for (int i = 0; i < itemPool.Count; i++)
        {
            if (type == itemPool[i].type)
            {
                itemPool[i].EnQueue(resource);

                break;
            }
        }
    }

    public GameObject DeQueueItem(ItemType type, Vector2 position)
    {
        for (int i = 0; i < itemPool.Count; i++)
        {
            if (type == itemPool[i].type)
            {
                var success = itemPool[i].DeQueue(position);

                if (success != null)
                {
                    Vector2 randomPosition = new Vector2(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f));

                    var item = Instantiate(success, position + randomPosition, Quaternion.identity, itemPool[i].parent);

                    generateditem.Add(item);

                    return item;
                }
            }
        }

        return null;
    }

    public void DeleteAllItem()
    {
        for (int i = 0; i < generateditem.Count; i++)
        {
            if (generateditem[i].gameObject.activeSelf)
            {
                generateditem[i].GetComponent<Item>().EnQueueThisItem();
            }
        }
    }
}

[System.Serializable]
public class ItemPool
{
    public Stack<GameObject> itemStack = new Stack<GameObject>();

    public GameObject Object;
    public Transform parent;
    public ItemType type;

    public void EnQueue(GameObject item)
    {
        itemStack.Push(item);
    }

    public GameObject DeQueue(Vector2 position)
    {
        if (itemStack.Count > 0)
        {
            var item = itemStack.Pop();

            item.transform.position = position;
            item.SetActive(true);

            return null;
        }
        else
        {
            return Object;
        }
    }
}
