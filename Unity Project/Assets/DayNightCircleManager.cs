using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCircleManager : MonoBehaviour {
    private Material skybox;
    [Header("In Game Monitor")]
    [SerializeField] private float currentSunSize;
    [SerializeField] private float currentAtmosphereThickness;
    [SerializeField] private float dot;
    [SerializeField] private float startHour;
    public float hours_infloat;
    public float minutes_infloat;
    public float seconds_infloat;

    private float clockTimerMultiplier;

    [Header("Pre-Assigned Variables")]
  //  public Transform stars;
    public Transform[] clouds;
    public Light sunLight;
    public Gradient nightDayColor;

    public Vector3 dayRotateSpeed = new Vector3(-1, 0, 0);
    public Vector3 nightRotateSpeed = new Vector3(-1, 0, 0);

    public float maxIntensity = 1f;
    public float minIntensity = 0f;
    public float minPoint = -0.2f;

    public float maxAmbient = 1f;
    public float minAmbient = 0f;
    public float minAmbientPoint = -0.2f;

    public Gradient nightDayFogColor;
    public AnimationCurve fogDensityCurve;
    public float fogScale = 1f;

    public float dayAtmosphereThickness = 0.6f;
    public float nightAtmosphereThickness = 0.87f;

    [Header ("Changable Variables for designers")]
    public float sunSizeMin = 0.05f;
    public float sunSizeMax = 0.1f;
    public float initialCloudFlowSpeed = 1;
    private float currentCloudFlowSpeed;

    //if you set it to 1, means 1s in real life equal to 4 minute(240 seconds) in the game has passed, so 24 hours only need 6 minute to pass
    [SerializeField] private float timePassScale = 1;      //Generally control the time pass scale.                                

    private float atmosphereThickness_default;
    private float timer_sunSet;
    private float timer_sunRise;
    private float clockTimer;

    private float tRange;

    // Use this for initialization
    void Start () {
        skybox = RenderSettings.skybox;
        currentSunSize = skybox.GetFloat("_SunSize");
        currentAtmosphereThickness = dayAtmosphereThickness;
        skybox.SetFloat("_AtmosphereThickness", currentAtmosphereThickness);

        clockTimerMultiplier = 240 * timePassScale;
        tRange = 1 - minPoint;

        InitializeClockHour();
        //hours_infloat = 12;

    }
	
	// Update is called once per frame
	void Update () {
        //float dotSun = Mathf.Clamp01((Vector3.Dot(sunLight.transform.forward, Vector3.down) - minPoint) / tRange);
        dot = (Vector3.Dot(sunLight.transform.forward, Vector3.down) - minPoint) / tRange;
        
        if(dot >= 0)
        {
            currentSunSize = Mathf.Lerp(sunSizeMax, sunSizeMin, dot);
            skybox.SetFloat("_SunSize", currentSunSize);                        //Make the sun feel like larger in the moring and evening
        }
        UpdateClock();

        float lightIntensity = ((maxIntensity - minIntensity) * dot) + minIntensity;

        sunLight.intensity = lightIntensity;

        

        tRange = 1 - minAmbientPoint;
        dot = Mathf.Clamp01((Vector3.Dot(sunLight.transform.forward, Vector3.down) - minAmbientPoint) / tRange);

        lightIntensity = ((maxAmbient - minAmbient) * dot) + minAmbient;
        RenderSettings.ambientIntensity = lightIntensity;
        
        sunLight.color = nightDayColor.Evaluate(dot);
        RenderSettings.ambientLight = sunLight.color;
        
     
                //-----------------------------------------------------------------------------------------------//
        if (dot < 0.5)   //when the sun is about to set down, when the sun light
        {
            timer_sunSet += Time.deltaTime;
            currentAtmosphereThickness = Mathf.Lerp(dayAtmosphereThickness, nightAtmosphereThickness, timer_sunSet * timePassScale / 6);  //we keep the change of atmosphereThickness as nearly 30 minutes in realLife
            skybox.SetFloat("_AtmosphereThickness", currentAtmosphereThickness);

            timer_sunRise = 0;              //reset timer sunRise.
        }
        else         //when the sun is raising
        {
            

            timer_sunSet = 0;         // reset timer sunSet.
        }
        //skybox.SetFloat("_AtmosphereThickness", lightIntensity);

        sunLight.transform.Rotate(dayRotateSpeed * Time.deltaTime * timePassScale);


     

        if (Input.GetKeyDown(KeyCode.O)) timePassScale *= 0.5f;
        if (Input.GetKeyDown(KeyCode.P)) timePassScale *= 2f;
        //if (stars.gameObject != null)
        //{
        //    stars.transform.rotation = sunLight.transform.rotation;
        //}

        //IndicationCanvasManager.Instance.indicationText.text = "dot = " + dot;
        CloudFlow();
    }

    public void UpdateClock()
    {
        clockTimerMultiplier = 240 * timePassScale;
        clockTimer += Time.deltaTime;
        
        //hours = Mathf.Floor(clockTimer * clockTimerMultiplier / 60 / 60 % 24).ToString("00");
        //minutes = Mathf.Floor(clockTimer * clockTimerMultiplier / 60 % 60).ToString("00");
        //seconds = Mathf.Floor(clockTimer * clockTimerMultiplier % 60).ToString("00");

        hours_infloat = Mathf.Floor((startHour + clockTimer * clockTimerMultiplier / 60 / 60) % 24);
        minutes_infloat = Mathf.Floor(clockTimer * clockTimerMultiplier / 60 % 60);
        seconds_infloat = Mathf.Floor(clockTimer * clockTimerMultiplier % 60);
    
    }

    public void InitializeClockHour()
    {
        //Vector3 sunlightLocalForward = transform.worldToLocalMatrix.MultiplyVector(sunLight.transform.forward);
        dot = (Vector3.Dot(sunLight.transform.forward, Vector3.down) - minPoint) / tRange;

        startHour = (int)Mathf.Lerp(18, 12, Mathf.Clamp01(dot));
    }

    public void CloudFlow()
    {
        currentCloudFlowSpeed = initialCloudFlowSpeed * timePassScale;
        for(int i = 0; i < clouds.Length; i++)
        {
            clouds[i].RotateAround(Vector3.zero, Vector3.up, currentCloudFlowSpeed / 2 * Time.deltaTime);
        }
    }
}
