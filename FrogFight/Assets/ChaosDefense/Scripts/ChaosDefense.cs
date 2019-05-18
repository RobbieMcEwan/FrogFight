using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChaosDefense : MonoBehaviour
{

	[Header("Properties")]
	public int StartHp = 5;
	public float CurveDuration = 60f;

	public AnimationCurve DifficultyCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public Vector2 EnemiesPerSecond = new Vector2(0.5f, 3f);
	public Vector2 EnemySpeed = new Vector2(1f, 2f);
	public float EnemySpawnRadius = 1f;

	[Header("Status")]
	public bool IsActivated = false;
	public int CurrentHp;	
	[Range(0f, 1f)]

	public float TimeSinceStart = 0f;

	[Header("Required References")]
	public Collider EnemyObjective;
	public Object[] EnemyPrefabs;

	[Header("Events")]
	public UnityEvent OnHpLoss;

	public UnityEvent OnGameStart;
	public UnityEvent OnGameOver;

	private float _startTime;
	private float _nextSpawnTime;
	private List<ChaosEnemy> _enemies;

	public void Awake()
	{
        Debug.Log("Activate ChaosDefense");

		_enemies = new List<ChaosEnemy>();
		RestoreHp();

        Activate();
	}

	public void RestoreHp()
	{
		CurrentHp = StartHp;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.F12)) Activate();
		
		if (IsActivated) {
			RunWave();
			HitEnemies();
		}
	}

	private void RunWave()
	{
		TimeSinceStart += Time.deltaTime;
		if (!(Time.time > _nextSpawnTime)) return;
		
		SpawnEnemy();
		_nextSpawnTime = Time.time + (1f / DifficultyLerp(EnemiesPerSecond, TimeSinceStart));
	}

	private void HitEnemies()
	{
		#if UNITY_EDITOR
		if (!Input.GetMouseButtonDown(0)) return;
		var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		#else
        if(Input.touchCount <= 0) return;
		var mainTouch = Input.GetTouch(0);
		var ray = Camera.main.ScreenPointToRay(mainTouch.position);
		#endif
		
		if (!Physics.Raycast(ray, out var hit, float.MaxValue)) return;
		var hitEnemy = hit.collider.GetComponent<ChaosEnemy>();
		if (hitEnemy == null) return;

		hitEnemy.Die();
		_enemies.Remove(hitEnemy);
	}

	public void Activate()
	{
		if (IsActivated) return;
		
		IsActivated = true;
		_startTime = Time.time;
		_nextSpawnTime = Time.time + (1f / DifficultyLerp(EnemiesPerSecond, TimeSinceStart));
		OnGameStart.Invoke();
	}

	private float DifficultyLerp(Vector2 factor, float time)
	{
		return Mathf.Lerp(factor.x, factor.y, DifficultyCurve.Evaluate(time / CurveDuration));
	}

	public void SpawnEnemy()
	{
		var newEnemy = (Instantiate(EnemyPrefabs[Random.Range(0, EnemyPrefabs.Length)]) as GameObject)?.GetComponent<ChaosEnemy>();
		if (newEnemy == null) return;

		newEnemy.transform.position = Random.onUnitSphere * EnemySpawnRadius;
		if (newEnemy.transform.position.y < 0f) {
			var pos = newEnemy.transform.position;
			pos.y *= -1f;
			newEnemy.transform.position = pos;
		}
		
		_enemies.Add(newEnemy);
		newEnemy.Initialize(this, DifficultyLerp(EnemySpeed, TimeSinceStart));
	}

	public void EnemyHit(ChaosEnemy source)
	{
		CurrentHp--;
		OnHpLoss.Invoke();
		
		_enemies.Remove(source);
		Destroy(source.gameObject);

		if (CurrentHp <= 0) {
			EndGame();
		}
	}

	public void EndGame()
	{
		for (var i = _enemies.Count - 1; i >= 0; i--) {
			Destroy(_enemies[i].gameObject);
		}

		_enemies.Clear();
		
		OnGameOver.Invoke();

		IsActivated = false;
	}
}
