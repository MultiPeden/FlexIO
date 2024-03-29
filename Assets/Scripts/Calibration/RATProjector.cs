﻿using UnityEngine;



namespace FlexIO
{
    //for controlling the rendering loop:
    //http://docs.unity3d.com/Documentation/ScriptReference/Camera.Render.html
    //http://answers.unity3d.com/questions/460596/call-camerarender-twice-rendertexture-contents-dif.html
    //http://docs.unity3d.com/Documentation/ScriptReference/Camera.OnRenderImage.html

    /// <summary>
    /// Behavior that when added to a Unity Camera makes it a projector in the RoomAlive scene
    /// </summary>
    [AddComponentMenu("RoomAliveToolkit/RATProjector")]
    public class RATProjector : MonoBehaviour
    {
        /// <summary>
        /// Asset containing the RoomAlive XML calibration data. 
        /// </summary>
        public RATCalibrationData calibrationData = null;
        /// <summary>
        /// Projector name in the RoomAlive calibration XML file. 
        /// </summary>
        public string nameInConfiguration = "0";

        /// <summary>
        /// A link to the RATProjectionManager
        /// </summary>
     //   public RATProjectionManager projectionManager;

        public Vector4 lensDist;
        public int imageWidth = 1280;
        public int imageHeight = 800;

        public bool initialized { get; private set; }
        internal Camera cam;

        private ProjectorCameraEnsemble.Projector projConfig;
        //EDIT private RATDynamicMask dynamicMask;

        public int displayIndex = -1;


        public void Awake()
        {
            //  cam = this.GetComponent<Camera>();
            //  LoadCalibrationData();

            //  cam.enabled = true;

            initialized = true;
        }

        public void Start()
        {

        }

        public bool hasCalibration
        {
            get
            {
                return calibrationData != null && calibrationData.IsValid();
            }
        }

        internal void LoadCalibrationData()
        {
            cam = this.GetComponent<Camera>();
            projConfig = null;
            if (hasCalibration)
            {
                ProjectorCameraEnsemble ensembleConfig = calibrationData.GetEnsemble();
                foreach (ProjectorCameraEnsemble.Projector pc in ensembleConfig.projectors)
                {
                    if (pc.name == nameInConfiguration)
                    {
                        projConfig = pc;
                    }
                }
            }
            else
            {
                projConfig = null;
            }


            if (projConfig != null)
            {
                if (displayIndex < 0)
                    displayIndex = projConfig.displayIndex;
                //Debug.Log("Projective Rendering - Loading projector calibration information.");
                imageWidth = projConfig.width;
                imageHeight = projConfig.height;

                //// used by shadow etc...
                //// this is the vertical field of view - fy
                cam.aspect = (float)imageWidth / imageHeight;

                cam.fieldOfView = 2.0f * Mathf.Atan(imageHeight * 0.5f / (float)projConfig.cameraMatrix[1, 1]) * Mathf.Rad2Deg;

                Matrix4x4 opencvProjMat = GetProjectionMatrix(projConfig.cameraMatrix, cam.nearClipPlane, cam.farClipPlane);
                cam.projectionMatrix = UnityUtilities.ConvertRHtoLH(opencvProjMat);

                //var irCoef = projConfig.lensDistortion.AsFloatArray();
                //! jolaur -- looks like this is not being used and is now 2 elements instead of four in the new xml format
                //! lensDist = new Vector4(irCoef[0], irCoef[1], irCoef[2], irCoef[3]); 
                lensDist = new Vector4();

                Matrix4x4 worldToLocal = RAT2Unity.Convert(projConfig.pose);
                worldToLocal = UnityUtilities.ConvertRHtoLH(worldToLocal);
                this.transform.localPosition = worldToLocal.ExtractTranslation();
                this.transform.localRotation = worldToLocal.ExtractRotation();
            }
            else
            {
                Debug.Log("Projective Rendering - Using default camera calibration information.");
                lensDist = new Vector4();
            }

        }

        private Matrix4x4 GetProjectionMatrix(FlexIO.Matrix intrinsics, float zNear, float zFar)
        {
            float p_x = (float)intrinsics[0, 2];
            float p_y = (float)intrinsics[1, 2];

            //the intrinsics are in Kinect coordinates: X - left, Y - up, Z, forward
            //we need the coordinates to be: X - right, Y - down, Z - forward
            p_x = imageWidth - p_x;
            p_y = imageHeight - p_y;

            // http://spottrlabs.blogspot.com/2012/07/opencv-and-opengl-not-always-friends.html
            // http://opencv.willowgarage.com/wiki/Posit
            Matrix4x4 projMat = new Matrix4x4();
            projMat[0, 0] = (float)(2.0 * intrinsics[0, 0] / imageWidth);
            projMat[1, 1] = (float)(2.0 * intrinsics[1, 1] / imageHeight);
            projMat[2, 0] = (float)(-1.0f + 2 * p_x / imageWidth);
            projMat[2, 1] = (float)(-1.0f + 2 * p_y / imageHeight);

            // Note this changed from previous code
            // see here: http://www.songho.ca/opengl/gl_projectionmatrix.html
            projMat[2, 2] = -(zFar + zNear) / (zFar - zNear);
            projMat[3, 2] = -2.0f * zNear * zFar / (zFar - zNear);
            projMat[2, 3] = -1;

            // Transpose tp fit Unity's column major matrix (in contrast to vision raw major ones).
            projMat = projMat.transpose;
            return projMat;
        }




    }
}


