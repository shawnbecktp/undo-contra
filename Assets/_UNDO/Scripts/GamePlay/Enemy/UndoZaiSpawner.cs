﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class UndoZaiSpawner : MonoBehaviour {

	public Transform undoZai;
	public float spawnRange = 15f;
	public float spawnDelay = 3f;
	int spawned = 0;
	public int maxSpawns = 0;

	List<Transform> spawnedUndoZais = new List<Transform>();

	private static UndoZaiSpawner _instance;
	public static UndoZaiSpawner Instance {
		get {
			return _instance;
		}
	}

	void Awake() {

		if ( _instance == null ) _instance = this;
		else if ( _instance != this ) Destroy( this );
	}

	void OnEnable() {
		StartCoroutine( SpawnRoutine() );	
	}

	IEnumerator SpawnRoutine() {
		while(true) {
			yield return new WaitForSeconds(spawnDelay);
			if ( spawned < maxSpawns ) Spawn();
		}
	}



	void Spawn() {
		Transform t = PoolManager.Pools["UndoZai"].Spawn( undoZai, new Vector3( Random.Range(-spawnRange,spawnRange),0,6f), this.transform.rotation );

		spawnedUndoZais.Add(t);
		spawned++;
	}

	public void StopSpawning() {
		StopAllCoroutines();
		for ( int i = spawnedUndoZais.Count-1; i >= 0; i-- ) {
			Despawn( spawnedUndoZais[i] );
		}
	}

	public void Despawn( Transform t ) {
		UndoZaiSpawner.Instance.spawned--;
		PoolManager.Pools["UndoZai"].Despawn(t );
		spawnedUndoZais.Remove(t);
	}


}
