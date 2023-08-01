using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform targetMesh;
    public Vector3 offset;
    [SerializeField]
    private float rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float hor = Input.GetAxis("Mouse X") * rotateSpeed;
        targetMesh.Rotate(0f, hor, 0f);
    }
}
