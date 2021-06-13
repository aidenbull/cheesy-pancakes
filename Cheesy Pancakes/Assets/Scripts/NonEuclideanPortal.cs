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
    PortalColliderScript portal1Collider;

    //portal2 objects
    GameObject portal2;
    GameObject camera2Object;
    Camera camera2;
    Vector3 camera2StartCoords;
    GameObject renderPlane2;
    Vector3 renderPlane2StartCoords;
    GameObject backPlane2;
    PortalColliderScript portal2Collider;

    // Start is called before the first frame update
    void Start()
    {
        mainCameraStartCoords = mainCamera.transform.position;

        //portal1 objects
        portal1 = transform.Find("portal1").gameObject;
        camera1Object = transform.Find("Camera1").gameObject;
        backPlane1 = portal1.transform.Find("backPlane").gameObject;
        renderPlane1 = portal1.transform.Find("renderPlane").gameObject;
        renderPlane1StartCoords = renderPlane1.transform.position;
        camera1 = camera1Object.GetComponent<Camera>();
        portal1Collider = portal1.GetComponent<PortalColliderScript>();

        //portal2 objects
        portal2 = transform.Find("portal2").gameObject;
        camera2Object = transform.Find("Camera2").gameObject;
        backPlane2 = portal2.transform.Find("backPlane").gameObject;
        renderPlane2 = portal2.transform.Find("renderPlane").gameObject;
        renderPlane2StartCoords = renderPlane2.transform.position;
        camera2 = camera2Object.GetComponent<Camera>();
        portal2Collider = portal2.GetComponent<PortalColliderScript>();


        //Create render textures for the two render planes
        RenderTexture renderPlane1Texture = new RenderTexture(1048, 1048, 16);
        camera2.targetTexture = renderPlane1Texture;
        renderPlane1.GetComponent<MeshRenderer>().material.mainTexture = renderPlane1Texture;

        RenderTexture renderPlane2Texture = new RenderTexture(1048, 1048, 16);
        camera1.targetTexture = renderPlane2Texture;
        renderPlane2.GetComponent<MeshRenderer>().material.mainTexture = renderPlane2Texture;

        camera1Object.transform.rotation = portal1.transform.rotation;
        camera2Object.transform.rotation = portal2.transform.rotation;
        camera1Object.transform.position = portal1.transform.position + Quaternion.Euler(portal1.transform.eulerAngles - portal2.transform.eulerAngles) * Quaternion.AngleAxis(180f, camera1Object.transform.up) * (mainCameraStartCoords - portal2.transform.position);
        camera2Object.transform.position = portal2.transform.position + Quaternion.Euler(portal2.transform.eulerAngles - portal1.transform.eulerAngles) * Quaternion.AngleAxis(180f, camera2Object.transform.up) * (mainCameraStartCoords - portal1.transform.position);
        //camera1Object.transform.localPosition = (mainCameraStartCoords - portal2.transform.position);
        //camera2Object.transform.localPosition = (mainCameraStartCoords - portal1.transform.position);
        camera1StartCoords = camera1Object.transform.position;
        camera2StartCoords = camera2Object.transform.position;

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
        CheckForTeleports();
        UpdateIllusion1();
        UpdateIllusion2();
    }

    void UpdateIllusion1()
    {
        camera2Object.transform.position = camera2StartCoords + Quaternion.Euler(portal2.transform.eulerAngles - portal1.transform.eulerAngles) * Quaternion.AngleAxis(180f, camera1.transform.up) * (mainCamera.transform.position - mainCameraStartCoords);
        float xDisplacement = Vector3.Dot(Vector3.Project(camera2Object.transform.position - backPlane2.transform.position, camera2Object.transform.right), camera2Object.transform.right);
        float yDisplacement = Vector3.Dot(Vector3.Project(camera2Object.transform.position - backPlane2.transform.position, camera2Object.transform.up), camera2Object.transform.up);
        camera2.lensShift = new Vector2(-xDisplacement / 2 / (viewPlaneWidth / 2), -yDisplacement / 2 / (viewPlaneWidth / 2));
        float perpDistance = Vector3.Dot(Vector3.Project((backPlane2.transform.position - camera2Object.transform.position), backPlane2.transform.up), backPlane2.transform.up);
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
        float cameraIllusionPerpDist = Vector3.Dot(Vector3.Project((renderPlane1StartCoords - mainCamera.transform.position), -renderPlane1.transform.up), -renderPlane1.transform.up);
        if (cameraIllusionDist < viewPlaneWidth && cameraIllusionPerpDist < minIllusionDistance && cameraIllusionPerpDist > -(portalLength/2)-0.1f)
        {
            float currentIllusionOffset = minIllusionDistance - cameraIllusionPerpDist;
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
        camera1Object.transform.position = camera1StartCoords + Quaternion.Euler(portal1.transform.eulerAngles - portal2.transform.eulerAngles) * Quaternion.AngleAxis(180f, camera1.transform.up) * (mainCamera.transform.position - mainCameraStartCoords);
        float xDisplacement = Vector3.Dot(Vector3.Project(camera1Object.transform.position - backPlane1.transform.position, camera1Object.transform.right), camera1Object.transform.right);
        float yDisplacement = Vector3.Dot(Vector3.Project(camera1Object.transform.position - backPlane1.transform.position, camera1Object.transform.up), camera1Object.transform.up);
        camera1.lensShift = new Vector2(-xDisplacement / 2 / (viewPlaneWidth / 2), -yDisplacement / 2 / (viewPlaneWidth / 2));
        float perpDistance = Vector3.Dot(Vector3.Project((backPlane1.transform.position - camera1Object.transform.position), backPlane1.transform.up), backPlane1.transform.up);
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
        float cameraIllusionPerpDist = Vector3.Dot(Vector3.Project((renderPlane2StartCoords - mainCamera.transform.position), -renderPlane2.transform.up), -renderPlane2.transform.up);
        if (cameraIllusionDist < viewPlaneWidth && cameraIllusionPerpDist < minIllusionDistance && cameraIllusionPerpDist > -(portalLength / 2) - 0.1f)
        {
            float currentIllusionOffset = minIllusionDistance - cameraIllusionPerpDist;
            renderPlane2.transform.position = renderPlane2StartCoords + -renderPlane2.transform.up * currentIllusionOffset;
        }
        else
        {
            renderPlane2.transform.position = renderPlane2StartCoords;
        }
    }

    bool inPortal1 = false;
    bool inPortal2 = false;
    void CheckForTeleports()
    {
        Check1to2Teleport();
        Check2to1Teleport();
    }

    //Hardcoding this just for the purpose of the jam
    public GameObject teleportingObject;
    void Check1to2Teleport()
    {
        if (portal1Collider.IsTouchingPlayer)
        {
            if (Vector3.Dot(teleportingObject.transform.position - portal1.transform.position, portal1.transform.forward) < -0.1)
            {
                teleportingObject.transform.position = portal2.transform.position + Quaternion.Euler(portal2.transform.eulerAngles - portal1.transform.eulerAngles) * Quaternion.AngleAxis(180f, portal2.transform.up) * (teleportingObject.transform.position - portal1.transform.position);
                teleportingObject.transform.rotation = Quaternion.Euler(teleportingObject.transform.rotation.eulerAngles + (portal2.transform.eulerAngles - portal1.transform.eulerAngles) + new Vector3(0f, 180f, 0));
                //mainCamera.transform.rotation *= Quaternion.AngleAxis(180f, portal2.transform.up);
            }
        }
    }

    void Check2to1Teleport()
    {
        if (portal2Collider.IsTouchingPlayer)
        {
            if (Vector3.Dot(teleportingObject.transform.position - portal2.transform.position, portal2.transform.forward) < -0.1)
            {
                teleportingObject.transform.position = portal1.transform.position + Quaternion.Euler(portal1.transform.eulerAngles - portal2.transform.eulerAngles) * Quaternion.AngleAxis(180f, portal2.transform.up) * (teleportingObject.transform.position - portal2.transform.position);
                teleportingObject.transform.rotation = Quaternion.Euler(teleportingObject.transform.rotation.eulerAngles + (portal1.transform.eulerAngles - portal2.transform.eulerAngles) + new Vector3(0f, 180f, 0));
                //mainCamera.transform.rotation *= Quaternion.AngleAxis(180f, portal2.transform.up);
            }
        }
    }
}
