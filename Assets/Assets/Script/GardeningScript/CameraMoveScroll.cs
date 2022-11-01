using UnityEngine;

public class CameraMoveScroll : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public void Update()
    {
        Wheel();
        Move();
    }

    void Wheel()
    {
        float scroollWheel = Input.GetAxis("Mouse ScrollWheel") * -1;
        Camera.main.orthographicSize += scroollWheel * 5f;
        if (Camera.main.orthographicSize >= 4.5)
        {
            Camera.main.orthographicSize = 4.5f;
        }
        else if (Camera.main.orthographicSize <= 1)
        {
            Camera.main.orthographicSize = 1.0f;
        }
    }

    public void Move()
    {
        if (Camera.main.orthographicSize == 1)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 5f && transform.position.z >= -5f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -5f && transform.position.z <= 5f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 5f && transform.position.z <= 5f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -5f && transform.position.z >= -5f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 1.5f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 4f && transform.position.z >= -4f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -4f && transform.position.z <= 4f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 4f && transform.position.z <= 4f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -4f && transform.position.z >= -4f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 2f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 3f && transform.position.z >= -3f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -3f && transform.position.z <= 3f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 3f && transform.position.z <= 3f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -3f && transform.position.z >= -3f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 2.5f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 2.5f && transform.position.z >= -2.5f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -2.5f && transform.position.z <= 2.5f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 2.5f && transform.position.z <= 2.5f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -2.5f && transform.position.z >= -2.5f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 3f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 2f && transform.position.z >= -2f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -2f && transform.position.z <= 2f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 2f && transform.position.z <= 2f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -2f && transform.position.z >= -2f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 3.5f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 1.5f && transform.position.z >= -1.5f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -1.5f && transform.position.z <= 1.5f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 1.5f && transform.position.z <= 1.5f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -1.5f && transform.position.z >= -1.5f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 4)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 1f && transform.position.z >= -1f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -1f && transform.position.z <= 1f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 1f && transform.position.z <= 1f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -1f && transform.position.z >= -1f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }

        if (Camera.main.orthographicSize == 4.5f)
        {
            if (Input.GetKey(KeyCode.RightArrow) && transform.position.x <= 0.5f && transform.position.z >= -0.5f)
            {
                transform.Translate(0.02f, 0, -0.02f);
            }

            if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x >= -0.5f && transform.position.z <= 0.5f)
            {
                transform.Translate(-0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.UpArrow) && transform.position.x <= 0.5f && transform.position.z <= 0.5f)
            {
                transform.Translate(0.02f, 0, 0.02f);
            }

            if (Input.GetKey(KeyCode.DownArrow) && transform.position.x >= -0.5f && transform.position.z >= -0.5f)
            {
                transform.Translate(-0.02f, 0, -0.02f);
            }
        }
    }


}
