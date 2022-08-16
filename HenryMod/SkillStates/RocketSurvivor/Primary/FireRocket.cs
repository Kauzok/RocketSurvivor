﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Primary
{
    public class FireRocket : BaseState
    {
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = FireRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f);	//TODO: REPLACE
			Util.PlaySound(attackSoundString, base.gameObject);

			if (FireRocket.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(FireRocket.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(FireRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * FireRocket.damageCoefficient, FireRocket.force, base.RollCrit(), DamageColorIndex.Default, null, -1f);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.PrioritySkill;
		}

		public static string muzzleString = "MuzzleCenter";
		public static string attackSoundString = "Play_Moffein_RocketSurvivor_M1_Shoot";
		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
		public static float damageCoefficient = 5.2f;
		public static float force = 2000f;
		public static float baseDuration = 0.8f;

		private float duration;
	}
}