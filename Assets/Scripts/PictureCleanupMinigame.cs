using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PictureCleanupMinigame : Minigame
{
    [Header("Pictures")]
    public Texture2D[] pictures;          // the 3 pictures to clean (set Wrap Mode to Repeat on each)

    [Header("UI")]
    public RawImage pictureImage;         // shows the picture
    public RawImage noiseImage;           // same size, sitting ON TOP of the picture (no texture needed)
    public Slider filterSlider;           // Min 0, Max 1: clears the noise
    public Slider alignSlider;            // Min 0, Max 1: lines the picture up
    public Button saveButton;
    public TMP_Text titleText;            // "Picture 1 / 3"
    public TMP_Text feedbackText;

    [Header("Rules")]
    public float tolerance = 0.05f;       // how close each slider must be to its secret target
    public float maxNoise = 0.9f;         // how heavy the static looks when far off (0 to 1)
    public float flickerSeconds = 0.06f;  // how fast the static flickers

    private Texture2D noiseTexture;
    private int current;
    private float targetFilter;
    private float targetAlign;
    private float flickerTimer;

    protected override void Start()
    {
        base.Start();

        CreateNoiseTexture();

        filterSlider.onValueChanged.AddListener(_ => UpdatePicture());
        alignSlider.onValueChanged.AddListener(_ => UpdatePicture());
        saveButton.onClick.AddListener(SavePicture);
    }

    protected override void OnOpened()
    {
        current = 0;
        LoadPicture(current, "Use the sliders to clear the noise.");
    }

    private void Update()
    {
        if (!IsOpen)
            return;

        // Make the static flicker by jumping the noise texture around
        flickerTimer -= Time.deltaTime;
        if (flickerTimer <= 0f)
        {
            noiseImage.uvRect = new Rect(Random.value, Random.value, 4f, 4f);
            flickerTimer = flickerSeconds;
        }
    }

    private void CreateNoiseTexture()
    {
        int size = 128;
        noiseTexture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.wrapMode = TextureWrapMode.Repeat;

        Color32[] pixels = new Color32[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            byte v = (byte)Random.Range(0, 256);
            pixels[i] = new Color32(v, v, v, 255);
        }

        noiseTexture.SetPixels32(pixels);
        noiseTexture.Apply();
        noiseImage.texture = noiseTexture;
    }

    private void LoadPicture(int index, string message)
    {
        pictureImage.texture = pictures[index];

        if (titleText != null)
            titleText.text = "Picture " + (index + 1) + " / " + pictures.Length;

        // New secret settings for every picture
        targetFilter = Random.Range(0.2f, 0.8f);
        targetAlign = Random.Range(0.2f, 0.8f);

        filterSlider.value = 0f;
        alignSlider.value = 0f;

        SetFeedback(message);
        UpdatePicture();
    }

    private void UpdatePicture()
    {
        float filterError = Mathf.Abs(filterSlider.value - targetFilter);
        float alignError = Mathf.Abs(alignSlider.value - targetAlign);

        // The static fades away as the filter slider gets close to its target
        float noiseAlpha = Mathf.Clamp01((filterError - tolerance) * 2.5f) * maxNoise;
        noiseImage.color = new Color(1f, 1f, 1f, noiseAlpha);

        // The picture slides sideways when it's badly aligned
        float shift = Mathf.Max(0f, alignError - tolerance) * 0.3f;
        shift *= Mathf.Sign(alignSlider.value - targetAlign);
        pictureImage.uvRect = new Rect(shift, 0f, 1f, 1f);
    }

    private void SavePicture()
    {
        bool filterOk = Mathf.Abs(filterSlider.value - targetFilter) <= tolerance;
        bool alignOk = Mathf.Abs(alignSlider.value - targetAlign) <= tolerance;

        if (!filterOk || !alignOk)
        {
            SetFeedback("Still noisy or out of line. Keep adjusting.");
            return;
        }

        current++;

        if (current >= pictures.Length)
        {
            Complete();
        }
        else
        {
            LoadPicture(current, "Picture saved. Next one.");
        }
    }

    private void SetFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
    }
}
