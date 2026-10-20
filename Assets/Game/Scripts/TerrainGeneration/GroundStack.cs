using UnityEditor.Animations;
using UnityEngine;

[ExecuteAlways]
public class GroundStack : MonoBehaviour
{
    private const float SPACE_BETWEEN_TILES = 0.25f;

    [SerializeField][Range(0,4)]
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

    void Awake()
    {
        if(transform.childCount > 1) return;

        for(int i = 0; i < 5; i++)
        {
            Instantiate(_prefab,new Vector3(0,i*SPACE_BETWEEN_TILES,0),Quaternion.identity,transform).SetActive(i <= _level? true: false);
        }
    }
    void OnLevelChanged(int level)
    {
        for(int i = 0; i < 5; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i <= level? true: false);
        }
    }
}
