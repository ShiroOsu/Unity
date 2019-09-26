using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

[CustomEditor(typeof(SpawnPrefab))]
public class SpawningEditor : Editor
{
    #region Variables

    Transform nullTransform = null;
    bool headerGrpSpawnPoints = true;

    #endregion Variables

    #region Helper Functions

    private void Spacer()
    {
        EditorGUILayout.Space();
    }
    /// <summary>
    /// Returns a GUIContent with name for inspector and a tooltip.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="toolTip"></param>
    /// <returns></returns>
    private GUIContent ToolTip(string name, string toolTip)
    {
        return new GUIContent(name, toolTip);
    }

    /// <summary>
    /// Returns given GameObject as an ObjectField in the inspector.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="name"></param>
    /// <param name="toolTip"></param>
    /// <param name="changeAble"></param>
    /// <returns></returns>
    private GameObject GameObjectField(GameObject gameObject, string name, string toolTip, bool changeAble = true)
    {
        return gameObject = (GameObject)EditorGUILayout.ObjectField(ToolTip(name, toolTip), gameObject, typeof(GameObject), changeAble);
    }

    /// <summary>
    /// Returns given Transform as an ObjectField in the inspector.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <param name="toolTip"></param>
    /// <param name="changeAble"></param>
    /// <returns></returns>
    private Transform TransformField(Transform transform, string name, string toolTip, bool changeAble = true)
    {
        return transform = (Transform)EditorGUILayout.ObjectField(ToolTip(name, toolTip), transform, typeof(Transform), changeAble);
    }

    /// <summary>
    /// Returns given Integer as an Integer in the inspector.
    /// </summary>
    /// <param name="integer"></param>
    /// <param name="name"></param>
    /// <param name="toolTip"></param>
    /// <returns></returns>
    private int IntField(int integer, string name, string toolTip)
    {
        return integer = EditorGUILayout.IntField(ToolTip(name, toolTip), integer);
    }

    /// <summary>
    /// Returns given Float as an Float in the inspector.
    /// </summary>
    /// <param name="flo"></param>
    /// <param name="name"></param>
    /// <param name="toolTip"></param>
    /// <returns></returns>
    private float FloatField(float flo, string name, string toolTip)
    {
        return flo = EditorGUILayout.FloatField(ToolTip(name, toolTip), flo);
    }

    #endregion Helper Functions

    #region Unity Functions

    public override void OnInspectorGUI()
    {
        SpawnPrefab myTarget = target as SpawnPrefab;

        #region Preload Prefabs
        Spacer();

        // Preload prefabs toggle
        myTarget.preload = EditorGUILayout.BeginToggleGroup(new GUIContent("Preload Prefabs",
            "If hundreds or more prefabs are gonna be in the game" +
            " this is useful for lowering the load it will need to spawn more prefabs. "), myTarget.preload);

        // How many prefabs to preload
        myTarget.quantity = EditorGUILayout.IntField(ToolTip("Quantity to preload",
            "Quantity of prefabs to store in scene."), myTarget.quantity);

        // End toggle
        EditorGUILayout.EndToggleGroup();

        Spacer();
        #endregion Preload Prefabs

        #region Where to Spawn Prefabs

        Assert.IsNotNull(myTarget.prefabToSpawn, "The (enemy) Prefab in spawn script is null.");
        myTarget.prefabToSpawn = GameObjectField(myTarget.prefabToSpawn, "Prefab", "The Prefab that is gonna be spawned.");

        // Are we changing in the inspector?
        EditorGUI.BeginChangeCheck();

        myTarget.spawnPointsSize = IntField(myTarget.spawnPointsSize, "Number of spawn points", "Amount of spawn points.");

        // The list can not be negative
        // Is spawn points less than 0 ? set it to 0, if its greater than 30 ? set it to 30 else set it to the Integer given.
        myTarget.spawnPointsSize = myTarget.spawnPointsSize < 0 ? 0 : myTarget.spawnPointsSize > 30 ? 30 : myTarget.spawnPointsSize;

        // We have made a change in the inspector.
        if (EditorGUI.EndChangeCheck())
        {
            // Adding empty transforms to the spawn point list
            while (myTarget.spawnPos.Count != myTarget.spawnPointsSize)
            {
                // If spawn points size is less than desired size, add more empty transforms
                if (myTarget.spawnPos.Count < myTarget.spawnPointsSize)
                {
                    // looping through spawnPointsSize because it has a higher value than spawnPos
                    for (int i = 0; i < myTarget.spawnPointsSize; i++)
                    {
                        myTarget.spawnPos.Add(nullTransform);
                    }
                }

                // If spawn points size is larger than desired size, remove transforms until correct size
                if (myTarget.spawnPos.Count > myTarget.spawnPointsSize)
                {
                    // looping through spawnPos because it has a higher value than spawnPointsSize
                    for (int i = 0; i < myTarget.spawnPos.Count; i++)
                    {
                        myTarget.spawnPos.RemoveAt(i);
                        if (myTarget.spawnPos.Count == myTarget.spawnPointsSize)
                        {
                            break;
                        }
                    }
                }
            }
        }

        // Header group will open in play mode.
        headerGrpSpawnPoints = EditorGUI.BeginFoldoutHeaderGroup(new Rect(30, 88, 80, 10), headerGrpSpawnPoints, "Spawn Points");

        // Make some space in inspector 
        Spacer(); Spacer(); Spacer();

        if (headerGrpSpawnPoints)
        {
            // Show the list of spawn points in the inspector
            for (int i = 0; i < myTarget.spawnPos.Count; i++)
            {
                Assert.IsNotNull(myTarget.spawnPos[i], "Spawn point " + i + " is null.");
                myTarget.spawnPos[i] = TransformField(myTarget.spawnPos[i], "Spawn point " + i, "Spawn position of prefab to spawn.");
            }
        }

        // Start toggle
        EditorGUI.EndFoldoutHeaderGroup();

        Spacer();
        #endregion Where to Spawn Prefabs

        #region Spawn Prefabs on a timer

        myTarget.bTimer = EditorGUILayout.BeginToggleGroup(ToolTip("Spawn prefabs on timer", "The times given will be the same for every spawn point."), myTarget.bTimer);
        myTarget.initialSpawnTime = FloatField(myTarget.initialSpawnTime, "Time before first spawn", "The time it takes for the first prefab to spawn." +
            "After it will be the interval time.");
        myTarget.spawnCooldown = FloatField(myTarget.spawnCooldown, "Interval between each spawn.", "The time delay between each spawned prefab.");

        // End toggle
        EditorGUILayout.EndToggleGroup();

        Spacer();
        #endregion Spawn Prefabs on a timer

        #region Spawn set amount of Prefabs

        // Start toggle
        myTarget.bSetAmount = EditorGUILayout.BeginToggleGroup(ToolTip("Spawn a set number of prefabs", ""), myTarget.bSetAmount);

        myTarget.spawnAmount = IntField(myTarget.spawnAmount, "Number of prefabs to spawn.", "");
        myTarget.wait = FloatField(myTarget.wait, "Time between each spawn.", "");

        // End toggle
        EditorGUILayout.EndToggleGroup();

        #endregion Spawn set amount of Prefabs

    }

    #endregion Unity Functions
}
