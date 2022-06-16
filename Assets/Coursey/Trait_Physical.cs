using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trait_Physical : TraitBase
{
    private object[,] characteristicsMatrix = { 
        //Name of characteritic, its value, its min, its max
        /* 0 */{ "Size",            -1f, 0.1f, 1f},
        /* 1 */{ "Density",         -1f, 0.1f, 1f},
        /* 2 */{ "MassMultiplier",   1f, 0.1f, 1f},
        /* 3 */{ "Color_R",         -1f, 0.1f, 1f},
        /* 4 */{ "Color_G",         -1f, 0.1f, 1f},
        /* 5 */{ "Color_B",         -1f, 0.1f, 1f}
    };

    public float size = -1;
    public float density = -1;
    public Color color = new Color(-1f, -1f, -1f);

    public float mass = -1;
    public float massMultiplier = 1f;

    public Rigidbody rb = null;

    
    public override void InitializeTrait()
    {
        isRequired = true;
        traitName = "Physical";
        base.InitializeTrait();
        rb = self.GetComponent<Rigidbody>() == null ? AddRigidbody(): self.GetComponent<Rigidbody>();
        AssignCharacteristics();
        SendDebugMessage();
    }

    private Rigidbody AddRigidbody()
    {
        debugMessage += $"A Rigidbody wasn't found, so one was added.";
        Rigidbody rigidbody = self.AddComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        return rigidbody;
    }

    private void AssignCharacteristics()
    {
        //Iterates through characteristics list
        for(int i = 0; i < characteristicsMatrix.GetLength(0); i++)
        {
            if((float)characteristicsMatrix[i,1] <= 0f)
            {
                characteristicsMatrix[i, 1] = Random.Range((float)characteristicsMatrix[i, 2], (float)characteristicsMatrix[i, 3]);
                debugMessage += $"The {characteristicsMatrix[i,0]} was undefined " +
                    $"or <= 0 and has been randomly set to {characteristicsMatrix[i, 1]}.\n";
            }
        }
        //Actually assigning values to characteristics
        size = (float)characteristicsMatrix[0, 1];
        density = (float)characteristicsMatrix[1, 1];
        massMultiplier = (float)characteristicsMatrix[2, 1];
        mass = size * density * massMultiplier;
        color = new Color((float)characteristicsMatrix[3, 1], (float)characteristicsMatrix[4, 1], (float)characteristicsMatrix[5, 1]);
    }
}
