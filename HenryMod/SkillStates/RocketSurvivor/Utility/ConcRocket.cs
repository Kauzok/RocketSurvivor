﻿using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.RocketSurvivorSkills.Utility
{
	public class ConcRocket : BaseState
	{
		public override void OnEnter()
		{
			base.OnEnter();
			this.duration = ConcRocket.baseDuration / this.attackSpeedStat;
			Ray aimRay = base.GetAimRay();
			base.StartAimMode(aimRay, 3f, false);

			base.PlayAnimation("LeftArm, Override", "ShootGun", "ShootGun.playbackRate", 1.8f); //TODO: REPLACE
			Util.PlaySound(attackSoundString, base.gameObject);

			if (ConcRocket.effectPrefab)
			{
				EffectManager.SimpleMuzzleFlash(ConcRocket.effectPrefab, base.gameObject, muzzleString, false);
			}
			if (base.isAuthority)
			{
				ProjectileManager.instance.FireProjectile(ConcRocket.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * ConcRocket.damageCoefficient, ConcRocket.force, false, DamageColorIndex.Default, null, -1f);
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
			return InterruptPriority.Pain;
		}

		public static string muzzleString = "MuzzleCenter";
		public static string attackSoundString = "Play_Moffein_RocketSurvivor_R_Shoot";//"Play_MULT_m1_grenade_launcher_shoot";
		public static GameObject projectilePrefab;
		public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();	//Use a less threatening VFX for this
		public static float damageCoefficient = 0f;
		public static float force = 3000f;
		public static float baseDuration = 0.8f;

		private float duration;
	}
}