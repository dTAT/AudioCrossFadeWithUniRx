using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Toolkit;

/// <summary>
///
/// </summary>
public class AudioPool : MonoBehaviour
{
    private class Pool : ObjectPool<AudioEntity>
    {
        GameObject prefabOriginal;
        Transform parentTransform;
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="parentTransform_">親のtransform</param>
        /// <param name="prefabOriginal_">生成元prefab</param>
        public Pool(Transform parentTransform_, GameObject prefabOriginal_)
        {
            parentTransform = parentTransform_;
            prefabOriginal = prefabOriginal_;
        }
        /// <summary>
        /// 新規にインスタンスを生成する
        /// </summary>
        /// <returns>AudioEntity</returns>
        protected override AudioEntity CreateInstance()
        {
            var go = GameObject.Instantiate(prefabOriginal);
            go.transform.SetParent(parentTransform);
            return go.GetComponent<AudioEntity>();
        }
    }

    Pool pool = null;
    [SerializeField]
    AudioEntity entityPrefab;
    public int PoolSize = 32;
    const int CreateEachFrame = 2;
    /// <summary>
    /// /
    /// </summary>
    AudioEntity.Factory factory;
    public void Awake()
    {
        pool = new Pool(this.transform, entityPrefab.gameObject);
        pool.PreloadAsync(PoolSize, CreateEachFrame);
    }

    public AudioEntity Rent()
    {
        return pool.Rent();
    }
    public void Return(AudioEntity entity)
    {
        pool.Return(entity);
    }

    public int Count
    {
        get
        {
            return pool.Count;
        }
    }
}