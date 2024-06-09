using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonInstance<UIManager>
{
    public enum playStatus
    {
        mainMenu,
        playStage,
        Die,
        Clear
    }

    public Dictionary<playStatus, GameObject[]> UIs = new Dictionary<playStatus, GameObject[]>();

    public GameObject[] playStageStatusUIs;
    public GameObject[] mainMenuStatusUIs;
    public GameObject[] DieStatusUIs;
    public GameObject[] ClearStatusUIs;


    public void UI_PlayStatus()
    {
        foreach (var ui in playStageStatusUIs)
            ui.SetActive(true);

        foreach (var ui in mainMenuStatusUIs)
            ui.SetActive(true);

        foreach (var ui in DieStatusUIs)
            ui.SetActive(true);

        foreach (var ui in ClearStatusUIs)
            ui.SetActive(true);
    }
}
