using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WipeMinigame : Minigame
{
    [Header("Solar panels")]
    public Renderer[] dustLayers;      // a Quad just above each solar panel, with a Mesh Collider
    public int textureSize = 128;      // dust detail (128 is plenty)
    public Color dustColor = new Color(0.72f, 0.42f, 0.25f);
    [Range(0.2f, 1f)]
    public float dustAlpha = 0.9f;     // how thick the dust starts

    [Header("Wiping")]
    public float brushRadius = 9f;     // in texture pixels
    public float wipeStrength = 2f;    // how fast dust comes off (higher = faster)
    [Range(0.5f, 1f)]
    public float requiredFraction = 0.9f;   // how clean it must be to finish

    [Header("UI")]
    public Image progressFill;         // Image Type = Filled
    public TMP_Text progressText;      // optional

    [Header("Camera view")]
    public Transform cameraFocus;      // optional close-up of the panels

    private class Layer
    {
        public Collider collider;
        public Texture2D texture;
        public float[] alpha;
        public bool[] clean;
        public Color32[] pixels;
        public int cleanCount;
    }

    private List<Layer> layers = new List<Layer>();
    private Camera cam;
    private Vector3 savedCamLocalPos;
    private Quaternion savedCamLocalRot;

    protected override void Start()
    {
        base.Start();

        foreach (Renderer r in dustLayers)
            layers.Add(BuildLayer(r));

        UpdateUI(0f);
    }

    private Layer BuildLayer(Renderer r)
    {
        int n = textureSize;
        Layer layer = new Layer();
        layer.collider = r.GetComponent<Collider>();
        layer.alpha = new float[n * n];
        layer.clean = new bool[n * n];
        layer.pixels = new Color32[n * n];

        // Blotchy dust, built from noise
        float seed = Random.value * 100f;
        for (int y = 0; y < n; y++)
        {
            for (int x = 0; x < n; x++)
            {
                int i = y * n + x;
                float blotch = Mathf.PerlinNoise(seed + x * 0.08f, seed + y * 0.08f);
                float shade = 0.85f + blotch * 0.3f;

                layer.alpha[i] = dustAlpha * (0.9f + Random.value * 0.1f);
                layer.pixels[i] = new Color32(
                    (byte)Mathf.Clamp(dustColor.r * shade * 255f, 0f, 255f),
                    (byte)Mathf.Clamp(dustColor.g * shade * 255f, 0f, 255f),
                    (byte)Mathf.Clamp(dustColor.b * shade * 255f, 0f, 255f),
                    255);
            }
        }

        layer.texture = new Texture2D(n, n, TextureFormat.RGBA32, false);
        layer.texture.wrapMode = TextureWrapMode.Clamp;
        ApplyTexture(layer);

        // A simple see-through material that works in any render pipeline
        Material m = new Material(Shader.Find("Sprites/Default"));
        m.mainTexture = layer.texture;
        r.material = m;

        return layer;
    }

    protected override void OnOpened()
    {
        cam = Camera.main;
        savedCamLocalPos = cam.transform.localPosition;
        savedCamLocalRot = cam.transform.localRotation;

        if (cameraFocus != null)
            cam.transform.SetPositionAndRotation(cameraFocus.position, cameraFocus.rotation);

        UpdateUI(GetProgress());
    }

    protected override void OnClosed()
    {
        cam.transform.localPosition = savedCamLocalPos;
        cam.transform.localRotation = savedCamLocalRot;
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

        Mouse mouse = Mouse.current;
        if (mouse == null || !mouse.leftButton.isPressed)
            return;

        Ray ray = cam.ScreenPointToRay(mouse.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, 50f))
            return;

        foreach (Layer layer in layers)
        {
            if (hit.collider != layer.collider)
                continue;

            Wipe(layer, hit.textureCoord);

            float progress = GetProgress();
            UpdateUI(progress);

            if (progress >= requiredFraction)
                FinishCleaning();

            break;
        }
    }

    private void Wipe(Layer layer, Vector2 uv)
    {
        int n = textureSize;
        int cx = Mathf.RoundToInt(uv.x * (n - 1));
        int cy = Mathf.RoundToInt(uv.y * (n - 1));
        int r = Mathf.CeilToInt(brushRadius);

        for (int y = Mathf.Max(0, cy - r); y <= Mathf.Min(n - 1, cy + r); y++)
        {
            for (int x = Mathf.Max(0, cx - r); x <= Mathf.Min(n - 1, cx + r); x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cx, cy));
                if (dist > brushRadius)
                    continue;

                float falloff = 1f - dist / brushRadius;
                int i = y * n + x;

                layer.alpha[i] = Mathf.Max(0f, layer.alpha[i] - wipeStrength * falloff * Time.deltaTime);

                if (!layer.clean[i] && layer.alpha[i] < 0.1f)
                {
                    layer.clean[i] = true;
                    layer.cleanCount++;
                }
            }
        }

        ApplyTexture(layer);
    }

    private void ApplyTexture(Layer layer)
    {
        for (int i = 0; i < layer.pixels.Length; i++)
            layer.pixels[i].a = (byte)(layer.alpha[i] * 255f);

        layer.texture.SetPixels32(layer.pixels);
        layer.texture.Apply();
    }

    private float GetProgress()
    {
        int cleaned = 0;
        int total = 0;

        foreach (Layer layer in layers)
        {
            cleaned += layer.cleanCount;
            total += layer.alpha.Length;
        }

        return total == 0 ? 0f : (float)cleaned / total;
    }

    private void UpdateUI(float progress)
    {
        // Show 100% once the required amount is reached
        float shown = Mathf.Clamp01(progress / requiredFraction);

        if (progressFill != null)
            progressFill.fillAmount = shown;

        if (progressText != null)
            progressText.text = "Cleaning: " + Mathf.RoundToInt(shown * 100f) + "%";
    }

    private void FinishCleaning()
    {
        // Wipe away whatever dust is left
        foreach (Layer layer in layers)
        {
            for (int i = 0; i < layer.alpha.Length; i++)
                layer.alpha[i] = 0f;

            ApplyTexture(layer);
        }

        UpdateUI(1f);
        Complete();
    }
}
