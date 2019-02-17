public interface ICollisionManager : IManager
{
	void ReportHit(UnityEngine.GameObject hitObject, int damage);
	void Register(UnityEngine.GameObject gameObject, ICollidable collidable);
	void Unregister(UnityEngine.GameObject gameObject);
}