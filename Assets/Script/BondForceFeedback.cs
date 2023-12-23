using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BondForceFeedback
{
    public Transform atomParent; // Set this in the Inspector

    private BondOperation bo;

    private bool MolecularOperations = false;

    private void GetBondOperations(){
        MolecularOperations = true;
        bo = new BondOperation();
    }
    public void OnGrabTriggered(string bondName, float distanceToMove)
    {
        if(!MolecularOperations){
            GetBondOperations();
        }
        //Tuple<int, int> atoms = bo.GetConnectedAtoms(bondName);
        MoveSphereAway(0, 2, distanceToMove);
    } 

    private void MoveSphereAway(int atomA, int atomB, float distanceToMove)
    {
        // Calculate the direction from the stick to the sphere
        Vector3 A = atomParent.GetChild(atomA).transform.localPosition;
        Vector3 B = atomParent.GetChild(atomB).transform.localPosition;

        Vector3 bondDirection = A - B;

        Vector3 planeNormal = bondDirection.normalized; // Adjust this based on your stick's orientation
        distanceToMove = 0.1f;
        float signedDistance;

        for (int i = 0; i < atomParent.childCount - 1; i++)
        {
            if(i != atomA || i != atomB){
                Vector3 atom = atomParent.GetChild(i).transform.localPosition;
                
                Vector3 atomDirection = A - atom;
                signedDistance = Vector3.Dot(atomDirection, planeNormal);
                if(signedDistance >= 0){
                    signedDistance = 1;
                }else{
                    signedDistance = -1;
                }

                Vector3 newPosition = atom + bondDirection.normalized * signedDistance * distanceToMove;
                atomParent.GetChild(i).transform.localPosition = newPosition;
            }
        }   
    }
}
