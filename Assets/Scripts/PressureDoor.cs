/*
 * Author: Muhammad Farhan
 * Date: 9/5/25
 * Description: Script for doors triggered by pressure pads
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureDoor : MonoBehaviour
{
    public bool IsDoorOpen = false;
    [SerializeField] private int requiredSwitchesToOpen = 1;
    private List<PressurePad> currentSwitchesOpen = new();
    public int CurrentSwitchesOpen => currentSwitchesOpen.Count;

    private Animator animator;

    [SerializeField] private AudioClip doorSFXClip;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void AddPressureSwitch(PressurePad currentSwitch)
    {
        if (!currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Add(currentSwitch);
        }
        TryOpen();
    }

    public void RemovePressureSwitch(PressurePad currentSwitch)
    {
        if (currentSwitchesOpen.Contains(currentSwitch))
        {
            currentSwitchesOpen.Remove(currentSwitch);
        }
        TryOpen();
    }

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
            //this.gameObject.SetActive(true);
            IsDoorOpen = false;
        }
    }

    private void OpenDoor()
    {
        if (!IsDoorOpen)
        {
            animator.SetBool("Open", true);  // open
            AudioManager.Instance.PlaySFX(doorSFXClip, transform.position);
            //this.gameObject.SetActive(false);
            IsDoorOpen = true;
        }
    }
}
