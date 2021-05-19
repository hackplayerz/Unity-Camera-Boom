using UnityEngine;

[ExecuteInEditMode]
 public class OrbitCameraWithBoom : MonoBehaviour
 {
     [Header("Set view target.")]
     [SerializeField] 
     private Transform viewTarget;
     [Header("Distance between target to camera.")]
     [SerializeField,Range(0,15f)] 
     private float distance = 5.0f;
     [SerializeField] 
     private float distanceMin = .5f;
     [SerializeField] 
     private float distanceMax = 15f;
     
     [Header("Mouse setting.")]
     [SerializeField,Range(0,10)] 
     private float mouseSpeed = 5f;
     
     [Header("Angle lock.")]
     [SerializeField,Range(-90,0)] 
     private float yawMin = -20f;
     [SerializeField,Range(0,90)] 
     private float yawMax = 80f;
     [SerializeField]
     private Vector2 cameraDamp= new Vector2(0,0.6f);
     
     private float _pitch = 0.0f;
     private float _yaw = 0.0f;

     private void OnDrawGizmos()
     {
         var position = transform.position;
         var targetPosition = viewTarget.position + (Vector3)cameraDamp;
         Gizmos.color = Color.cyan;
         Gizmos.DrawSphere(position,0.1f);
         Gizmos.DrawSphere(targetPosition,0.1f);
         Gizmos.color = Color.red;
         Gizmos.DrawLine(position,targetPosition);
     }

     private void Start()
     {
         Vector3 angles = transform.eulerAngles;
         _pitch = angles.y;
         _yaw = angles.x;
     }

     private void LateUpdate()
     {
         var myTransform = transform;
         var myPosition = myTransform.position;
         var targetPosition = viewTarget.position + (Vector3)cameraDamp;

         // Get mouse input value.
         _pitch += Input.GetAxis("Mouse X") * mouseSpeed * distance;
         _yaw -= Input.GetAxis("Mouse Y") * mouseSpeed;
         // Clamping y value. 
         _yaw = ClampAngle(_yaw, yawMin, yawMax);

         var ray = new Ray(targetPosition, (myPosition - targetPosition).normalized);
         var isBlocked = Physics.SphereCast(ray, 0.1f, out var hit, distance);
         var dist = isBlocked ? hit.distance : distance;
         dist = Mathf.Clamp(dist, distanceMin, distanceMax);
         
         Quaternion rotation = Quaternion.Euler(_yaw, _pitch, 0);
         var cameraTargetDistance = Vector3.Distance(targetPosition, myPosition);
         distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * cameraTargetDistance, distanceMin, distanceMax);
         Vector3 negDistance = new Vector3(0.0f, 0.0f, -dist);

         

         Vector3 position = rotation * negDistance + targetPosition;
         
         myTransform.rotation = rotation;
         myTransform.position = position;

         Quaternion viewTargetRotation = viewTarget.transform.rotation;
         Quaternion targetRotation = Quaternion.Euler(viewTargetRotation.eulerAngles.x, rotation.eulerAngles.y, viewTargetRotation.eulerAngles.z);
         viewTarget.transform.rotation = targetRotation;
     }

     public static float ClampAngle(float angle, float min, float max)
     {
         if (angle < -360F)
             angle += 360F;

         if (angle > 360F)
             angle -= 360F;
         return Mathf.Clamp(angle, min, max);
     }
 }