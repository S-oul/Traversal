using UnityEngine;

[ExecuteAlways]
public class GroundStack : MonoBehaviour
{
    private const float SPACE_BETWEEN_TILES = 0.25f;
    private const int MAX_LEVEL = 30;

    [SerializeField][Range(0,MAX_LEVEL)]
    private int _level = 0;

    public GameObject _prefab;

    public int Level { 
        get => _level;
        set
        {
            if (_level == value) return;

            _level = value;

            OnLevelChanged(_level);
        }
    }

    void OnLevelChanged(int level)
    {
        for(int i = 0; i < level; i++)
        {
            Instantiate(_prefab,new Vector3(transform.position.x,i*SPACE_BETWEEN_TILES,transform.position.z),Quaternion.identity,transform);
        }
    }
}
