using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private List<string> layersToIgnore;   // Specify the layer(s) to ignore collisions with

    private List<int> layerToIgnoreIndices;                 // Stores a list of layer indices to ignore collisions with

    private void Awake()
    {
        if (layersToIgnore.Count == 0) {
            return;
        }

        // Get the layer index of the layer(s) to ignore
        layerToIgnoreIndices = new List<int>();

        foreach (string layer in layersToIgnore) {
            layerToIgnoreIndices.Add(LayerMask.NameToLayer(layer));
        }

        foreach (int layerIndex in layerToIgnoreIndices) 
        {            
            if (layerIndex == -1)
            {
                Debug.LogError("Layer '" + layerIndex + "' not found. Make sure the layer name is correct.");
                return;
            }

            // If the layer exists, ignore collisions with it and the entity
            Physics2D.IgnoreLayerCollision(gameObject.layer, layerIndex, true);
        }
    }
}
