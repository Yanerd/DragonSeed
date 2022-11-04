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

        Debug.Log($"{transform.position.x}, {transform.position.y}, {transform.position.z}");

        if (Input.GetKey(KeyCode.W) && transform.position.x < 1 * (5 - DefenseUIManager.INSTANCE.MapState))
        {
            transform.position += Vector3.right * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.A) && transform.position.z < 1 * (5 - DefenseUIManager.INSTANCE.MapState))
        {
            transform.position += Vector3.forward * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S) && transform.position.x > -5)
        {
            transform.position += Vector3.left * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) && transform.position.z > -5)
        {
            transform.position += Vector3.back * Time.deltaTime;
        }
    }


}
