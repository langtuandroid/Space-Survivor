using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXGenerator : MonoBehaviour
{
    public List<VFXPool> vfxPool = new List<VFXPool>();

    public List<GameObject> generatedResource = new List<GameObject>();

    public static VFXGenerator instance;

    private void Awake()
    {
        instance = this;
    }

    public void EnQueueVFX(VFXType type, GameObject resource)
    {
        for (int i = 0; i < vfxPool.Count; i++)
        {
            if (type == vfxPool[i].type)
            {
                resource.gameObject.SetActive(false);
                vfxPool[i].EnQueue(resource);

                break;
            }
        }
    }

    public void GenerateVFX(VFXType type, Vector2 position)
    {
        for (int i = 0; i < vfxPool.Count; i++)
        {
            if (type == vfxPool[i].type)
            {
                var success = vfxPool[i].DeQueue(position);

                if (!success.success)
                {
                    var vfx = Instantiate(success.Object, position, Quaternion.identity, transform);

                    vfx.transform.localScale = new Vector3(GetParticleSize(type), GetParticleSize(type), GetParticleSize(type));

                    generatedResource.Add(vfx);

                    vfxPool[i].activeParticle.Add(vfx);

                    StartCoroutine(endVFX2());

                    IEnumerator endVFX2()
                    {
                        yield return new WaitForSeconds(vfxPool[i].endTime);

                        EnQueueVFX(vfxPool[i].type, vfx);
                    }

                }
                else
                {
                    StartCoroutine(endVFX());

                    IEnumerator endVFX()
                    {
                        yield return new WaitForSeconds(vfxPool[i].endTime);

                        EnQueueVFX(vfxPool[i].type, success.Object);
                    }

                    vfxPool[i].activeParticle.Add(success.Object);
                }

                break;
            }
        }
    }

    public void AddParticleSize(VFXType type, float size)
    {
        for (int i = 0; i < vfxPool.Count; i++)
        {
            if (type == vfxPool[i].type)
            {
                vfxPool[i].AddParticleSize(size);
            }
        }
    }

    public float GetParticleSize(VFXType type)
    {
        for (int i = 0; i < vfxPool.Count; i++)
        {
            if (type == vfxPool[i].type)
            {
                return vfxPool[i].particleSize;
            }
        }

        return 1;
    }
}

[System.Serializable]
public class VFXPool
{
    public Stack<GameObject> vfxStack = new Stack<GameObject>();

    public List<GameObject> activeParticle = new List<GameObject>();

    public GameObject Prefab;
    public Transform parent;
    public VFXType type;
    public float endTime = 5f;
    public float particleSize = 1f;

    public void EnQueue(GameObject enemy)
    {
        vfxStack.Push(enemy);

        activeParticle.Remove(enemy);
    }

    public Return DeQueue(Vector2 position)
    {
        Return ret = new Return();

        if (vfxStack.Count > 0)
        {
            var vfx = vfxStack.Pop();

            vfx.transform.position = position;
            vfx.SetActive(true);

            ret.Object = vfx;
            ret.success = true;

            return ret;
        }
        else
        {
            ret.Object = Prefab;
            ret.success = false;

            return ret;
        }
    }

    public void AddParticleSize(float size)
    {
        particleSize += size;

        ChangeGeneratedParticleSize();
    }

    private void ChangeGeneratedParticleSize()
    {

        foreach (GameObject particle in vfxStack)
        {
            foreach (ParticleSystem particle1 in particle.GetComponentsInChildren<ParticleSystem>())
            {
                particle1.transform.localScale = new Vector3(particleSize, particleSize, particleSize);
            }
        }

        for (int i = 0; i < activeParticle.Count; i++)
        {

            foreach (ParticleSystem particle1 in activeParticle[i].GetComponentsInChildren<ParticleSystem>())
            {
                particle1.transform.localScale = new Vector3(particleSize, particleSize, particleSize);
            }
        }
    }

    public class Return
    {
        public GameObject Object;
        public bool success = false;
    }
}

public enum VFXType
{
    none, enemyDie1, playerDie1, missileExplode, projectileBlowupSmall, Explode1, Heal1, AtomicExplosion, enemyDie2, enemyDie3
, enemyDie4, UpgradeModuleUnboxing
}
