using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{  
    private struct PointInSpace
    {
        public Vector3 Position;
        public float Time ;
    }
     
    [SerializeField]
    [Tooltip("The transform to follow")]
    private Rigidbody2D target;
      
    [SerializeField]
    [Tooltip("The offset between the target and the camera")]
    private Vector3 offset;
     
    [Tooltip("The delay before the camera starts to follow the target")]
    [SerializeField]
    private float delay = 0.5f;
      
    [SerializeField]
    [Tooltip("The speed used in the lerp function when the camera follows the target")]
    private float speed = 5;
      
    ///<summary>
    /// Contains the positions of the target for the last X seconds
    ///</summary>
    private Queue<PointInSpace> pointsInSpace = new Queue<PointInSpace>();
  
    void FixedUpdate() 
    {
        // Add the current target position to the list of positions
        pointsInSpace.Enqueue( new PointInSpace() { Position = new Vector3(target.position.x, target.position.y, transform.position.z), Time = Time.time } ) ;
         
        // Move the camera to the position of the target X seconds ago 
        while( pointsInSpace.Count > 0 && pointsInSpace.Peek().Time <= Time.time - delay + Mathf.Epsilon )
        {
            transform.position = Vector3.Lerp( transform.position, pointsInSpace.Dequeue().Position + offset, Time.deltaTime * speed);
        }
    }
    
    public string GetClickedObject()
    {
        string objectClicked = "none";
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100))
            {
                Debug.Log(hit.transform.gameObject.tag);
                objectClicked = hit.transform.gameObject.tag;
            }
        }

        return objectClicked;
    }
}
