using Photon.Pun;
using System;
using UnityEngine;

namespace MarsFPSKit
{
    public class BloodyScreenVitalsRuntimeData
    {
        public float hitPoints;
        public float maxHP = 100f;
        public float lastHit;
        /// <summary>
        /// For displaying the bloody screen
        /// </summary>
        public float hitAlpha;
    }

    /// <summary>
    /// Implements a CoD style health regeneration type of health system
    /// </summary>
    [CreateAssetMenu(menuName = "MarsFPSKit/Vitals/Bloody Screen")]
    public class Kit_BloodyScreenVitals : Kit_VitalsBase
    {
        public float simpleBloodyScreenTime = 3f;
        /// <summary>
        /// How many seconds do we wait (since the last hit) until we regenerate our health?
        /// </summary>
        public float timeUntilHealthIsRegenerated = 5f;
        /// <summary>
        /// How fast do we recover? HP/S
        /// </summary>
        public float healthRegenerationSpeed = 25f;

        /// <summary>
        /// First person hit reactions will be applied if this is set to true
        /// </summary>
        public bool hitReactionEnabled = true;
        /// <summary>
        /// Intensity of the hit reactions
        /// </summary>
        public float hitReactionsIntensity = 1.2f;
        /// <summary>
        /// How fast will we recover from the hit reactions?
        /// </summary>
        public float hitReactionsReturnSpeed = 5f;
        /// <summary>
        /// Check this if you want to display the health like in the simple vitals
        /// </summary>
        public bool displayHealthLikeSimple;

        /// <summary>
        /// ID for fall death CAT
        /// </summary>
        public int fallDamageSoundCatID;
        /// <summary>
        /// ID for out of map death CAT
        /// </summary>
        public int outOfMapSoundCatID;

        public override void ApplyHeal(Kit_PlayerBehaviour pb, float heal)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                vrd.hitPoints = Mathf.Clamp(vrd.hitPoints + heal, 0, vrd.maxHP);
            }
        }

        public override void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, int idWhoShot, int gunID, Vector3 shotFrom)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                //Check if we can take damage
                if (!pb.spawnProtection || pb.spawnProtection.CanTakeDamage(pb))
                {
                    //Check for hitpoints
                    if (vrd.hitPoints > 0)
                    {
                        //Set time
                        vrd.lastHit = Time.time;
                        //Apply damage
                        vrd.hitPoints -= dmg;
                        //Hit reactions
                        if (hitReactionEnabled)
                        {
                            Vector3 dir = (pb.playerCameraTransform.InverseTransformDirection(Vector3.Cross(pb.playerCameraTransform.forward, pb.transform.position - shotFrom))).normalized * hitReactionsIntensity;
                            dir *= Mathf.Clamp(dmg / 30f, 0.3f, 1f);

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.playerCameraHitReactionsTransform, dir, 0.1f));

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.weaponsHitReactions, dir * 2f, 0.1f));
                        }
                        //Set damage effect
                        vrd.hitAlpha = 2f;
                        //Play voice
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Projectile);
                        }
                        //Check for death
                        if (vrd.hitPoints <= 0)
                        {
                            //Call the die function on pb
                            pb.Die(botShot, idWhoShot, gunID);
                        }
                    }
                }
            }
        }

        public override void ApplyDamage(Kit_PlayerBehaviour pb, float dmg, bool botShot, int idWhoShot, string deathCause, Vector3 shotFrom)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                //Check if we can take damage
                if (!pb.spawnProtection || pb.spawnProtection.CanTakeDamage(pb))
                {
                    //Check for hitpoints
                    if (vrd.hitPoints > 0)
                    {
                        //Set time
                        vrd.lastHit = Time.time;
                        //Apply damage
                        vrd.hitPoints -= dmg;
                        //Hit reactions
                        if (hitReactionEnabled)
                        {
                            Vector3 dir = (pb.playerCameraTransform.InverseTransformDirection(Vector3.Cross(pb.playerCameraTransform.forward, pb.transform.position - shotFrom))).normalized * hitReactionsIntensity;
                            dir *= Mathf.Clamp(dmg / 30f, 0.3f, 1f);

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.playerCameraHitReactionsTransform, dir, 0.1f));

                            Kit_ScriptableObjectCoroutineHelper.instance.StartCoroutine(Kit_ScriptableObjectCoroutineHelper.instance.Kick(pb.weaponsHitReactions, dir * 2f, 0.1f));
                        }
                        //Set damage effect
                        vrd.hitAlpha = 2f;
                        //Play voice
                        if (pb.voiceManager)
                        {
                            pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Projectile);
                        }
                        //Check for death
                        if (vrd.hitPoints <= 0)
                        {
                            // Before player dies, need to go down until they arent revived.
                            GoDown(pb, deathCause, botShot, idWhoShot);
                        }
                    }
                }
            }
        }

        public override void ApplyFallDamage(Kit_PlayerBehaviour pb, float dmg)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                //Check for hitpoints
                if (vrd.hitPoints > 0)
                {
                    pb.deathSoundCategory = fallDamageSoundCatID;
                    if (pb.voiceManager)
                    {
                        pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                    }
                    //Set time
                    vrd.lastHit = Time.time;
                    //Set damage effect
                    vrd.hitAlpha = 2f;
                    //Apply damage
                    vrd.hitPoints -= dmg;
                    //Play voice
                    if (pb.voiceManager)
                    {
                        pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Other);
                    }
                    //Check for death
                    if (vrd.hitPoints <= 0)
                    {
                        //Reset player force
                        pb.ragdollForce = 0f;
                        // Before player dies, need to go down until they arent revived.
                        GoDown(pb, "-1");
                    }
                }
            }
        }

        public override void ApplyEnvironmentalDamage(Kit_PlayerBehaviour pb, float dmg, int deathSoundCategory)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                //Check for hitpoints
                if (vrd.hitPoints > 0)
                {
                    pb.deathSoundCategory = deathSoundCategory;
                    if (pb.voiceManager)
                    {
                        pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                    }
                    //Set time
                    vrd.lastHit = Time.time;
                    //Set damage effect
                    vrd.hitAlpha = 2f;
                    //Apply damage
                    pb.LocalDamage(dmg, 0, pb.transform.position, Vector3.zero, 0f, pb.transform.position, 0, pb.isBot, pb.id);
                    //Play voice
                    if (pb.voiceManager)
                    {
                        pb.voiceManager.DamageTaken(pb, Kit_VoiceManagerBase.DamageType.Other);
                    }
                    //Check for death
                    if (vrd.hitPoints <= 0)
                    {
                        //Reset player force
                        pb.ragdollForce = 0f;
                        // Before player dies, need to go down until they arent revived.
                        GoDown(pb, "-3");
                    }
                }
            }
        }

        public override void Suicide(Kit_PlayerBehaviour pb)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                //Reset player force
                pb.ragdollForce = 0f;
                //Call the die function on pb
                pb.Die(-3);
            }
        }

        public override void ChangeHealth(Kit_PlayerBehaviour pb, int healthNum) {
            //Create runtime data
            BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
            //Set new max health
            vrd.maxHP = healthNum;
            //Assign it
            pb.customVitalsData = vrd;
        }

        // Function that puts player health to zero, removes ui, turns screen black and white, and kills player when timer runs out.
        // hqr = Has Quick Revive
        public override void GoDown(Kit_PlayerBehaviour pb, string cause, bool botshot = false, int killer = 0, bool hqr = false) {
            // If single player and has quick revive
                // revive player
            // else (go down until revived, or player dies)
            if (botshot && (killer != 0)) {
                pb.Die(botshot, killer, cause);
            } else {
                pb.Die(int.Parse(cause));
            }
        }

        // Function that revives player while they are down amd removes all perks after they get back up.
        public override void Revive(Kit_PlayerBehaviour pb) {

        }

        public override void CustomUpdate(Kit_PlayerBehaviour pb)
        {
            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;
                //Clamp
                vrd.hitPoints = Mathf.Clamp(vrd.hitPoints, 0f, vrd.maxHP);

                //Decrease hit alpha
                if (vrd.hitAlpha > 0)
                {
                    vrd.hitAlpha -= (Time.deltaTime * 2) / simpleBloodyScreenTime;
                }

                if (pb.isFirstPersonActive)
                {
                    if (displayHealthLikeSimple)
                    {
                        //Update hud
                        pb.main.hud.DisplayHealth(vrd.hitPoints);
                        pb.main.hud.DisplayHurtState(vrd.hitAlpha);
                    }
                    else
                    {
                        //Update hud with negative values, so its hidden
                        pb.main.hud.DisplayHealth(-1);
                        //Negative values should hide it
                        pb.main.hud.DisplayHurtState(1 - vrd.hitPoints / vrd.maxHP);
                    }
                }

                //Return hit reactions
                if (hitReactionEnabled)
                {
                    pb.playerCameraHitReactionsTransform.localRotation = Quaternion.Slerp(pb.playerCameraHitReactionsTransform.localRotation, Quaternion.identity, Time.deltaTime * hitReactionsReturnSpeed);
                    pb.weaponsHitReactions.localRotation = Quaternion.Slerp(pb.weaponsHitReactions.localRotation, Quaternion.identity, Time.deltaTime * hitReactionsReturnSpeed);
                }

                if (vrd.hitPoints < vrd.maxHP)
                {
                    //Check for hp regeneration
                    if (Time.time > vrd.lastHit + timeUntilHealthIsRegenerated)
                    {
                        vrd.hitPoints += Time.deltaTime * healthRegenerationSpeed;
                    }
                }

                //Check if we are lower than death threshold
                if (pb.transform.position.y <= pb.main.mapDeathThreshold)
                {
                    pb.deathSoundCategory = outOfMapSoundCatID;
                    if (pb.voiceManager)
                    {
                        pb.deathSoundID = pb.voiceManager.GetDeathSoundID(pb, pb.deathSoundCategory);
                    }
                    pb.Die(-1);
                }
            }
        }

        public override void Setup(Kit_PlayerBehaviour pb)
        {
            //Create runtime data
            BloodyScreenVitalsRuntimeData vrd = new BloodyScreenVitalsRuntimeData();
            //Set standard values
            vrd.hitPoints = 100f; // Starting health number
            //Assign it
            pb.customVitalsData = vrd;
        }

        public override void OnPhotonSerializeView(Kit_PlayerBehaviour pb, PhotonStream stream, PhotonMessageInfo info)
        {
            BloodyScreenVitalsRuntimeData vrd = pb.customVitalsData as BloodyScreenVitalsRuntimeData;

            if (pb.customVitalsData != null && pb.customVitalsData.GetType() == typeof(BloodyScreenVitalsRuntimeData))
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(vrd.hitAlpha);
                    stream.SendNext(vrd.hitPoints);
                }
                else
                {
                    vrd.hitAlpha = (float)stream.ReceiveNext();
                    vrd.hitPoints = (float)stream.ReceiveNext();
                }
            }
            else
            {
                if (stream.IsWriting)
                {
                    stream.SendNext(0f);
                    stream.SendNext(vrd.maxHP);
                }
                else
                {
                    stream.ReceiveNext();
                    stream.ReceiveNext();
                }
            }
        }
    }
}
