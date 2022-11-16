using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour
{
    [SerializeField] Texture2D[] CursorImage;
    //0 = basic, 1 = vegetable, 2 = building, 3 = x, 4 = water



    void Start()
    {
        BasicCursor();
    }

    public void BasicCursor()
    {
        Cursor.SetCursor(CursorImage[0], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void VegetableCursor()
    {
        Cursor.SetCursor(CursorImage[1], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void BuildingCursor()
    {
        Cursor.SetCursor(CursorImage[2], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void noBuildingZoneCursor()
    {
        Cursor.SetCursor(CursorImage[3], Vector2.zero, CursorMode.ForceSoftware);
    }
    public void WarterCursor()
    {
        Cursor.SetCursor(CursorImage[4], Vector2.zero, CursorMode.ForceSoftware);
    }


}
