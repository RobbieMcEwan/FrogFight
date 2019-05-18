using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChaosEnemy : MonoBehaviour
{

	public ChaosDefense Parent;

	public float Speed;

	public UnityEvent OnDie;
	
	private bool _initialized;
	
	public void Initialize(ChaosDefense parent, float speed)
	{
		Parent = parent;
		Speed = speed;
		_initialized = true;
	}

	public void Update()
	{
		if (!_initialized) return;

		transform.LookAt(Parent.EnemyObjective.transform.position, Vector3.up);
		transform.Translate(Vector3.forward * Speed * Time.deltaTime, Space.Self);
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other == Parent.EnemyObjective) {
			Parent.EnemyHit(this);
		}
	}

	public void Die()
	{
		Speed = 0f;
		OnDie.Invoke();
		Destroy(gameObject);
	}
	
}
