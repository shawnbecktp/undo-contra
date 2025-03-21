﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class UndoZai : Enemy {

	public float agroDistance = 5f;
	public Material[] mats;
	public Animator anim;
	public Transform enemyShot;
	public List<Transform> attackSpawnPoints = new List<Transform>();
	public float shotDelay = 3f;
	public float moveSpeed = 1f;
	public float attackAnimationTime = 1f;
	public SkinnedMeshRenderer rend;
	public Transform despawnEffect;

	// Use this for initialization
	void OnEnable () {
		if ( rend ) rend.material = mats[Random.Range(0,mats.Length)];
		curHp = this.maxHp;
		StartCoroutine( AIRoutine() );
	}

	IEnumerator AIRoutine() {

		anim.Play("Spawning");

		yield return new WaitForSeconds(1.0f);

		anim.Play("Run");
		while( this.transform.position.z > 1f ) {

			this.transform.Translate( Vector3.forward * Time.deltaTime * moveSpeed );

			yield return null;
		}

		StartCoroutine(LookAtRoutine());
		anim.Play("Idle");
		yield return new WaitForSeconds( 1.5f);

		while ( true ) {

			yield return null;

			if ( Vector3.Distance( this.transform.position, PlayerController.Instance.root.position ) < agroDistance ) {
				anim.Play("Attack");
			}
			yield return new WaitForSeconds( attackAnimationTime );
			anim.Play("Idle");

			yield return new WaitForSeconds( shotDelay );

		}

	}


 	public void Attack(int attackPoint) {
		
		Transform t = PoolManager.Pools["Attacks"].Spawn( enemyShot, attackSpawnPoints[attackPoint].position, attackSpawnPoints[attackPoint].rotation );
		t.LookAt( PlayerController.Instance.center );

		PoolManager.Pools["Attacks"].Spawn("RedShotMuzzle", attackSpawnPoints[attackPoint].position, attackSpawnPoints[attackPoint].rotation ); 
	}

	public override void TakeHit( int damage, Vector3 point ) {

		FlashCharacter();

		curHp -= Mathf.Clamp (damage, 0, 100000);

		if ( curHp <= 0 ) {
			Death(); 
		}

		if ( this.transform.CompareTag("Player")) ScreenFlashManager.Instance.FlashRed();
	}

	IEnumerator LookAtRoutine() {
		while ( true ) {
			yield return null;
			this.transform.LookAt( PlayerController.Instance.center );
		}
	}

	public override void Death() {
		if ( despawnEffect ) PoolManager.Pools["Attacks"].Spawn(despawnEffect, this.transform.position, Quaternion.identity );
		UndoZaiSpawner.Instance.Despawn( this.transform );
	}


}
