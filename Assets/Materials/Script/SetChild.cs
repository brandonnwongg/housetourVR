using UnityEngine;

public class SetChild : MonoBehaviour
{
    public GameObject childObject;  // The object to be made a child
    public GameObject parentObject; // The object to be the parent

    void Start()
    {
        if (childObject != null && parentObject != null)
        {
            // Set the childObject's parent to parentObject
            childObject.transform.SetParent(parentObject.transform);

            // Optional: Reset the child's position relative to the parent
            childObject.transform.localPosition = Vector3.zero;
            childObject.transform.localRotation = Quaternion.identity;
            childObject.transform.localScale = Vector3.one;

            Debug.Log($"{childObject.name} is now a child of {parentObject.name}");
        }
        else
        {
            Debug.LogError("Child or Parent object is not assigned.");
        }
    }
}
