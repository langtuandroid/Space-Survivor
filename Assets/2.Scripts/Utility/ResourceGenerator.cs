using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    public List<ResourcePool> resourcePool = new List<ResourcePool>();

    public List<GameObject> generatedResource = new List<GameObject>();

    //[SerializeField] private GameObject blueMineral;
    //[SerializeField] private Transform blueMineralParent;


    public static ResourceGenerator instance;

    private void Awake()
    {
        instance = this;
    }

    public void EnQueueResource(resourceType type, GameObject resource)
    {
        for (int i = 0; i < resourcePool.Count; i++)
        {
            if (type == resourcePool[i].type)
            {
                resourcePool[i].EnQueue(resource);

                break;
            }
        }
    }

    public void DeQueueResource(resourceType type, Vector2 position, DropTable dropTable)
    {
        for (int i = 0; i < resourcePool.Count; i++)
        {
            if (type == resourcePool[i].type)
            {
                int dropAmount = 1;
                switch (type)
                {
                    case resourceType.Mineral1:
                        dropAmount = Random.Range(dropTable.dropExpMinAmount, dropTable.dropExpmaxAmount);
                        break;

                    case resourceType.Mineral2:
                        dropAmount = Random.Range(dropTable.dropExpMinAmount, dropTable.dropExpmaxAmount);
                        break;

                    case resourceType.Mineral3:
                        dropAmount = Random.Range(dropTable.dropExpMinAmount, dropTable.dropExpmaxAmount);
                        break;

                    case resourceType.Crystal:
                        dropAmount = Random.Range(dropTable.dropCrystalMinAmount, dropTable.dropCrystalmaxAmount);
                        break;
                }

                //StartCoroutine(force());

                //IEnumerator force()
                //{
                for (int z = 0; z < dropAmount; z++)
                {
                    var resource = resourcePool[i].DeQueue(position);

                    Vector3 randomPosition = new Vector3(Random.Range(-.3f, .3f), Random.Range(-.3f, .3f), 0);

                    resource.transform.position = resource.transform.position + randomPosition;

                    generatedResource.Add(resource);

                    //자원에 움직임 주는 경우
                    if (dropTable.maxDropForce > 0)
                    {
                        var rigid = resource.AddComponent<Rigidbody2D>();

                        if (rigid != null)
                            rigid.drag = 3;

                        if (dropTable.maxDropForce > 0 && rigid != null)
                            rigid.AddForce(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(0, dropTable.maxDropForce));

                        StartCoroutine(deleteRigid());

                        IEnumerator deleteRigid()
                        {
                            yield return new WaitForSeconds(5f);

                            if (rigid != null)
                                Destroy(rigid);
                        }
                    }
                }
                //}

                break;
            }
        }
    }

    public void DeleteAllResource()
    {
        for (int i = 0; i < generatedResource.Count; i++)
        {
            if (generatedResource[i].gameObject.activeSelf)
            {
                generatedResource[i].GetComponent<Resource>().EnQueueThisResource();
            }
        }
    }
}

[System.Serializable]
public class ResourcePool
{
    public Stack<GameObject> resourceStack = new Stack<GameObject>();

    public GameObject Object;
    public Transform parent;
    public resourceType type;

    public void EnQueue(GameObject enemy)
    {
        resourceStack.Push(enemy);
    }

    public GameObject DeQueue(Vector2 position)
    {
        if (resourceStack.Count > 0)
        {
            var resource = resourceStack.Pop();

            resource.transform.position = position;
            resource.SetActive(true);
            resource.GetComponent<Resource>().ResetResource();

            return resource;
        }
        else
        {
            var resource = GameObject.Instantiate(Object, position, Quaternion.identity, parent);
            return resource;
        }
    }
}

public enum resourceType { none, Mineral1, Mineral2, Crystal, Mineral3 }
