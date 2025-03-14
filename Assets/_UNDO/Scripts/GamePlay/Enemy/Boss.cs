﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class Boss : Enemy {

	[Header("Data")]
	public Animator anim;
	public PlayerController target;
	public enum BossBehaviour {Shuffle, Shoot, Rocket, QuickRocket}
	public BossBehaviour bossBehaviour = BossBehaviour.Shuffle;
	BossBehaviour lastBossBehaviour = BossBehaviour.Shuffle;
	float aiSpeed = 1f;

	[Header("Awareness")]
	public float closeDistance = 10f;
	public float farDistance = 100f;
	enum TargetDistance {Close, Far}
	TargetDistance targetDistance = TargetDistance.Close;

	[Header("Shuffling")]
	public Transform[] shufflingPoints;
	int lastShufflePointIndex = 0;
	public int shuffleCount = 10;
	public float shuffleSpeed = 10f;
	public float shuffleDelay = 0.2f;

	[Header("Shooting")]
	public Transform enemyShot;
	public Transform[] attackSpawnPoints;
	public int shots = 10;
	public float shotInterval = 0.2f;
	public float shotSpread = 0.2f;

	[Header("Rocket Swarm & QuickRockets")]
	public Transform[] rocketSpawnPoints;
	public Transform homingRocket;
	public float rocketShotInterval = 0.2f;
	public Transform rocketSwarmPosition;

	void OnEnable() {
		Invoke ("CheckTarget", 1.0f);
	}

	/* ==============================================================================================================
	 * AI MAIN
	 * ============================================================================================================ */

	void CheckTarget() {
		if ( Vector3.Distance( this.transform.position, target.root.position) < closeDistance ) targetDistance = TargetDistance.Close;
		else targetDistance = TargetDistance.Far;

		// Set new boss behaviour
		do { bossBehaviour = (BossBehaviour) Random.Range(0,System.Enum.GetValues(typeof(BossBehaviour)).Length); }
		while ( lastBossBehaviour == bossBehaviour || (this.curHp > maxHp/2 && bossBehaviour == BossBehaviour.Rocket ));
//		while ( lastBossBehaviour == bossBehaviour );

		lastBossBehaviour = bossBehaviour;

		switch( targetDistance ) {
			// CLOSE
			case TargetDistance.Close :
			
				switch ( bossBehaviour ) {
//				case BossBehaviour.Idle: StartIdle(); break;
				case BossBehaviour.Shuffle: StartShuffle(); break;
				case BossBehaviour.Shoot: StartShoot(); break;
				case BossBehaviour.Rocket: StartRocketSwarm(); break;
				case BossBehaviour.QuickRocket: StartQuickRocket(); break;
				}

			break;

			// FAR
			case TargetDistance.Far :
			
				switch ( bossBehaviour ) {
//				case BossBehaviour.Idle: StartIdle(); break;
				case BossBehaviour.Shuffle: StartShuffle(); break;
				case BossBehaviour.Shoot: StartShoot(); break;
				case BossBehaviour.Rocket: StartRocketSwarm(); break;
				case BossBehaviour.QuickRocket: StartQuickRocket(); break;
				}

			break;
		}

	}

	/* ==============================================================================================================
	 * Idling
	 * ============================================================================================================ */

	void StartIdle() {
		StartCoroutine(IdleRoutine());
	}

	IEnumerator IdleRoutine() {
		yield return new WaitForSeconds( 1f / aiSpeed );
		EndAIRoutine();
	}


	/* ==============================================================================================================
	 * Shuffling
	 * ============================================================================================================ */

	void StartShuffle() {
		StartCoroutine(ShuffleRoutine());
	}

	IEnumerator ShuffleRoutine() {
		int _shuffleCount = shuffleCount;
		int _shufflePointIndex = 0;

		// Get new shuffle point
		do { _shufflePointIndex = Random.Range(0,shufflingPoints.Length); }
		while ( _shufflePointIndex == lastShufflePointIndex );

		AudioManager.Instance.Play("event:/BossShuffle",this.transform.position);

		while ( _shuffleCount > 0 ) {

			this.transform.position = Vector3.MoveTowards( this.transform.position, shufflingPoints[_shufflePointIndex].position, Time.deltaTime * shuffleSpeed );
			this.transform.LookAt( target.root );

			yield return null;

			if ( this.transform.position == shufflingPoints[_shufflePointIndex].position ) {
				// Get new shuffle point
				do { _shufflePointIndex = Random.Range(0,shufflingPoints.Length); }
				while ( _shufflePointIndex == lastShufflePointIndex );
				_shuffleCount--;

				if ( _shuffleCount > 0 ) AudioManager.Instance.Play("event:/BossShuffle",this.transform.position);

				yield return new WaitForSeconds( shuffleDelay / aiSpeed );
			}
		}

		yield return new WaitForSeconds( 1f / aiSpeed);

		EndAIRoutine();

	}

	/* ==============================================================================================================
	 * Shoot
	 * ============================================================================================================ */

	void StartShoot() {
		StartCoroutine( ShootRoutine());
	}

	IEnumerator ShootRoutine() {

		yield return new WaitForSeconds( 2f / aiSpeed );

		this.transform.LookAt( target.center );

		for ( int i = 0; i < shots; i++ ) {
			Transform t1 = PoolManager.Pools["Attacks"].Spawn( enemyShot, attackSpawnPoints[0].position, attackSpawnPoints[0].rotation );
			Transform t2 = PoolManager.Pools["Attacks"].Spawn( enemyShot, attackSpawnPoints[1].position, attackSpawnPoints[1].rotation );

			t1.LookAt( target.center );
			t2.LookAt( target.center );

			t1.Rotate( new Vector3 (Random.Range(-shotSpread,shotSpread),Random.Range(-shotSpread,shotSpread),0f) );
			t2.Rotate( new Vector3 (Random.Range(-shotSpread,shotSpread),Random.Range(-shotSpread,shotSpread),0f) );

			PoolManager.Pools["Attacks"].Spawn("RedShotMuzzle", attackSpawnPoints[0].position, attackSpawnPoints[0].rotation );
			PoolManager.Pools["Attacks"].Spawn("RedShotMuzzle", attackSpawnPoints[1].position, attackSpawnPoints[1].rotation );

			AudioManager.Instance.Play("event:/BossShot",this.transform.position );

			yield return new WaitForSeconds( shotInterval );

			this.transform.LookAt( target.center );

		}

		yield return new WaitForSeconds( 1f / aiSpeed);

		StartShuffle ();

		//EndAIRoutine();

	}

	/* ==============================================================================================================
	 * Rocket Swarm
	 * ============================================================================================================ */

	void StartRocketSwarm() {
		StartCoroutine( RocketSwarmRoutine());
	}

	IEnumerator RocketSwarmRoutine() {

		this.transform.LookAt( target.center );

		AudioManager.Instance.Play ("event:/Alert", Vector3.zero);

		while ( this.transform.position != rocketSwarmPosition.position ) {

			this.transform.position = Vector3.MoveTowards( this.transform.position, rocketSwarmPosition.position, Time.deltaTime * shuffleSpeed );
			yield return null;
		}

		this.transform.LookAt( target.center );
		this.anim.Play("Ultimate");
		AudioManager.Instance.Play ("event:/SuperCharge", this.transform.position);

		yield return new WaitForSeconds( 2.8f);

		for ( int i = 0; i < rocketSpawnPoints.Length; i++ ) {
			Transform t = PoolManager.Pools["Attacks"].Spawn( homingRocket, rocketSpawnPoints[i].position, rocketSpawnPoints[i].rotation );
			t.GetComponent<HomingRocket>().StartHoming( target.root );
			PoolManager.Pools["Attacks"].Spawn("RedShotMuzzle", rocketSpawnPoints[i].position, rocketSpawnPoints[i].rotation );
			AudioManager.Instance.Play("event:/Launch",this.transform.position );
			yield return new WaitForSeconds( rocketShotInterval );

			this.transform.LookAt( target.center );

		}

		yield return new WaitForSeconds( 3f);

		EndAIRoutine();
	}

	/* ==============================================================================================================
	 * Quick Rocket
	 * ============================================================================================================ */

	void StartQuickRocket() {
		StartCoroutine( QuickRocketRoutine());
	}

	IEnumerator QuickRocketRoutine() {

		yield return new WaitForSeconds( 0.5f / aiSpeed);

		this.transform.LookAt( target.center );

		for ( int i = 0; i < 4; i++ ) {
			Transform t = PoolManager.Pools["Attacks"].Spawn( homingRocket, rocketSpawnPoints[i].position, rocketSpawnPoints[i].rotation );
			t.GetComponent<HomingRocket>().StartHoming( target.root );
			PoolManager.Pools["Attacks"].Spawn("RedShotMuzzle", rocketSpawnPoints[i].position, rocketSpawnPoints[i].rotation );
			AudioManager.Instance.Play("event:/Launch",this.transform.position );
			yield return new WaitForSeconds( rocketShotInterval/2f );

			this.transform.LookAt( target.center );

		}

		yield return new WaitForSeconds( 1f / aiSpeed);

		EndAIRoutine();
	}

	/* ==============================================================================================================
	 * End Routine
	 * ============================================================================================================ */

	void EndAIRoutine() {
		if (curHp < maxHp / 2f) aiSpeed = 2f;
		CheckTarget();
	}

	public override void TakeHit( int damage, Vector3 point ) {
		FlashCharacter();
		base.TakeHit( damage,point);

	}

	public override void Death() {
		_isDead = true;
		StopAllCoroutines ();
		FlashCharacter();
		CombatManager.Instance.EndGame();
	}
}
