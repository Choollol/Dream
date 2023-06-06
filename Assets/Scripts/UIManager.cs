using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public enum UI
    {
        None, Paused, WakeUp, Settings, Controls
    }

    public GameManager gameManager;

    public List<GameObject> uiList;

    public UI currentUI;

    private Dictionary<string, GameObject> uiDict = new Dictionary<string, GameObject>();
    void Start()
    {
        foreach (GameObject ui in uiList)
        {
            uiDict.Add(ui.name, ui);
        }

        currentUI = UI.None;

        //debug
    }

    void Update()
    {

    }
    public void ClearUI()
    {
        foreach (GameObject ui in uiList)
        {
            ui.SetActive(false);
        }
    }
    
    public void OpenPaused()
    {
        ClearUI();
        uiDict["Paused UI"].SetActive(true);
        currentUI = UI.Paused;
    }
    public void OpenWakeUp()
    {
        ClearUI();
        uiDict["Wake Up UI"].SetActive(true);
        currentUI = UI.WakeUp;
    }
    public void OpenSettings()
    {
        ClearUI();
        uiDict["Settings UI"].SetActive(true);
        currentUI = UI.Settings;
    }
    public void OpenControls()
    {
        ClearUI();
        uiDict["Controls UI"].SetActive(true);
        currentUI = UI.Controls;
    }
}
