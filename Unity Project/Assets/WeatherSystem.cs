﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : Photon.MonoBehaviour {

    bool appliedInitialUpdate;
    int correctWeather;

    public GameObject[] listofWeathers;
    public int currentWeatherNo;
    public int RngNumber;
    public float timer;
	// Use this for initialization
	void Start () {
        //Your load stuff from the connection
        //currentWeatherNo =;
        currentWeatherNo = Random.Range(0, 2);
      
    }
    //void RngGenerate()
    //{
    //    RngNumber = Random.Range(0, 2);
       
    //}
    // Update is called once per frame
    void Update () {

        if (!PhotonNetwork.isMasterClient)
            currentWeatherNo = correctWeather;
        else
        {
            timer += 1.0f * Time.deltaTime;
            //Save currentweatherNo to connection;
            if (timer > 10)
            {
                timer = 0;

                currentWeatherNo = Random.Range(0, 3);
            }
        }

        if (currentWeatherNo == 0 || currentWeatherNo == 3)
        {//Stop all weather , if its 0 means its clear weather
            listofWeathers[1].SetActive(false);
            listofWeathers[2].SetActive(false);
        }
        else if (currentWeatherNo == 1)
        {
            listofWeathers[1].SetActive(true);
            listofWeathers[2].SetActive(false);

        }
        else if (currentWeatherNo == 2)
        {
            listofWeathers[2].SetActive(true);
            listofWeathers[1].SetActive(false);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            // stream.SendNext((int)controllerScript._characterState);
            stream.SendNext(currentWeatherNo);

        }
        else
        {
            correctWeather = (int)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                currentWeatherNo = correctWeather;
            }
        }
    }
}
