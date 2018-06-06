// @Copyright Davide Galdo 2017
// @License GPLv3
// @Virtual reality sound controller



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

    public AudioSource screamSound = null;      //Noise and voice from the people
    public AudioSource stuffNoiseSound = null;  //Noise from the object
    public AudioSource earthquakeSound = null;  //Earthquake proper source

    
    private bool _lowIntensityisPlaying = false; //Used to check if the low intensity sounds are playing
    /// <summary>
    /// True if low intensity sounds are playing false otherwise
    /// </summary>
    public bool lowIntensityisPlaying
    {
        get { return _lowIntensityisPlaying; }
    }

    private bool playHighIntensity = false;
    private bool playLowIntensity = false;


    private bool _highIntensityisPlaying = false;  //Used to check if the high intensity sounds are playing
    private bool playScream = false;

    /// <summary>
    /// True if high intensity sounds are playing false otherwise
    /// </summary>
    public bool highIntensityisPlaying
    {
        get { return _highIntensityisPlaying; }
    }


    /// <summary>
    /// Start the low intensity earthquake sounds
    /// </summary>
    public void StartLowIntensitySounds()
    {
        _lowIntensityisPlaying = true;
        playLowIntensity = true;
        
    }

    /// <summary>
    /// Stop the low intensity earthquake sounds
    /// </summary>
    public void StopLowIntensitySounds()
    {
        _lowIntensityisPlaying = false;
        playLowIntensity = false;
    }

    /// <summary>
    /// Start the hight intensity earthquake sounds
    /// </summary>
    public void StartHighIntensitySounds()
    {
        _highIntensityisPlaying = true;
        playHighIntensity = true;
        playScream = true;
        
    }

    /// <summary>
    /// Stop the hight intensity earthquake sounds
    /// </summary>
    public void StopHighIntensitySounds()
    {
        _highIntensityisPlaying = false;
        playHighIntensity = false;


    }

    public void StopScream()
    {
        playScream = false;
    }

    void Update()
    {

        //Play-Stop low intensity Noise
        if (playLowIntensity)
        {
            if (stuffNoiseSound != null)
            {
                stuffNoiseSound.volume = 0.4f;
                if (!stuffNoiseSound.isPlaying)
                {
                    stuffNoiseSound.Play();
                }
            }

            if (earthquakeSound != null)
            {
                earthquakeSound.volume = 0.1f;
                if (!earthquakeSound.isPlaying)
                {
                    earthquakeSound.Play();
                }
            }
        }else
        {
            if (stuffNoiseSound != null && stuffNoiseSound.isPlaying)
                stuffNoiseSound.Stop();
            if (earthquakeSound != null && earthquakeSound.isPlaying)            
                earthquakeSound.Stop();
        }

            //Play Stop high intensity noise

            if (playHighIntensity)
            {
                if (stuffNoiseSound != null)                
                    stuffNoiseSound.volume = 1f;

                if (earthquakeSound != null)
                    earthquakeSound.volume = 0.5f;

                if (screamSound != null && playScream)
                    screamSound.Play();
            }else
            {
                if (stuffNoiseSound != null)
                    stuffNoiseSound.volume = 0.4f;

                if (earthquakeSound != null)
                    earthquakeSound.volume = 0.1f;

                if (screamSound != null && !playScream )               
                    screamSound.Stop();
            }





    }

    



}
