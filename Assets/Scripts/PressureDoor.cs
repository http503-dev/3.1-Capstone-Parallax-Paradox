/*
 * Author: Muhammad Farhan
 * Date: 9/5/25
 * Description: Script for doors triggered by pressure pads
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls a door that opens when a specified number of linked pressure pads are activated.
/// </summary>
public class PressureDoor : MonoBehaviour
{
    /// <summary>
    /// Whether the door is currently open.
    /// </summary>
    public bool IsDoorOpen = false;

    [SerializeField] private int requiredSwitchesToOpen = 1;
    private List<PressurePad> currentSwitchesOpen = new();

    /// <summary>
    /// The number of currently activated pressure pads linked to this door.
    /// </summary>
    public int CurrentSwitchesOpen => currentSwitchesOpen.Count;

    private Animator animator;
    [SerializeField] private AudioClip doorSFXClip;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Adds an activated pressure pad to the list and checks if the door should open.
    /// </summary>
    /// <param name="currentSwitch">The pressure pad being activated.</param>
    public void AddPressureSwitch(PressurePad currentSwitch)
    {
        if (!currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Add(currentSwitch);
        }
        TryOpen();
    }

    /// <summary>
    /// Removes a deactivated pressure pad from the list and checks if the door should close.
    /// </summary>
    /// <param name="currentSwitch">The pressure pad being deactivated.</param>
    public void RemovePressureSwitch(PressurePad currentSwitch)
    {
        if (currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Remove(currentSwitch);
        }
        TryOpen();
    }

    /// <summary>
    /// Determines whether to open or close the door based on the number of active pads.
    /// </summary>
    private void TryOpen()
    {
        if (CurrentSwitchesOpen == requiredSwitchesToOpen)
        {
            OpenDoor();
        }
        else if (CurrentSwitchesOpen < requiredSwitchesToOpen)
        {
            CloseDoor();
        }
    }

    private void CloseDoor()
    {
        if (IsDoorOpen)
        {
            animator.SetBool("Open", false); // close
            AudioManager.Instance.PlaySFX(doorSFXClip, transform.position);
            IsDoorOpen = false;
        }
    }

    private void OpenDoor()
    {
        if (!IsDoorOpen)
        {
            animator.SetBool("Open", true);  // open
            AudioManager.Instance.PlaySFX(doorSFXClip, transform.position);
            IsDoorOpen = true;
        }
    }
}
