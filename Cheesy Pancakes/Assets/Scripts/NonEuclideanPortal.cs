using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Don't look at this code if you're squeamish. Many shortcuts have been taken to reduce time writing. 
 // As such the following code breaks many principles of writing clean and reusable code. You've been advised
public class NonEuclideanPortal : MonoBehaviour
{
    public Camera mainCamera;
    Vector3 mainCameraStartCoords;

    public float viewPlaneWidth = 1.6f;
    public float portalLength = 1.0f;
    public float minIllusionDistance = 0.2f;

    //portal1 objects
    GameObject portal1;
    GameObject camera1Object;
    Camera camera1;
    Vector3 camera1StartCoords;
    GameObject renderPlane1;
    Vector3 renderPlane1StartCoords;
    GameObject backPlane1;

    //portal2 objects
    GameObject portal2;
    GameObject camera2Object;
    Camera camera2;
    Vector3 camera2StartCoords;
    GameObject renderPlane2;
    Vector3 renderPlane2StartCoords;
    GameObject backPlane2;

    // Start is called before the first frame update
    void Start()
    {
        //portal1 objects
        portal1 = transform.Find("portal1").gameObject;
        camera1Object = transform.Find("Camera1").gameObject;
        backPlane1 = portal1.transform.Find("backPlane").gameObject;
        renderPlane1 = portal1.transform.Find("renderPlane").gameObject;
        renderPlane1StartCoords = renderPlane1.transform.position;
        camera1 = camera1Object.GetComponent<Camera>();

        mainCameraStartCoords = mainCamera.transform.position;

        //portal2 objects
        portal2 = transform.Find("portal2").gameObject;
        camera2Object = transform.Find("Camera2").gameObject;
        backPlane2 = portal2.transform.Find("backPlane").gameObject;
        renderPlane2 = portal2.transform.Find("renderPlane").gameObject;
        renderPlane2StartCoords = renderPlane2.transform.position;
        camera2 = camera2Object.GetComponent<Camera>();


        //Create render textures for the two render planes
        RenderTexture renderPlane1Texture = new RenderTexture(1048, 1048, 16);
        camera2.targetTexture = renderPlane1Texture;
        renderPlane1.GetComponent<MeshRenderer>().material.mainTexture = renderPlane1Texture;

        RenderTexture renderPlane2Texture = new RenderTexture(1048, 1048, 16);
        camera1.targetTexture = renderPlane2Texture;
        renderPlane2.GetComponent<MeshRenderer>().material.mainTexture = renderPlane2Texture;

        camera1Object.transform.position = portal1.transform.position + Quaternion.Euler(portal2.transform.eulerAngles - portal1.transform.eulerAngles) * (mainCameraStartCoords - portal2.transform.position);
        camera2Object.transform.position = portal2.transform.position + Quaternion.Euler(portal1.transform.eulerAngles - portal2.transform.eulerAngles) * (mainCameraStartCoords - portal1.transform.position);
        //camera1Object.transform.localPosition = (mainCameraStartCoords - portal2.transform.position);
        //camera2Object.transform.localPosition = (mainCameraStartCoords - portal1.transform.position);
        camera1Object.transform.rotation = portal1.transform.rotation;
        camera2Object.transform.rotation = portal2.transform.rotation;
        camera1StartCoords = camera1Object.transform.localPosition;
        camera2StartCoords = camera2Object.transform.localPosition;

        //Set correct starting offsets for illusion cameras
        Debug.Log(mainCameraStartCoords);
        Debug.Log(camera1StartCoords);
        Debug.Log(camera2StartCoords);
        Debug.Log(portal1.transform.position);
        Debug.Log(portal2.transform.position);
        Debug.Log(Quaternion.Euler(portal1.transform.localEulerAngles));
        Debug.Log(Quaternion.Euler(portal2.transform.localEulerAngles));
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateIllusion1();
        UpdateIllusion2();
    }

    void UpdateIllusion1()
    {
        camera2Object.transform.localPosition = camera2StartCoords + Quaternion.Euler(portal1.transform.eulerAngles - portal2.transform.eulerAngles) * (mainCamera.transform.position - mainCameraStartCoords);
        float xDisplacement = Vector3.Dot(Vector3.Project(camera2Object.transform.position - backPlane2.transform.position, camera2Object.transform.right), camera2Object.transform.right);
        float yDisplacement = Vector3.Dot(Vector3.Project(camera2Object.transform.position - backPlane2.transform.position, camera2Object.transform.up), camera2Object.transform.up);
        camera2.lensShift = new Vector2(-xDisplacement / 2 / (viewPlaneWidth / 2), -yDisplacement / 2 / (viewPlaneWidth / 2));
        float perpDistance = Vector3.Project((backPlane2.transform.position - camera2Object.transform.position), backPlane2.transform.up).magnitude;
        if (perpDistance > minIllusionDistance)
        {
            float fov = 2 * Mathf.Atan((viewPlaneWidth / 2) / (perpDistance)) * Mathf.Rad2Deg;
            camera2.fieldOfView = fov;
            camera2.nearClipPlane = perpDistance;
        }
        else
        {
            float fov = 2 * Mathf.Atan((viewPlaneWidth / 2) / (minIllusionDistance)) * Mathf.Rad2Deg;
            camera2.fieldOfView = fov;
            camera2.nearClipPlane = minIllusionDistance;
        }


        float cameraIllusionDist = (mainCamera.transform.position - renderPlane1.transform.position).magnitude;
        float cameraIllusionPerpDist = Vector3.Project((renderPlane1StartCoords - mainCamera.transform.position), -renderPlane1.transform.up).magnitude;
        if (cameraIllusionDist < viewPlaneWidth && cameraIllusionPerpDist < minIllusionDistance && cameraIllusionPerpDist > -(portalLength/2)-0.1f)
        {
            float currentIllusionOffset = mainCamera.transform.position.z - renderPlane1StartCoords.z + minIllusionDistance;
            renderPlane1.transform.position = renderPlane1StartCoords + -renderPlane1.transform.up * currentIllusionOffset;
        }
        else
        {
            renderPlane1.transform.position = renderPlane1StartCoords;
        }
    }

    //One of the ugliest Ctrl-c Ctrl-v's of my life
    void UpdateIllusion2()
    {
        camera1Object.transform.localPosition = camera1StartCoords + Quaternion.Euler(portal2.transform.eulerAngles - portal1.transform.eulerAngles) * (mainCamera.transform.position - mainCameraStartCoords);
        float xDisplacement = Vector3.Dot(Vector3.Project(camera1Object.transform.position - backPlane1.transform.position, camera1Object.transform.right), camera1Object.transform.right);
        float yDisplacement = Vector3.Dot(Vector3.Project(camera1Object.transform.position - backPlane1.transform.position, camera1Object.transform.up), camera1Object.transform.up);
        camera1.lensShift = new Vector2(-xDisplacement / 2 / (viewPlaneWidth / 2), -yDisplacement / 2 / (viewPlaneWidth / 2));
        float perpDistance = Vector3.Project((backPlane1.transform.position - camera1Object.transform.position), backPlane1.transform.up).magnitude;
        if (perpDistance > minIllusionDistance)
        {
            float fov = 2 * Mathf.Atan((viewPlaneWidth / 2) / (perpDistance)) * Mathf.Rad2Deg;
            camera1.fieldOfView = fov;
            camera1.nearClipPlane = perpDistance;
        }
        else
        {
            float fov = 2 * Mathf.Atan((viewPlaneWidth / 2) / (minIllusionDistance)) * Mathf.Rad2Deg;
            camera1.fieldOfView = fov;
            camera1.nearClipPlane = minIllusionDistance;
        }


        float cameraIllusionDist = (mainCamera.transform.position - renderPlane2.transform.position).magnitude;
        float cameraIllusionPerpDist = Vector3.Project((renderPlane2StartCoords - mainCamera.transform.position), -renderPlane1.transform.up).magnitude;
        if (cameraIllusionDist < viewPlaneWidth && cameraIllusionPerpDist < minIllusionDistance && cameraIllusionPerpDist > -(portalLength / 2) - 0.1f)
        {
            float currentIllusionOffset = mainCamera.transform.position.z - renderPlane2StartCoords.z + minIllusionDistance;
            renderPlane2.transform.position = renderPlane2StartCoords + -renderPlane2.transform.up * currentIllusionOffset;
        }
        else
        {
            renderPlane2.transform.position = renderPlane2StartCoords;
        }
    }
}