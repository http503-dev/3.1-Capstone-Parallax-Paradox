/*
 * Author: Muhammad Farhan
 * Date: 15/5/25
 * Description: Script for the forcefields that prevent players from bring props through different levels
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Targetable"))
        {
            Superliminal controller = FindObjectOfType<Superliminal>();
            if (controller.target != null && controller.target.gameObject == other.gameObject)
            {
                controller.ForceDrop();
            }
        }
    }
}
