using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ControllerFirstSelect : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;

    [Header("Ui First Selected")]
    [SerializeField] private GameObject menuFirstSelect;
    [SerializeField] private GameObject pauseFirstSelect;
    [SerializeField] private GameObject settingsFirstSelect;
    [SerializeField] private GameObject audioFirstSelect;
    [SerializeField] private GameObject videoFirstSelect;
    [SerializeField] private GameObject controlsFirstSelect;
    [SerializeField] private GameObject EndOfDay;

    private void Update()
    {
        //if(eventSystem.currentSelectedGameObject != null)
        //{
        //    Debug.Log(eventSystem.currentSelectedGameObject.name);
        //}    
    }

    internal bool IsControllerConnected()
    {
        return Gamepad.all.Count > 0;
    }

    internal void SetFirstSelect(string menu)
    {
        switch (menu)
        {
            case "Menu": SetFirst(menuFirstSelect); break;
            case "Pause": SetFirst(pauseFirstSelect); break;
            case "Settings": SetFirst(settingsFirstSelect); break;
            case "Audio": SetFirst(audioFirstSelect); break;
            case "Video": SetFirst(videoFirstSelect); break;
            case "Controls": SetFirst(controlsFirstSelect); break;
            case "EndOfDay": SetFirst(EndOfDay); break;
            default: Debug.LogWarning("First Select is not set for: " + menu); break;
        }
    }

    private void SetFirst(GameObject firstSelect)
    {
        eventSystem.SetSelectedGameObject(firstSelect);
        Debug.Log("First Select: " + firstSelect.name);
    }

    internal void ClearSelected()
    {
        eventSystem.SetSelectedGameObject(null);
    }
}
