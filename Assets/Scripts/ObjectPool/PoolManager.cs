using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
#region 宣告
    public List<GameObject> poolPrefabs;
    private List<ObjectPool<GameObject>> poolEffectList = new List<ObjectPool<GameObject>>();

    private Queue<GameObject> soundQueue = new Queue<GameObject>();
#endregion

#region 事件函數
    private void OnEnable()
    {
        EventHandler.ParticleEffectEvent += OnParticleEffectEvent;
        EventHandler.InitSoundEffect += InitSoundEffect;
    }

    private void OnDisable()
    {
        EventHandler.ParticleEffectEvent -= OnParticleEffectEvent;
        EventHandler.InitSoundEffect -= InitSoundEffect;
    }

    private void Start()
    {
        CreatPool();
    }
#endregion

#region 註冊事件
    private void OnParticleEffectEvent(ParticleEffectType effectType, Vector3 pos)
    {
        //WORKFLOW : 根據特效補全
        ObjectPool<GameObject> objPool = effectType switch
        {
            ParticleEffectType.LeaveFalling01 => poolEffectList[0],
            ParticleEffectType.LeaveFalling02 => poolEffectList[1],
            ParticleEffectType.Rock => poolEffectList[2],
            ParticleEffectType.ReapableScenery => poolEffectList[3],
            _ => null,
        };

        GameObject obj = objPool.Get();
        obj.transform.position = pos;
        StartCoroutine(ReleaseRoutine(objPool, obj));
    }
    
    private void InitSoundEffect(SoundDetails soundDetails)
    {
        var obj = GetPoolObject();
        obj.GetComponent<Sound>().SetSound(soundDetails);
        obj.SetActive(true);
        StartCoroutine(DisableSound(obj, soundDetails.soundClip.length));
    }
#endregion

    /// <summary>
    /// 生成對象池
    /// </summary>
    private void CreatPool()
    {
        foreach (GameObject item in poolPrefabs)
        {
            Transform parent = new GameObject(item.name).transform;
            parent.SetParent(transform);

            var newPool = new ObjectPool<GameObject>(
                () => Instantiate(item, parent),
                e => { e.SetActive(true);},
                e => { e.SetActive(false);},
                e=>{Destroy(e);}
                );
            
            poolEffectList.Add(newPool);
        }
    }

    private IEnumerator ReleaseRoutine(ObjectPool<GameObject> pool, GameObject obj)
    {
        yield return new WaitForSeconds(1.5f);
        pool.Release(obj);
    }

    private void CreatSoundPool()
    {
        var parent = new GameObject(poolPrefabs[4].name).transform;
        parent.SetParent(transform);

        for (int i = 0; i < 20; i++)
        {
            GameObject newObj = Instantiate(poolPrefabs[4], parent);
            newObj.SetActive(false);
            soundQueue.Enqueue(newObj);
        }
    }

    private GameObject GetPoolObject()
    {
        if (soundQueue.Count < 2)
        {
            CreatSoundPool();
        }

        return soundQueue.Dequeue();
    }

    private IEnumerator DisableSound(GameObject obj, float duration)
    {
        yield return new WaitForSeconds(duration);
        obj.SetActive(false);
        soundQueue.Enqueue(obj);
    }
    
}
