using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDock : MonoBehaviour
{
    GameObject manager;
    
    // Start is called before the first frame update
    void Start()
    {
       manager = GameObject.FindGameObjectsWithTag("Manager")[0];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setHandOpen(bool open){
        manager.GetComponent<VariableManager>().setHandOpen(open);
    }
}