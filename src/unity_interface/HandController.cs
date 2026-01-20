using System.IO.Ports;
using UnityEngine;
using System; 

public class HandController : MonoBehaviour
{
    public string portName = "COM3";  // The COM port the ESP32 is connected to
    public int baudRate = 115200;     // Matching the baud rate set in the ESP32 code

    private SerialPort stream;        
    public GameObject[] indexFingerJoints;  // Array to hold joints for the index finger
    public GameObject[] middleFingerJoints; // Array to hold joints for the middle finger
    public GameObject[] ringFingerJoints;   // Array to hold joints for the ring finger
    public GameObject[] pinkyFingerJoints;  // Array to hold joints for the pinky finger
    public GameObject[] thumbJoints;        // Array to hold joints for the thumb
    public GameObject wrist;                // GameObject for the wrist control

    private int[] potentiometerValues = new int[5];  // Array to store potentiometer values for each finger
    private int[] potentiometerOffsets = { 0, 0, 0, 0, 0 };  // Offset for each potentiometer based on the calibration that is conducted
    private float yaw, pitch, roll;                  // Variables to hold Yaw, Pitch, and Roll data
    private float yawOffset = -180.0f;  // Offset for yaw based on calibration
    private float pitchOffset = -90.0f;  // Offset for pitch based on calibration
    private float rollOffset = -180.0f;  // Offset for roll based on calibration
    private Vector3 acceleration;        // Vector3 to hold acceleration data for wrist position

    void Start()
    {
        stream = new SerialPort(portName, baudRate);
        if (!stream.IsOpen)
        {
            try
            {
                stream.Open();  // open the serial port
                stream.ReadTimeout = 50;
                Debug.Log("Serial port opened successfully."); // debugging message to ensure connection
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to open serial port: " + e.Message); // debugging message to ensure connection
            }
        }
    }

    void Update()
    {
        if (stream.IsOpen)
        {
            try
            {
                string dataString = stream.ReadLine();  // Read the line of data from the serial port
                Debug.Log("Received: " + dataString);
                ProcessData(dataString);  // Process the received data
            }
            catch (System.TimeoutException)
            {
                
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("Failed to read from serial: " + e.Message);
            }
        }
    }

    void ProcessData(string data)
    {
        string[] tokens = data.Split(',');
        if (tokens.Length == 11)  // Ensure that we are receiving exactly 11 pieces of data
        {
            for (int i = 0; i < 5; i++)
            {
                potentiometerValues[i] = int.Parse(tokens[i]) + potentiometerOffsets[i];  // Parse each potentiometer value and apply offset
                UpdateFinger(i, potentiometerValues[i]);  // Update the corresponding finger
            }

            yaw = float.Parse(tokens[5]) + yawOffset;  // Parse Yaw value and apply offset
            pitch = float.Parse(tokens[6]) + pitchOffset;  // Parse Pitch value and apply offset
            roll = float.Parse(tokens[7]) + rollOffset;  // Parse Roll value and apply offset
            acceleration = new Vector3(float.Parse(tokens[8]), float.Parse(tokens[9]), float.Parse(tokens[10]));
            UpdateWristOrientation();  // Update the orientation of the wrist
            Debug.Log($"Adjusted YPR: Yaw={yaw}, Pitch={pitch}, Roll={roll}, Accel={acceleration}");
        }
        else
        {
            Debug.LogWarning($"Unexpected data format received. Received {tokens.Length} fields.");
        }
    }

    void UpdateFinger(int index, int potentiometerValue)
    {
        GameObject[] joints = index switch
        {
            0 => indexFingerJoints,
            1 => middleFingerJoints,
            2 => ringFingerJoints,
            3 => pinkyFingerJoints,
            4 => thumbJoints,
            _ => null
        };

        if (joints != null)
        {
            float baseAngle = Mathf.Lerp(0, 90, potentiometerValue / 1023.0f);
            foreach (GameObject joint in joints)
            {
                float angleModifier = CalculateAngleModifier(joints.Length);
                joint.transform.localRotation = Quaternion.Euler(-baseAngle * angleModifier, 0, 0);
            }
        }
    }

    void UpdateWristOrientation()
    {
        Quaternion newOrientation = Quaternion.Euler(-pitch, -yaw, -roll);
        wrist.transform.localRotation = newOrientation;
        wrist.transform.localPosition = acceleration;  // Assume acceleration data represents positional changes
    }

    float CalculateAngleModifier(int totalJoints)
    {
        return 1.0f - (1 / (float)totalJoints);  // Simplified to affect all joints evenly
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (GameObject[] fingerJoints in new GameObject[][] { indexFingerJoints, middleFingerJoints, ringFingerJoints, pinkyFingerJoints, thumbJoints })
        {
            for (int i = 0; i < fingerJoints.Length; i++)
            {
                if (collision.collider.gameObject == fingerJoints[i])
                {
                    float angle = fingerJoints[i].transform.localEulerAngles.x; // Get the current angle of the joint
                    SendServoCommand(i + 1, Mathf.RoundToInt(angle)); // Send command to Arduino
                    break;
                }
            }
        }
    }

    void SendServoCommand(int servoID, int angle)
    {
        if (stream.IsOpen)
        {
            stream.WriteLine($"{servoID},{angle}"); // Send servo ID and angle in the format "servoID,angle"
        }
    }

    void OnDestroy()
    {
        if (stream != null && stream.IsOpen)
        {
            stream.Close();  // Ensure to close the serial port when the application or script is stopped
            Debug.Log("Serial port closed.");
        }
    }
}
