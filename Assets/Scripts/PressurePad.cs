/*
 * Author: Muhammad Farhan
 * Date: 9/5/25
 * Description: Script for pressure pad mechanic
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class PressurePad : MonoBehaviour
{
    [SerializeField] private PressureDoor currentDoor;
    [SerializeField] private Animator animator;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Superliminal"))
        {
            currentDoor.AddPressureSwitch(this);
            animator.SetBool("Down", true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Superliminal"))
        {
            currentDoor.RemovePressureSwitch(this);
            animator.SetBool("Down", false);
        }
    }
}
