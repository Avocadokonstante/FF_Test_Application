using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreateBonds : MonoBehaviour
{
    [SerializeField] public GameObject bondPrefab;
    static bool bondscreated = false;
    public float cutoff;
    public Transform atomParent;
    public Transform bondParent;
    public Transform parentTransform;

    /// <summary>
    /// This mehtod is called every frame, it resets and rescaled the bond objects according to the atom movement
    /// and resets the visibility of those objects.
    /// </summary> 
    public void moveBonds(){
        int bondcounter = 0;
        for (int i = 0; i < atomParent.childCount - 1; i++)
        {
            for (int j = i + 1; j < atomParent.childCount; j++)
            {
                if (i != j )
                {
                    Vector3 start = atomParent.GetChild(i).transform.localPosition;
                    Vector3 end = atomParent.GetChild(j).transform.localPosition;

                    float distance = Vector3.Distance(start, end);                    
                    Vector3 bondPosition = (start + end) / 2f;
                    Vector3 bondScale = new Vector3(0.05f, Vector3.Distance(start, end) / 2f, 0.05f);

                    GameObject bond = bondParent.GetChild(bondcounter).gameObject;

                    bond.transform.localPosition = bondPosition;
                    Quaternion rotation = Quaternion.FromToRotation(Vector3.up, end - start);
                    Quaternion finalRotation = parentTransform.rotation * rotation;

                    bond.transform.rotation = finalRotation;
                    bond.transform.localScale = bondScale;

                    string atomTypeA = atomParent.GetChild(i).gameObject.GetComponent<AtomType>().atomType.ToString();
                    string atomTypeB = atomParent.GetChild(j).gameObject.GetComponent<AtomType>().atomType.ToString();

                    setVisibility(distance, bond, atomTypeA, atomTypeB);

                    bondcounter++;
                }
            }
        }     
    }

    /// <summary>
    /// This method is called once, when the first configuration for a molecule is available. It must be called
    /// after the atoms are instantiated, it creates bond objects that connect atoms in an all-to-all mesh. 
    /// </summary>
    public void calculateBonds(){
        MainThreadDispatcher.Enqueue(() =>
        {
            for (int i = 0; i < atomParent.childCount - 1; i++)
            {
                for (int j = i + 1; j < atomParent.childCount; j++)
                {
                    Vector3 start = atomParent.GetChild(i).transform.localPosition;
                    Vector3 end = atomParent.GetChild(j).transform.localPosition;
                    float distance = Vector3.Distance(start, end);  
                    string atomTypeA = atomParent.GetChild(i).gameObject.GetComponent<AtomType>().atomType.ToString();
                    string atomTypeB = atomParent.GetChild(j).gameObject.GetComponent<AtomType>().atomType.ToString();                  
                    
                    createBond(start, end, distance, atomTypeA, atomTypeB);
                }
            }
        });
        bondscreated = true;  
    }

    /// <summary>
    /// This method instanciates a single bond with a bond prefab, and then scales and rotates it so it fits between
    /// according to two atom coordinates.
    /// </summary>
    /// <param name="start"> coordinates of atom 1 </param>
    /// <param name="end"> coordinates of atom 2 </param>
    /// <param name="distance"> the distance between the two atom coordinates = bond length </param>
    /// <param name="atomTypeA"> the element of atom A as string </param>
    /// <param name="atomTypeB"> the element of atom B as string </param>
    void createBond(Vector3 start, Vector3 end, float distance, string atomTypeA, string atomTypeB)
    {
        Vector3 bondPosition = (start + end) / 2f;
        Vector3 bondScale = new Vector3(0.05f, Vector3.Distance(start, end) / 2f, 0.05f);

        GameObject bond = Instantiate(bondPrefab, bondPosition, Quaternion.identity);
        bond.transform.SetParent(bondParent);
        bond.transform.rotation = parentTransform.rotation;

        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, end - start);
        bond.transform.rotation = rotation;
        bond.transform.localScale = parentTransform.localScale;

        setVisibility(distance, bond, atomTypeA, atomTypeB);  
    }

    // covalent radii, from Pyykko and Atsumi, Chem. Eur. J. 15, 2009, 188-197:
    // "H": 0.32,"He": 0.46,"Li": 1.2,"Be": 0.94,"B": 0.77,"C": 0.75,"N": 0.71,"O": 0.63,"F": 0.64,"Ne": 0.67,"Na": 1.4,"Mg": 1.25,"Al": 1.13,"Si": 1.04,"P": 1.1,"S": 1.02,"Cl": 0.99,"Ar": 0.96,"K": 1.76,"Ca": 1.54,"Sc": 1.33,"Ti": 1.22,"V": 1.21,"Cr": 1.1,"Mn": 1.07,"Fe": 1.04,"Co": 1.0,"Ni": 0.99,"Cu": 1.01,"Zn": 1.09,"Ga": 1.12,"Ge": 1.09,"As": 1.15,"Se": 1.1,"Br": 1.14,"Kr": 1.17,"Rb": 1.89,"Sr": 1.67,"Y": 1.47,"Zr": 1.39,"Nb": 1.32,"Mo": 1.24,"Tc": 1.15,"Ru": 1.13,"Rh": 1.13,"Pd": 1.08,"Ag": 1.15,"Cd": 1.23,"In": 1.28,"Sn": 1.26,"Sb": 1.26,"Te": 1.23,"I": 1.32,"Xe": 1.31,"Cs": 2.09,"Ba": 1.76,"La": 1.62,"Ce": 1.47,"Pr": 1.58,"Nd": 1.57,"Pm": 1.56,"Sm": 1.55,"Eu": 1.51,"Gd": 1.52,"Tb": 1.51,"Dy": 1.5,"Ho": 1.49,"Er": 1.49,"Tm": 1.48,"Yb": 1.53,"Lu": 1.46,"Hf": 1.37,"Ta": 1.31,"W": 1.23,"Re": 1.18,"Os": 1.16,"Ir": 1.11,"Pt": 1.12,"Au": 1.13,"Hg": 1.32,"Tl": 1.3,"Pb": 1.3,"Bi": 1.36,"Po": 1.31,"At": 1.38,"Rn": 1.42,"Fr": 2.01,"Ra": 1.81,"Ac": 1.67,"Th": 1.58,"Pa": 1.52,"U": 1.53,"Np": 1.54,"Pu": 1.55
    // Add as needed
    private Dictionary<string, float> rcov = new Dictionary<string, float>
    {
        {"H", 0.32f},
        {"C", 0.75f},
        {"N", 0.71f},
        {"O", 0.63f}    
    };

    private float k1 = 16.0f;
    private float k2 = 4.0f/3.0f;

    /// <summary>
    /// This method checks and sets the visibility of bonds partially based on code from Robert Paton's Sterimol script, which based this part on Grimme's D3 code, by dis/enabeling
    /// the mesh renderer. 
    /// </summary>
    /// <param name="distance"> the distance between the two atom coordinates = bond length </param>
    /// <param name="bond"> a reference to the dedicated bond object, that is to be made visible or invisible </param> the  <summary>
    /// <param name="atomTypeA"> the element of atom A as string </param>
    /// <param name="atomTypeB"> the element of atom B as string </param>
    void setVisibility(float distance, GameObject bond, string atomTypeA, string atomTypeB)
    {
        // Check if element is key in rcov Dictionary
        if (rcov.ContainsKey(atomTypeA) && rcov.ContainsKey(atomTypeB))
        {
            // Retrieve the radius value from the dictionary
            float radiusValueA = rcov[atomTypeA];
            float radiusValueB = rcov[atomTypeB];

            // based on Grimme's D3 code
            double rco = radiusValueA + radiusValueB;
            rco = rco * k2;
            double rr = rco / distance;
            double damp = 1.0 / (1.0 + Math.Exp(-k1 * (rr - 1.0))); 

            if (damp > 0.85)
            {
                bond.GetComponent<MeshRenderer>().enabled = true;
            } 
                else 
            {
                bond.GetComponent<MeshRenderer>().enabled = false;
            }

        }
            else
        {
            Debug.LogWarning("Object Type " + atomTypeA + "or" + atomTypeB + " not found in dictionary.");
        }
    }
    
    void Start()
    {
        calculateBonds();
    }
    void Update()
    {
        if (bondscreated)
        {
            moveBonds();
        }
    }
}
