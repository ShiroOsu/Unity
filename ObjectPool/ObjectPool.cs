using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    // Pool size.
    public const int DEFAULT_POOL_SIZE = 10;

    // Index name.
    public const string DEFAULT_NAME = "Pooled";

    /// <summary>
    /// The Pool class represents the pool for a particular GameObject(prefab).
    /// </summary>
    class Pool
    {

        // Index for pooled prefabs.
        int nextID = 0;

        // Stack("Pool") of inactive GameObjects.
        Stack<GameObject> inactive;

        // GameObject to spawn.
        GameObject prefab;

        // Empty GameObject to clean up hierarchy.
        GameObject parent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param qty="initialQuantity"></param>
        public Pool(GameObject prefab, int initialQuantity)
        {
            this.prefab = prefab;

            // Naming of our Empty GameObject in hierarchy.
            parent = new GameObject(prefab.name + "'s");

            inactive = new Stack<GameObject>(initialQuantity);
        }

        // Spawn Object from pool.
        // position, rotation, name
        public GameObject Spawn(Vector3 pos, Quaternion rot, string name = DEFAULT_NAME)
        {
            GameObject gameObject;

            // If our ObjectPool is empty,
            // instantiate a new object.
            if (inactive.Count == 0)
            {
                gameObject = (GameObject)Instantiate(prefab, pos, rot);
                gameObject.name = prefab.name + "(" + name + " " + (nextID++) + ")";

                // PoolMember component to know what pool object belongs to.
                gameObject.AddComponent<PoolMember>().myPool = this;
            }
            else
            {
                // Otherwise, grab the last object in the pool.
                gameObject = inactive.Pop();

                // If the expected object no longer exists try next sequence.
                if (gameObject == null)
                {
                    return Spawn(pos, rot);
                }
            }

            gameObject.transform.parent = parent.transform;
            gameObject.transform.position = pos;
            gameObject.transform.rotation = rot;
            gameObject.SetActive(true);
            return gameObject;
        }

        // Return an Object to the inactive pool.
        public void Despawn(GameObject gameObject)
        {
            gameObject.SetActive(false);
            inactive.Push(gameObject);
        }

    }

    /// <summary>
    /// Added to newly instantiated objects to link back to correct pool.
    /// </summary>
    class PoolMember : MonoBehaviour
    {
        public Pool myPool;
    }


    // All pools.
    static Dictionary<GameObject, Pool> pools;

    // Initialize Dictionary.
    static void Init(GameObject prefab = null, int qty = DEFAULT_POOL_SIZE)
    {
        if (pools == null)
        {
            pools = new Dictionary<GameObject, Pool>();
        }

        if (prefab != null && pools.ContainsKey(prefab) == false)
        {
            pools[prefab] = new Pool(prefab, qty);
        }
    }

    // Preload copies of an GameObject at the start of a scene.
    static public void Preload(GameObject prefab, int qty = DEFAULT_POOL_SIZE, string name = DEFAULT_NAME)
    {
        Init(prefab, qty);

        // Make an Array to grab all of the objects we are about to pre-load.
        GameObject[] gameObjects = new GameObject[qty];

        for (int i = 0; i < qty; i++)
        {
            gameObjects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity, name);
        }

        // Despawn the objects.
        for (int i = 0; i < qty; i++)
        {
            Despawn(gameObjects[i]);
        }

    }

    // Spawns a copy of the specified prefab.
    static public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot, string name = DEFAULT_NAME)
    {
        Init(prefab);

        return pools[prefab].Spawn(pos, rot, name);
    }

    // Despawn the specified GameObject into its pool.
    static public void Despawn(GameObject gameObject)
    {
        PoolMember poolMember = gameObject.GetComponent<PoolMember>();

        if (poolMember == null)
        {
            Debug.Log("Objects '" + gameObject.name +"' wasn't spawned from a pool. Destroying it instead.");
            GameObject.Destroy(gameObject);
        }
        else
        {
            poolMember.myPool.Despawn(gameObject);
        }
    }
}
