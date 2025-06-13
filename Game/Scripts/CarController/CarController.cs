using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum CarType
    {
        FrontWheelDrive,
        RearWheelDrive,
        FourWheelDrive
    }

    public CarType carType = CarType.FourWheelDrive;

    public enum ControlMode
    {
        Keyboard,
        Button
    }

    public ControlMode control;

    [Header("Wheel GameObject Meshes")]
    public GameObject FrontWheelLeft;
    public GameObject FrontWheelRight;
    public GameObject BackWheelLeft;
    public GameObject BackWheelRight;

    [Header("WheelColliders")]
    public WheelCollider frontWheelLeftCollider;
    public WheelCollider frontWheelRightCollider;
    public WheelCollider backWheelLeftCollider;
    public WheelCollider backWheelRightCollider;

    [Header("Movement, Steering, and Braking")]
    private float currentSpeed;
    public float maximumMotorTorque;
    public float maximumSteeringAngle = 20f;
    public float maxSpeed;
    public float brakePower;
    public Transform COM;
    float carSpeed;
    float carSpeedConverted;
    float motorTorque;
    float steeringAngle;
    float vertical = 0f;
    float horizontal = 0f;
    bool handBrake = false;
    Rigidbody carRigidBody;
    float tireAngle;

    [Header("Sounds And Effects")]
    public ParticleSystem[] smokeEffects;
    private bool smokeEffectEnabled;
    public AudioSource engineSound;
    public AudioClip engineClip;

    [Header("Laps")]
    public int maxLaps;
    public int currentLap = 0;

    void Start()
    {
        carRigidBody = GetComponent<Rigidbody>();

        if (carRigidBody != null)
        {
            carRigidBody.centerOfMass = COM.localPosition;
        }
        engineSound.volume = 0.5f;
        engineSound.pitch = 1f;
        engineSound.Play();
        engineSound.Pause();

        maxLaps = FindObjectOfType<LapSystem>().maxLaps;
    }

    void Update()
    {
        GetInputs();
        CalculateStearing();
        ApplyTransformToWheels();
    }

    void FixedUpdate()
    {
        CalculateCarMovement();
    }

    void GetInputs()
    {
        if (control == ControlMode.Keyboard)
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
        }
    }

    void CalculateCarMovement()
    {
        carSpeed = carRigidBody.velocity.magnitude;
        carSpeedConverted = Mathf.Round(carSpeed * 3.6f);

        handBrake = Input.GetKey(KeyCode.Space);

        if (handBrake)
        {
            ApplyBrake();
            motorTorque = 0;

            if (!smokeEffectEnabled)
            {
                EnableSmokeEffect(true);
                smokeEffectEnabled = true;
            }
        }
        else
        {
            ReleaseBrake();

            if (carSpeedConverted < maxSpeed)
                motorTorque = maximumMotorTorque * vertical;
            else
                motorTorque = 0;

            if (smokeEffectEnabled)
            {
                EnableSmokeEffect(false);
                smokeEffectEnabled = false;
            }

            if (carSpeedConverted > 0 || handBrake)
            {
                engineSound.UnPause();

                float gearRatio = currentSpeed / maxSpeed;
                int numberOfGears = 6;
                int currentGear = Mathf.Clamp(Mathf.FloorToInt(gearRatio * numberOfGears) + 1, 1, numberOfGears);

                float pitchMultiplier = 0.5f + (carSpeedConverted / maxSpeed);
                float volumeMultiplier = 0.2f + 0.8f * (carSpeedConverted / maxSpeed);

                engineSound.pitch = Mathf.Lerp(0.5f, 1f, pitchMultiplier) * currentGear;
                engineSound.volume = volumeMultiplier;
            }
            else
            {
                engineSound.Pause();
                engineSound.pitch = 0.5f;
                engineSound.volume = 0.2f;
            }
        }

        ApplyMotorTorque();
    }

    void CalculateStearing()
    {
        tireAngle = maximumSteeringAngle * horizontal;
        frontWheelLeftCollider.steerAngle = tireAngle;
        frontWheelRightCollider.steerAngle = tireAngle;
    }

    void ApplyBrake()
    {
        frontWheelLeftCollider.brakeTorque = brakePower;
        frontWheelRightCollider.brakeTorque = brakePower;
        backWheelLeftCollider.brakeTorque = brakePower;
        backWheelRightCollider.brakeTorque = brakePower;
    }

    void ApplyMotorTorque()
    {
        if (carType == CarType.FrontWheelDrive)
        {
            frontWheelLeftCollider.motorTorque = motorTorque;
            frontWheelRightCollider.motorTorque = motorTorque;
        }
        else if (carType == CarType.RearWheelDrive)
        {
            backWheelLeftCollider.motorTorque = motorTorque;
            backWheelRightCollider.motorTorque = motorTorque;
        }
        else if (carType == CarType.FourWheelDrive)
        {
            backWheelLeftCollider.motorTorque = motorTorque;
            backWheelRightCollider.motorTorque = motorTorque;
            frontWheelLeftCollider.motorTorque = motorTorque;
            frontWheelRightCollider.motorTorque = motorTorque;
        }
    }

    void ReleaseBrake()
    {
        frontWheelLeftCollider.brakeTorque = 0;
        frontWheelRightCollider.brakeTorque = 0;
        backWheelLeftCollider.brakeTorque = 0;
        backWheelRightCollider.brakeTorque = 0;
    }

    public void ApplyTransformToWheels()
    {
        Vector3 position;
        Quaternion rotation;

        frontWheelLeftCollider.GetWorldPose(out position, out rotation);
        FrontWheelLeft.transform.position = position;
        FrontWheelLeft.transform.rotation = rotation;

        frontWheelRightCollider.GetWorldPose(out position, out rotation);
        FrontWheelRight.transform.position = position;
        FrontWheelRight.transform.rotation = rotation;

        backWheelLeftCollider.GetWorldPose(out position, out rotation);
        BackWheelLeft.transform.position = position;
        BackWheelLeft.transform.rotation = rotation;

        backWheelRightCollider.GetWorldPose(out position, out rotation);
        BackWheelRight.transform.position = position;
        BackWheelRight.transform.rotation = rotation;
    }

    private void EnableSmokeEffect(bool enable)
    {
        foreach (ParticleSystem smokeEffect in smokeEffects)
        {
            if (enable)
                smokeEffect.Play();
            else
                smokeEffect.Stop();
        }
    }

    public void IncreaseLap() 
    {
        currentLap++; // ✅ Now works without error
        Debug.Log(gameObject.name + " Lap: "+ currentLap);
    }
}
