﻿using UnityEngine;
using System.Collections;


namespace RoomAliveToolkit
{
    public enum ViewDebugMode
    {
        None, RGB//, Depth
    }
    [AddComponentMenu("RoomAliveToolkit/RATUserViewCamera")]
    /// <summary>
    /// Unity Camera for rendering the user's view in view-dependent projection mapping scenarios.
    /// 
    /// The logic behind the RoomAlive User Views explained:
    /// 
    /// Let's assume that there are at least 4 different layers in the scene that will control what objects are
    /// visible from which camera (user or projector)
    /// 
    /// Create 4 layers in your scene:
    /// - VirtualTextures - for virtual objects that should be texture mapped onto existing surfaces
    /// - Virtual3DObjects - for virtual 3D objects that could be perspective mapped
    /// - StaticSurfaces - for existing static room geometry that is loaded from a file(obj mesh for example)
    /// - DynamicSurfaces- for dynamic depth meshes that represent the physical space
    /// 
    /// In RATProjectionManager set Texture Layers to be "VirtualTextures". These are view independent and therefore do not need to be rendered in user's views. 
    /// 
    /// In this component, select Virtual3DObjects as VirtualObjectMask (layer mask). This will render only the virtual (view-dependent) 3D objects for each user. 
    /// 
    /// However, it is also important to account for real world geometry to correctly occlude the virtual objects. To accomplish that, use RATProjectionPass components and set them up for each
    /// type of real world objects in the scene. The most common situations are for static (OBJ meshes captured during scene calibration) and dynamic objects (Kinect depth meshes)
    /// 
    /// To each RATUserViewCamera add a component RATProjectionPass for each physical layer that you want to projecto on:
    /// select: StaticSurfaces For TargetSurfaceLayer 
    /// Press on "Set Static Defaults" button
    ///
    /// select: DynamicSurfaces For TargetSurfaceLayer
    /// Press on "Set Dynamic Defaults" button
    /// 
    /// </summary>
    public class RATUserViewCamera : MonoBehaviour
    {
        public RATProjectionManager projectionManager;

        [ReadOnly]
        public RenderTexture targetRGBTexture;


        [Space(10)]

        [Space(10)]
        public float fieldOfView = 120;
        public float nearClippingPlane = 0.1f;
        public float farClippingPlane = 8f;
        public LayerMask virtualObjectsMask; //select only the layers you want to see in the user's view

        [Space(10)]
        public ViewDebugMode debugPlane = ViewDebugMode.RGB;
        /// <summary>
        /// the size of the debug view plane visible in the scene view
        /// </summary>
        [Range(0.1f, 3)]
        public float debugPlaneSize = 0.1f;

        [Space(10)]
        public Color backgroundColor = new Color(0, 0, 0, 0);
        public Color realSurfaceColor = new Color(0, 0, 0, 0);

        public RATProjectionPass[] projectionLayers;

        [Space(10)]
        public float separation = 0.02169f; //Distance between cameras
        public float convergence = 0.011f; // Use convergence to move close objects in/out of the screen
        public Matrix4x4 originalProjection1; // needed for convergence
        public Matrix4x4 p1;
        public Matrix4x4 originalProjection2;
        public Matrix4x4 p2;


        public Camera viewCamera
        {
            get { return camMONO; }
        }

        protected int texWidth = 2048; //width of the off-screen render texture for this user view (needs to be power of 2)
        protected int texHeight = 2048;//height of the off-screen render texture for this user view (needs to be power of 2)

        protected Mesh debugPlaneM;
        protected MeshFilter meshFilter;
        protected MeshRenderer meshRenderer;
        protected Material meshMat;

        protected int[] indices = new int[] { 0, 1, 2, 3, 2, 1 };
        protected Vector2[] uv = new Vector2[] { new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(1, 0) };
        protected Vector3[] pos = new Vector3[4];

        protected bool initialized = false;
        protected GameObject cameraGO;
        protected Camera cam1;
        protected Camera cam2;
        protected Camera camMONO;
        protected Rect rectReadRT;
        protected RATDepthMesh[] depthMeshes;
        protected Vector3 cam1Pos;
        protected Vector3 cam2Pos;

        private RATUser ratUser;
        private bool toggleCam = true;
        private float KeyInputDelayTimer; // Keyboard input delay... quick&dirty
        private bool resetCam = false;
        private bool isOn3D = false;

        private float timeBefore = 0f;
        private float timeTest = 0f;
        private float timeDiff, timeTestDiff;
        private Vector3 leftEye, rightEye, head;

        private int fixedUpdateCount = 0, lateUpdateCount = 0, renderCount = 0;

        public bool hasManager
        {
            get
            {
                return projectionManager != null && projectionManager.isActiveAndEnabled;
            }
        }

        void Awake()
        {
            QualitySettings.vSyncCount = 1;
            projectionLayers = gameObject.GetComponents<RATProjectionPass>();

            foreach (RATProjectionPass layer in projectionLayers)
                layer.Init();
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            Shader unlitShader = Shader.Find("Unlit/Texture");
            meshMat = new Material(unlitShader);
            debugPlaneM = new Mesh();
            meshFilter.hideFlags = HideFlags.HideInInspector;
            meshRenderer.hideFlags = HideFlags.HideInInspector;
            meshMat.hideFlags = HideFlags.HideInInspector;

            //Cam1
            cam1 = this.GetComponent<Camera>();
            if (cam1 == null)
                cam1 = gameObject.AddComponent<Camera>();
            cam1.hideFlags = HideFlags.HideInInspector;  // | HideFlags.HideInHierarchy

            cam1.rect = new Rect(0, 0, 1, 1);
            cam1.enabled = false; //important to disable this camera as we will be calling Render() directly. 
            cam1.aspect = texWidth / texHeight;

            if (!resetCam)
            {
                resetCam = true;
                cam1.ResetProjectionMatrix();
            }
            cam1.nearClipPlane = nearClippingPlane;
            cam1.farClipPlane = farClippingPlane;
            cam1.fieldOfView = fieldOfView;

            originalProjection1 = cam1.projectionMatrix;
            p1 = p2 = originalProjection1;

            //Right
            p1.m02 = convergence;


            //Left
            p2.m02 = convergence * -1;

        }

        void Start()
        {
            if (projectionManager == null)
                projectionManager = GameObject.FindObjectOfType<RATProjectionManager>();
            if (projectionManager != null)
                projectionManager.RegisterUser(this);

            ratUser = GetComponent<RATUser>();
            //Code assumes that this script is added to the camera GO 
            cameraGO = this.gameObject;



            cameraGO.transform.localPosition = new Vector3();

            targetRGBTexture = new RenderTexture(texWidth, texHeight, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
            targetRGBTexture.filterMode = FilterMode.Trilinear;
            targetRGBTexture.autoGenerateMips = true;
            targetRGBTexture.depth = 24;
            targetRGBTexture.Create();

            rectReadRT = new Rect(0, 0, texWidth, texHeight);

            depthMeshes = GameObject.FindObjectsOfType<RATDepthMesh>();

            initialized = true;
            separation = ReadSeparation();
        }

        public void FixedUpdate()
        {
            //fixedUpdateCount++;
            //Debug.Log("FixedUpdate: " + fixedUpdateCount);
            // this mostly updates the little debug view in the scene editor view
            if (debugPlaneSize < 0)
                debugPlaneSize = 0;

            KeyInputs();



            //Cam1
            cam1.nearClipPlane = nearClippingPlane;
            cam1.farClipPlane = farClippingPlane;
            cam1.fieldOfView = fieldOfView;

            meshRenderer.enabled = debugPlane != ViewDebugMode.None;
            if (meshRenderer.enabled)
            {
                //meshMat.mainTexture = debugPlane == ViewDebugMode.RGB?targetRGBTexture:targetDepthTexture;
                meshMat.mainTexture = targetRGBTexture;
                meshRenderer.sharedMaterial = meshMat;

                float z = debugPlaneSize <= nearClippingPlane ? nearClippingPlane : debugPlaneSize;
                float fac = Mathf.Tan(fieldOfView / 2 / 180f * Mathf.PI);
                float w = z * fac;
                float h = z * fac;
                pos[0] = new Vector3(-w, h, nearClippingPlane);
                pos[1] = new Vector3(w, h, nearClippingPlane);
                pos[2] = new Vector3(-w, -h, nearClippingPlane);
                pos[3] = new Vector3(w, -h, nearClippingPlane);
                debugPlaneM.vertices = pos;
                debugPlaneM.uv = uv;
                debugPlaneM.triangles = indices;
                meshFilter.mesh = debugPlaneM;
            }
            
            //StartCoroutine(LateFixedUpdate());
        }

        /*IEnumerator LateFixedUpdate()
        {
            if (!initialized) yield break;
            yield return new WaitForFixedUpdate();
            lateUpdateCount++;
            Debug.Log("LateUpdate: " + lateUpdateCount);
            if (timeDiff > 0.02)
            {
                Debug.Log("GREATER at" + Time.realtimeSinceStartup + "seconds. Diff: " + timeDiff);
                toggleCam = !toggleCam;
            }
            RenderUserView();
        }*/
        public void LateUpdate()
        {
            /*timeDiff = Time.realtimeSinceStartup - timeBefore;
            timeBefore = Time.realtimeSinceStartup;
            

            
                Debug.Log("TimeDiff: " + timeDiff + " at " + Time.realtimeSinceStartup);*/
                //toggleCam = !toggleCam;
            

            //lateUpdateCount++;
            //Debug.Log("LateUpdate: " + lateUpdateCount);
            if (!initialized)
                return;

            RenderUserView();

            // Projection mapping rendering is actually done by each of the projector cameras
            // Setup things for the last pass which will be rendered from the perspective of the projectors (i.e., Render Pass 3)
            // this "pass" doesn't  do any rendering at this point, but merely sets the correct shaders/materials on all 
            // physical objects in the scene. 
        }


        /// <summary>
        /// Render both virtual and physical objects together from the perspective of the user
        /// </summary>
        public void RenderUserView()
        {
            //timeTest = Time.realtimeSinceStartup;
            //renderCount++;
            //Debug.Log("Render: " + renderCount);
            head = ratUser.getHeadPosition();
            cam1Pos = cam1.transform.localPosition;
            leftEye = new Vector3(head.x + separation, head.y, head.z);
            rightEye = new Vector3(head.x - separation, head.y, head.z);

            if (isOn3D)
            {
                if (toggleCam) //Cam1
                {
                    toggleCam = !toggleCam;
                    cam1.transform.localPosition = leftEye;
                    cam1.projectionMatrix = p1;

                }
                else //Cam2
                {
                    toggleCam = !toggleCam;
                    cam1.transform.localPosition = rightEye;
                    cam1.projectionMatrix = p2;
                }
            }

            cam1.cullingMask = virtualObjectsMask;
            cam1.backgroundColor = backgroundColor;
            cam1.targetTexture = targetRGBTexture;
            cam1.clearFlags = CameraClearFlags.SolidColor;
            cam1.Render();
            cam1.clearFlags = CameraClearFlags.Nothing;

            //Debug.Log("Left: " + ratUser.getLeftEyePos() + "Right: " + ratUser.getRightEyePos());

            foreach (RATProjectionPass layer in projectionLayers)
            {
                if (layer.renderUserView && layer.userViewShader != null && layer.enabled)
                {
                    cam1.cullingMask = layer.targetSurfaceLayers;
                    Shader.SetGlobalColor("_ReplacementColor", realSurfaceColor);

                    cam1.RenderWithShader(layer.userViewShader, null);
                }
            }
            cam1.clearFlags = CameraClearFlags.SolidColor;
            timeTestDiff = Time.realtimeSinceStartup - timeTest;
            //Debug.Log("Render Duration: "+ timeTestDiff);
        }

        public virtual void RenderProjection(Camera camera)
        {
            RATProjectionPass[] layers = projectionLayers;
            for (int layerId = 0; layerId < layers.Length; layerId++)
            {
                RATProjectionPass layer = layers[layerId];
                if (layer == null || !layer.enabled || layer.projectionShader == null || !layer.renderProjectionPass)
                    continue;
                camera.cullingMask = layer.targetSurfaceLayers;

                //todo preload IDs
                Shader.SetGlobalVector("_UserViewPos", this.cam1.transform.position);
                Shader.SetGlobalTexture("_UserViewPointRGB", targetRGBTexture);
                //Shader.SetGlobalTexture("_UserViewPointDepth", targetDepthTexture);
                Shader.SetGlobalMatrix("_UserVP", this.cam1.projectionMatrix * this.cam1.worldToCameraMatrix);
                camera.RenderWithShader(layer.projectionShader, null);
            }
        }

        private void KeyInputs()
        {

            //3D On
            if (Input.GetKey(KeyCode.Tab) && KeyInputDelayTimer + 0.1f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                isOn3D = !isOn3D;
            }

            //Toggle Eyes
            if (Input.GetKey(KeyCode.F1) && KeyInputDelayTimer + 0.1f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                toggleCam = !toggleCam;
            }

            // Change Separation
            if (Input.GetKey(KeyCode.F2) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                separation = separation - 0.0001f;
                if (separation < 0.0f) separation = 0.0f;
            }

            if (Input.GetKey(KeyCode.F3) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                separation = separation + 0.0001f;
                if (separation > 1.0f) separation = 1.0f;
            }

            // Change Convergence
            if (Input.GetKey(KeyCode.F4) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                convergence = convergence + 0.0001f;
                p1 = originalProjection1;
                p1.m02 = convergence;
                p2 = originalProjection1;
                p2.m02 = convergence * -1;
            }

            if (Input.GetKey(KeyCode.F5) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                convergence = convergence - 0.0001f;
                p1 = originalProjection1;
                p1.m02 = convergence;
                p2 = originalProjection1;
                p2.m02 = convergence * -1;
            }

            // Change Field of View

            if (Input.GetKey(KeyCode.F6) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                fieldOfView = fieldOfView - 0.1f;
                cam1.ResetProjectionMatrix();
                cam1.fieldOfView = fieldOfView;
                originalProjection1 = cam1.projectionMatrix;
                p1 = originalProjection1;
                p1.m02 = convergence;
                p2 = originalProjection1;
                p2.m02 = convergence * -1;
            }

            if (Input.GetKey(KeyCode.F7) && KeyInputDelayTimer + 0.02f < Time.time)
            {
                KeyInputDelayTimer = Time.time;
                fieldOfView = fieldOfView + 0.1f;
                cam1.ResetProjectionMatrix();
                cam1.fieldOfView = fieldOfView;
                originalProjection1 = cam1.projectionMatrix;
                p1 = originalProjection1;
                p1.m02 = convergence;
                p2 = originalProjection1;
                p2.m02 = convergence * -1;
            }
        }

        private float ReadSeparation()
        {
            string text = System.IO.File.ReadAllText("EyeSeparation.txt");
            
            return float.Parse(text);
        }
    }


}
