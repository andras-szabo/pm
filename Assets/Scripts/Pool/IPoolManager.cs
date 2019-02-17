using UnityEngine;

public interface IPoolManager : IManager
{
	T Spawn<T>(T prototype, Vector3 position, Quaternion rotation, Transform parent) where T : MonoBehaviour;
	void Despawn<T>(T instance) where T : MonoBehaviour;
}
