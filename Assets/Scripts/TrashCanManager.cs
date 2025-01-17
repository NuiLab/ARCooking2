using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCanManager : MonoBehaviour
{
    GameObject globalRecords_GO;                    // Reference to global records
    string[] objects;                               // List of tags which can be stacked

    // Start is called before the first frame update
    void Start()
    {
        globalRecords_GO = GameObject.FindWithTag("Global Records");
        objects = globalRecords_GO.GetComponent<Records>().GetAllObjectTags();
    }

    // Update is called once per frame
 

    private void OnTriggerStay(Collider other)
    {
        if (Array.IndexOf(objects, other.gameObject.tag) != -1)
        {
            if (!other.GetComponent<ObjectManager>().isGrabbed)
                Destroy(other.gameObject);
        }
    }
}
