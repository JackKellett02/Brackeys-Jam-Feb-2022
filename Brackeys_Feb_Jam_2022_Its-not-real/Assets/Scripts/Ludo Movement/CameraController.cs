using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform playerObject;
    [SerializeField]
    private float followSpeed;
    [SerializeField]
    private bool clampBool = true;
    [SerializeField]
    private BoxCollider2D currentRoom;

    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            followSpeed -= 0.1f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E))
        {
            followSpeed += 0.1f * Time.deltaTime;
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            clampBool = !clampBool;
        }
    }

    private void FixedUpdate()
    {
        //Keeping player target within constraints of room, with the size of the camera kept in mind.
        //May not function perfectly if camera bounds are bigger than room.
        float orthoSize = Camera.main.orthographicSize;
        targetPosition = playerObject.position;
        Bounds roomBounds = currentRoom.bounds;

        //Upper and lower bounds checks
        if (targetPosition.y - orthoSize < roomBounds.center.y - roomBounds.extents.y)
        {
            targetPosition = new Vector3(targetPosition.x, roomBounds.center.y - roomBounds.extents.y + orthoSize, -10.0f);
        }
        else if(targetPosition.y + orthoSize > roomBounds.center.y + roomBounds.extents.y)
        {
            targetPosition = new Vector3(targetPosition.x, roomBounds.center.y + roomBounds.extents.y - orthoSize, -10.0f);
        }

        //Horizontal bounds checks
        if (targetPosition.x - orthoSize * Screen.width / Screen.height < roomBounds.center.x - roomBounds.extents.x)
        {
            targetPosition = new Vector3(roomBounds.center.x - roomBounds.extents.x + orthoSize * Screen.width / Screen.height, targetPosition.y, -10.0f);
        }
        else if (targetPosition.x + orthoSize * Screen.width / Screen.height > roomBounds.center.x + roomBounds.extents.x)
        {
            targetPosition = new Vector3(roomBounds.center.x + roomBounds.extents.x - orthoSize * Screen.width / Screen.height, targetPosition.y, -10.0f);
        }

        //Camera follows Player position smoothly with a lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.timeScale);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10.0f);
    }

    public void SwitchRooms(Collider2D newRoom)
    {
        currentRoom = newRoom.GetComponent<BoxCollider2D>();
    }
}
