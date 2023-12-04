using UnityEngine;

public class AtomType : MonoBehaviour
{
    public enum Type {H, O, N, C};  // Enum with types
    [SerializeField] public Type atomType; // Set Atomtype for each Prefab
}
