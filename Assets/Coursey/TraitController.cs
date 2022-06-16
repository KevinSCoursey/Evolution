using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraitController : MonoBehaviour
{
    public List<TraitBase> traits = new();

    public void Start()
    {
        GetTraits();
        InitializeAllTraits();
    }

    private List<TraitBase> GetTraits()
    {
        var components = gameObject.GetComponents<Component>();

        foreach (var component in components)
        {
            if (component.ToString().Contains("Trait_"))
            {
                traits.Add((TraitBase)component);
            }
        }
        return traits;
    }
    private void InitializeAllTraits()
    {
        foreach (var trait in traits)
        {
            trait.InitializeTrait();
        }
    }
}
