using UnityEngine;

public class CameraMoveScroll : MonoBehaviour
{
    Transform transform = null;

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    
    public void Update()
    {
        Move();
    }

    public void Move()
    {
        if (GameManager.INSTANCE.WANTINVASION)
            return;

        float maxLimit = 3 + (5 - DefenseUIManager.INSTANCE.MapState);
        float minLimit = -1f;

        if (Input.GetKey(KeyCode.W) && (transform.position.x < maxLimit || transform.position.z < maxLimit))
        {
            if (transform.position.z > maxLimit)
            {
                transform.position += Vector3.right * Time.deltaTime;
            }
            else if (transform.position.x > maxLimit)
            {
                transform.position += Vector3.forward * Time.deltaTime;
            }
            else
            { 
                transform.position += (Vector3.right + Vector3.forward).normalized * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.A) && (transform.position.z < maxLimit || transform.position.x > minLimit))
        {
            if (transform.position.z > maxLimit)
            {
                transform.position += Vector3.left * Time.deltaTime;
            }
            else if (transform.position.x < minLimit)
            {
                transform.position += Vector3.forward * Time.deltaTime;
            }
            else
            {
                transform.position += (Vector3.forward + Vector3.left).normalized * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.S) && (transform.position.z > minLimit || transform.position.x > minLimit))
        {
            if (transform.position.z < minLimit)
            {
                transform.position += Vector3.left * Time.deltaTime;
            }
            else if (transform.position.x < minLimit)
            {
                transform.position += Vector3.back * Time.deltaTime;
            }
            else
            {
                transform.position += (Vector3.left + Vector3.back).normalized * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.D) && (transform.position.z > minLimit || transform.position.x < maxLimit))
        {
            if (transform.position.z < minLimit)
            {
                transform.position += Vector3.right * Time.deltaTime;
            }
            else if (transform.position.x > maxLimit)
            {
                transform.position += Vector3.back * Time.deltaTime;
            }
            else
            {
                transform.position += (Vector3.back + Vector3.right).normalized * Time.deltaTime;
            }
        }
    }


}
