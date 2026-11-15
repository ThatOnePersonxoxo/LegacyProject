using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HeatStickMinigame : Minigame
{
    [Header("The probe camera")]
    public Renderer cameraRenderer;        // gets tinted: blue (cold), green (good), red (hot)
    public Color coldColor = new Color(0.3f, 0.5f, 1f);
    public Color goodColor = new Color(0.3f, 1f, 0.4f);
    public Color hotColor = new Color(1f, 0.3f, 0.2f);

    [Header("Heat-stick")]
    public Transform heatStick;            // the stick model
    public Transform stickRestPoint;       // empty object: where the stick waits
    public Transform stickHeatPoint;       // empty object: where the stick touches the camera
    public float stickMoveSpeed = 8f;
    public ParticleSystem heatEffect;      // optional glow or sparks while heating
    public AudioSource heatSound;          // optional looping sound while heating

    [Header("Gauge (in the world)")]
    public Transform needle;               // rotates around its local Z axis
    public float needleAngleAtMin = 120f;
    public float needleAngleAtMax = -120f;
    public TMP_Text statusText;            // optional readout (world-space or screen)

    [Header("Rules")]
    public float startTemperature = 20f;
    public float minTemperature = 0f;
    public float maxTemperature = 100f;
    public float goodMin = 45f;            // bottom of the green band
    public float goodMax = 65f;            // top of the green band
    public float overheatTemperature = 90f;
    public float heatRate = 25f;           // degrees gained per second while heating
    public float coolRate = 8f;            // degrees lost per second (varies a little over time)
    public float requiredSeconds = 10f;    // time to spend in the green band
    public float progressLossRate = 0.5f;  // progress lost per second when outside the band

    [Header("Camera view")]
    public Transform cameraFocus;          // optional close-up position

    private float temperature;
    private float progress;
    private float overheatMessageTimer;
    private Camera cam;
    private Vector3 savedCamLocalPos;
    private Quaternion savedCamLocalRot;
    private Color originalCameraColor;

    protected override void OnOpened()
    {
        cam = Camera.main;
        savedCamLocalPos = cam.transform.localPosition;
        savedCamLocalRot = cam.transform.localRotation;

        if (cameraFocus != null)
        {
            cam.transform.localPosition =
            cam.transform.parent.InverseTransformPoint(cameraFocus.position);

            cam.transform.localRotation = Quaternion.Inverse(cam.transform.parent.rotation) * cameraFocus.rotation;
        }

        if (cameraRenderer != null)
            originalCameraColor = cameraRenderer.material.color;

        if (heatStick != null && stickRestPoint != null)
            heatStick.SetPositionAndRotation(stickRestPoint.position, stickRestPoint.rotation);

        temperature = startTemperature;
        progress = 0f;
        overheatMessageTimer = 0f;
    }

    protected override void OnClosed()
    {
        cam.transform.localPosition = savedCamLocalPos;
        cam.transform.localRotation = savedCamLocalRot;

        if (cameraRenderer != null)
            cameraRenderer.material.color = originalCameraColor;

        if (heatEffect != null)
            heatEffect.Stop();
        if (heatSound != null)
            heatSound.Stop();
        Debug.Log("CAMERA RESTORED");
        Debug.Log("Local Position: " + cam.transform.localPosition);
        Debug.Log("World Position: " + cam.transform.position);
        Debug.Log("Parent: " + cam.transform.parent.name);
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Close();
            return;
        }

        bool heating = IsHeating();

        UpdateTemperature(heating);
        UpdateStick(heating);
        UpdateVisuals();
        UpdateProgress();
    }

    private bool IsHeating()
    {
        bool mouse = Mouse.current != null && Mouse.current.leftButton.isPressed;
        bool space = Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        return mouse || space;
    }

    private void UpdateTemperature(bool heating)
    {
        // Cooling speed wanders between 0.6x and 1.4x so the player can't just memorize a rhythm
        float drift = 0.6f + Mathf.PerlinNoise(Time.time * 0.7f, 0f) * 0.8f;

        temperature -= coolRate * drift * Time.deltaTime;
        if (heating)
            temperature += heatRate * Time.deltaTime;

        temperature = Mathf.Clamp(temperature, minTemperature, maxTemperature);

        // Too much heat damages the camera: start over
        if (temperature >= overheatTemperature)
        {
            temperature = startTemperature;
            progress = 0f;
            overheatMessageTimer = 2f;
        }

        if (overheatMessageTimer > 0f)
            overheatMessageTimer -= Time.deltaTime;
    }

    private void UpdateStick(bool heating)
    {
        if (heatStick != null && stickRestPoint != null && stickHeatPoint != null)
        {
            Transform goal = heating ? stickHeatPoint : stickRestPoint;
            float k = stickMoveSpeed * Time.deltaTime;
            heatStick.position = Vector3.Lerp(heatStick.position, goal.position, k);
            heatStick.rotation = Quaternion.Slerp(heatStick.rotation, goal.rotation, k);
        }

        if (heatEffect != null)
        {
            if (heating && !heatEffect.isPlaying) heatEffect.Play();
            else if (!heating && heatEffect.isPlaying) heatEffect.Stop();
        }

        if (heatSound != null)
        {
            if (heating && !heatSound.isPlaying) heatSound.Play();
            else if (!heating && heatSound.isPlaying) heatSound.Stop();
        }
    }

    private void UpdateVisuals()
    {
        // Gauge needle
        if (needle != null)
        {
            float t01 = Mathf.InverseLerp(minTemperature, maxTemperature, temperature);
            float angle = Mathf.Lerp(needleAngleAtMin, needleAngleAtMax, t01);
            needle.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        // Camera color: blue -> green -> red
        if (cameraRenderer != null)
        {
            Color c;
            if (temperature < goodMin)
                c = Color.Lerp(coldColor, goodColor, Mathf.InverseLerp(minTemperature, goodMin, temperature));
            else if (temperature <= goodMax)
                c = goodColor;
            else
                c = Color.Lerp(goodColor, hotColor, Mathf.InverseLerp(goodMax, overheatTemperature, temperature));

            cameraRenderer.material.color = c;
        }

        // Text readout
        if (statusText != null)
        {
            string message;
            if (overheatMessageTimer > 0f)
                message = "Overheated! Starting over.";
            else if (temperature < goodMin)
                message = "Too cold. Hold to heat.";
            else if (temperature > goodMax)
                message = "Too hot. Let it cool.";
            else
                message = "Perfect. Keep it steady.";

            statusText.text = "Temperature: " + temperature.ToString("0") + "°\n"
                + "Stable: " + progress.ToString("0.0") + " / " + requiredSeconds.ToString("0") + " s\n"
                + message;
        }
    }

    private void UpdateProgress()
    {
        bool inBand = temperature >= goodMin && temperature <= goodMax;

        if (inBand)
            progress += Time.deltaTime;
        else
            progress = Mathf.Max(0f, progress - progressLossRate * Time.deltaTime);

        if (progress >= requiredSeconds)
        {
            Complete();
        }
    }
}
