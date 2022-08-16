﻿using EntityStates.RocketSurvivorSkills.Primary;
using RocketSurvivor.Modules.Survivors;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Special
{
    public class FireAllRockets : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            fireStopwatch = 0f;
            delayBetweenShots = FireAllRockets.baseDelayBetweenShots / base.attackSpeedStat;
            shotsRemaining = FireAllRockets.baseShotCount;    //Skill felt underwhelming when it was tied to primary stocks: only useful when primary is fully loaded, which is the opposite of the skill's intended purpose.
            isCrit = base.RollCrit();

            selectedPrimarySkill = RocketSurvivorSetup.FireRocketSkillDef;
            if (base.skillLocator)
            {
                if (base.skillLocator.primary.skillDef == RocketSurvivorSetup.FireRocketAltSkillDef)
                {
                    selectedPrimarySkill = RocketSurvivorSetup.FireRocketAltSkillDef;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (fireStopwatch <= 0f)
            {
                fireStopwatch += delayBetweenShots;
                if(shotsRemaining > 0) FireProjectile();
            }
            fireStopwatch -= Time.fixedDeltaTime;

            if (base.isAuthority)
            {
                if (base.skillLocator)
                {
                    if (shotsRemaining <= 0)//(base.skillLocator.primary.stock <= 0)
                    {
                        this.outer.SetNextState(new Rearm());
                        return;
                    }
                }
            }
        }

        public void FireProjectile()
        {
            shotsRemaining--;
            if (base.isAuthority)
            {
                /*if (base.skillLocator && base.skillLocator.primary.stock > 0)
                {
                    base.skillLocator.primary.DeductStock(1);
                }*/

                Ray aimRay = base.GetAimRay();
                base.StartAimMode(aimRay, 3f, false);

                //Set force to 0? Knockback makes it pretty bad against small enemies.
                ProjectileManager.instance.FireProjectile(GetProjectilePrefab(), aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * GetDamageCoefficient(), GetForce() * 0.25f, isCrit, DamageColorIndex.Default, null, -1f);
            }

            if (FireRocket.effectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(GetEffectPrefab(), base.gameObject, GetMuzzleString(), false);
            }
            base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f); //TODO: REPLACE
            Util.PlaySound(FireRocket.attackSoundString, base.gameObject);
        }

        private GameObject GetProjectilePrefab()
        {
            if (selectedPrimarySkill == RocketSurvivorSetup.FireRocketAltSkillDef)
            {
                return FireRocketAlt.projectilePrefab;
            }
            else
            {
                return FireRocket.projectilePrefab;
            }
        }

        private GameObject GetEffectPrefab()
        {
            if (selectedPrimarySkill == RocketSurvivorSetup.FireRocketAltSkillDef)
            {
                return FireRocketAlt.effectPrefab;
            }
            else
            {
                return FireRocket.effectPrefab;
            }
        }

        private string GetMuzzleString()
        {
            if (selectedPrimarySkill == RocketSurvivorSetup.FireRocketAltSkillDef)
            {
                return FireRocketAlt.muzzleString;
            }
            else
            {
                return FireRocket.muzzleString;
            }
        }


        private float GetDamageCoefficient()
        {
            if (selectedPrimarySkill == RocketSurvivorSetup.FireRocketAltSkillDef)
            {
                return FireRocketAlt.damageCoefficient;
            }
            else
            {
                return FireRocket.damageCoefficient;
            }
        }

        private float GetForce()
        {
            if (selectedPrimarySkill == RocketSurvivorSetup.FireRocketAltSkillDef)
            {
                return FireRocketAlt.force;
            }
            else
            {
                return FireRocket.force;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        private SkillDef selectedPrimarySkill;
        private bool isCrit;
        private float fireStopwatch;
        private float delayBetweenShots;

        private int shotsRemaining;

        public static int baseShotCount = 4;
        public static float baseDelayBetweenShots = 0.2f;
    }
}