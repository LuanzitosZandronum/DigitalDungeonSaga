using UnityEngine;

public class Collision : MonoBehaviour
{
	public enum Type
	{
		None,
		Wall,
		Door
    }
	
	public Type collisionType = Type.Wall;

    public Type GetCollisionType()
    {
		return collisionType;
	}
}
