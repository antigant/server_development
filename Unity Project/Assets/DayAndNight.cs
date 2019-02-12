using UnityEngine;



public class DayAndNight : MonoBehaviour
{

    public Gradient nightColor;
    public Gradient dayColor;
    public Gradient LightColor;

    public float maxIntensity = 1f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;

    public float maxAmbient = 1f;
    public float minAmbient = 0f;
    public float minAmbientPoint = -0.2f;

    public Gradient nightdayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale = 1f;

    public float dayAtmosphereThickness = 0.5f;
    public float nightAtmosphereThickness = 0.5f;

    public Vector3 dayRotateSpeed;
    public Vector3 nightRotateSpeed;

    [SerializeField]
    float skyspeed = 1;

    Light mainLight;
    Skybox sky;
    UnityEngine.Material skyMat;

    //public GameObject starry;
    //public GameObject attachedLight;
    // Use this for initialization
    void Start()
    {
        mainLight = GetComponent<Light>();
        skyMat = RenderSettings.skybox;
    }

    void SetAmbience(float howmuch)
    {
        maxAmbient = howmuch;
    }
    // Update is called once per frame
    void Update()
    {
        float tRange = 1 - minPoint;
        float dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minPoint) / tRange);
        float i = ((maxIntensity - minIntensity) * dot) + minIntensity;

        mainLight.intensity = 1;

        tRange = i - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(mainLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
        i = (maxAmbient - minAmbient * dot) + minAmbient;

        //  RenderSettings.ambientIntensity = i;

        if (minIntensity < 0)
        {
            mainLight.color = nightColor.Evaluate(dot);
            RenderSettings.ambientLight = nightColor.Evaluate(dot);
        }
       else if (minIntensity >= 0)
        {
            mainLight.color = dayColor.Evaluate(dot);
            RenderSettings.ambientLight = LightColor.Evaluate(dot);
        }
        //7E749F

        // RenderSettings.

        RenderSettings.fogColor = nightdayFogColor.Evaluate(dot);
        RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

        i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
        skyMat.SetFloat("_AtmosphereThickness", i);

        if (dot > 0)
            transform.Rotate(dayRotateSpeed * Time.deltaTime * skyspeed);
        else
            transform.Rotate(nightRotateSpeed * Time.deltaTime * skyspeed);

        if (Input.GetKeyDown(KeyCode.Equals)) skyspeed += 0.5f;
        if (Input.GetKeyDown(KeyCode.Minus)) skyspeed -= 0.5f;
        
        //if(attachedLight.gameObject.transform.eulerAngles.x > 180)
        //{
        //    starry.SetActive(true);
        //}
        //else 
        //{
        //    starry.SetActive(false);
        //}

    }
}