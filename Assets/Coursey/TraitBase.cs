using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitBase : MonoBehaviour
{
    public GameObject self = null;
    public bool isRequired = false;
    public string traitName = string.Empty;
    public string debugMessage = string.Empty;
    
    public virtual void InitializeTrait()
    {
        self = gameObject;
        debugMessage += $"Adding the {traitName} trait.\n";
    }
    public virtual void SendDebugMessage()
    {
        Debug.Log(debugMessage);
        debugMessage = string.Empty;
    }
}
