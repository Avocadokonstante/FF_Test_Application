using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractionScript : MonoBehaviour
{
    public string targetTag = "atom"; // Tag of the objects you want to interact with.
    public GameObject spheres; // Reference to the GameObject containing the spheres.
    private XRBaseInteractable grabbedObject; // Reference to the grabbed object.
    private Material originalMaterial;
    private Vector3 originalScale;

    public static string name;
    public static bool isGrabbed = false;
    public static Vector3 position; 
    private void OnEnable()
    {
        // Attach event listeners for grab interaction.
        GetComponent<XRDirectInteractor>().onSelectEntered.AddListener(OnGrab);
        GetComponent<XRDirectInteractor>().onSelectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        // Remove event listeners.
        GetComponent<XRDirectInteractor>().onSelectEntered.RemoveListener(OnGrab);
        GetComponent<XRDirectInteractor>().onSelectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(XRBaseInteractable interactor)
    {
        // Check if the grabbed object has the specified tag.
        XRBaseInteractable interactable = GetComponent<XRDirectInteractor>().selectTarget;
        if (interactable.tag.Equals(targetTag))
        {
            // Show the spheres.
            spheres.SetActive(false);
            isGrabbed = true;
            grabbedObject = interactable;

            // Set the scale of the grabbed object based on the controller's scale.
            spheres.transform.localScale = grabbedObject.transform.lossyScale;
            // Assign the new material to the grabbed object.
            spheres.GetComponent<Renderer>().material = grabbedObject.GetComponent<Renderer>().material;
            // Set the local position of the grabbed object based on its position relative to the controller.
            spheres.transform.position = grabbedObject.transform.position;
            // Set gameObject name
            name = grabbedObject.name;
            // Set position for Unity Socket script to override the atoms position
            position = grabbedObject.transform.position;
        }
    }

    private void OnRelease(XRBaseInteractable interactor)
    {
        // Hide the spheres.
        spheres.SetActive(false);
        isGrabbed = false;
    }

    private void Update()
    {
        // If a grabbed object is available, override its position with the sphere's position.
        if (isGrabbed && grabbedObject != null)
        {
            grabbedObject.transform.position = spheres.transform.position;
            position = grabbedObject.transform.localPosition; // Set for UnitySocket script 
        }
    }
}
