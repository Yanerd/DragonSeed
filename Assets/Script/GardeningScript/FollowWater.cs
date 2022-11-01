using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWater : MonoBehaviour
{
    //=================================================
    [SerializeField] float dropSpeed = -1f;
    [SerializeField] float dropPoint_Ypos = 1f;
    [SerializeField] float MousePositionMinX = -0.5f;
    [SerializeField] float MousePositionMaxX = 10.5f;
    [SerializeField] float MousePositionMinz = -0.5f;
    [SerializeField] float MousePositionMaxZ = 10.5f;
    Vector3 FollowPoint;
    //=================================================

    void Update()
    {
        //if (ButtonManager.inst.buildingMode == true) return;
        ////=================================================
        //#region WaterDrag&Drop
        //if (Input.GetMouseButton(0))
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit, 1000))
        //    {
        //        FollowPoint = hit.point;
        //        FollowPoint.y = dropPoint_Ypos;
        //        this.transform.position = FollowPoint;

        //        if (FollowPoint.x <= MousePositionMinX)
        //        {
        //            FollowPoint.x = MousePositionMinX;
        //        }

        //        if (FollowPoint.z <= MousePositionMinz)
        //        {
        //            FollowPoint.z = MousePositionMinz;
        //        }

        //        if (FollowPoint.x >= MousePositionMaxX)
        //        {
        //            FollowPoint.x = MousePositionMaxX;
        //        }

                if (FollowPoint.x >= MousePositionMaxX)
                {
                    FollowPoint.x = MousePositionMaxX;
                }

        //        Vector3 a = Camera.main.WorldToScreenPoint(FollowPoint);
        //        this.transform.position = Camera.main.ScreenToWorldPoint(a);
        //    }
        //}
        //else
        //{
        //    transform.Translate(0, dropSpeed * Time.deltaTime, 0);
        //}

        //#endregion
        ////=================================================
    }



}
