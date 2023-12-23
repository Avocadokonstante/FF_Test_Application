using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolecularOperations : MonoBehaviour
{
    Vector3 position = Vector3.zero;
    [SerializeField] public GameObject[] atomPrefabs;
    public Transform parentTransform; // Set this in the Inspector
    public Transform atomTransform; // Set this in the Inspector
    public float frameRate;
    public float updateInterval;

    Dictionary<int,int> atomPrefabMap = new Dictionary<int, int>()
    {
        {1, 0},
        {6, 1},
        {7, 2},
        {8, 3}
    };

    /// <summary>
    /// Creates once a connection is established and the first message is received. 
    /// </summary>
    /// <param name="coordinates">List of Vector3 coordinates of the given elements.</param>
    /// <param name="elements">List of elements. 1 for hydrogen, 6 for carbon, ...</param>
    public void createMolecule(List<Vector3> coordinates, List<int> elements){

        MainThreadDispatcher.Enqueue(() =>
        {
            for (int i = 0; i < coordinates.Count; i++)
            {
                // Get the current Vector3 instance using the index
                Vector3 vector = coordinates[i];

                // Perform actions on the current Vector3 instance
                GameObject instantiatedPrefab = Instantiate(atomPrefabs[atomPrefabMap[elements[i]]]);    
                Debug.Log("Prefab instantiated");
                instantiatedPrefab.transform.parent = atomTransform;
                instantiatedPrefab.transform.localPosition = vector;
                instantiatedPrefab.transform.localScale = parentTransform.localScale;
                instantiatedPrefab.name = "" + i; 
            }
        });
    }

 
    /// <summary>
    /// interpolates position of all molecules from current position to target position. 
    /// Assumes targetPositions to have exactly one position per molecule.
    /// Iterates over every atom and starts coroutine for interpolation.
    /// </summary>
    /// <param name="targetPositions">List of coordinates. Assumed to be the same size as number of elements.</param>
    public void MoveTo(List<Vector3> targetPositions)
    {
        // Assuming the targetPositions list has the same number of positions as child objects
        if ((targetPositions != null) && (targetPositions.Count != atomTransform.childCount))
        {
            Debug.LogError("Mismatch between target positions count and child objects count.");
            return;
        }

        // Iterate through child objects and move them to corresponding positions
        for (int i = 0; i < atomTransform.childCount; i++)
        {
            Transform child = atomTransform.GetChild(i);
            // Move the child object to the target position in a Coroutine
            StartCoroutine(Move(child.localPosition, targetPositions[i], child));
        }
    }

    /// <summary>
    /// Coroutine to interpolate an object between a origin and destination coordinate. 
    /// Uses public variable frameRate to determine update frequency.
    /// Uses public variable updateInterval to determine size of each step.
    /// </summary>
    /// <param name="origin">Starting position of the object.</param>
    /// <param name="destination">Desired target position of the object.</param>
    /// <param name="element">The object to be moved</param>
    /// <returns></returns>
    IEnumerator Move(Vector3 origin, Vector3 destination, Transform element)
    {
        float timeEllapsed = 0;   
        float lerpTime = (float)1/frameRate;
        float movementTime = (float)updateInterval;

        while (timeEllapsed < movementTime)
        {
            element.localPosition = Vector3.Lerp(origin,destination,timeEllapsed/movementTime);
            timeEllapsed += lerpTime;
            yield return new WaitForSeconds(lerpTime);
        }
    }
}
