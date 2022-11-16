using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitManager : MonoBehaviour
{
    public void OnButtonExit()
    {
        SaveLoadManager.INSTANCE.Save();
        Application.Quit();
    }
}
