using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MarsFPSKit
{
    namespace ZombieWaveSurvival
    {
        public class LightPower : MonoBehaviour
        {
            GameObject[] lights;
            public bool lightsOn = false;
            bool lightsTurnedOn = false;

            void Start() {
                lights = GameObject.FindGameObjectsWithTag("Light");
                // Should be dark at first
                for(int i = 0; i < lights.Length; i++)
                {
                    lights[i].GetComponent<Light>().enabled = lightsOn;         
                }
            }

            // Update is called once per frame
            void Update()
            {
                if (lightsOn && !lightsTurnedOn) {
                    // Should be dark at first
                    for(int i = 0; i < lights.Length; i++)
                    {
                        lights[i].GetComponent<Light>().enabled = lightsOn;         
                    }
                    lightsTurnedOn = true;
                }
            }
        }
    }
}
