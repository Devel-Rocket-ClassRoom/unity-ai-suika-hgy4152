using UnityEngine;

[CreateAssetMenu(fileName = "FruitConfig", menuName = "WatermelonGame/FruitConfig")]
public class FruitConfig : ScriptableObject
{
    [System.Serializable]
    public class FruitEntry
    {
        public string fruitName;
        public Sprite sprite;
        [Tooltip("World-space radius")]
        public float radius = 0.5f;
        public int scoreValue;
    }

    public FruitEntry[] fruits = new FruitEntry[11];

    public FruitEntry Get(int level) => fruits[Mathf.Clamp(level - 1, 0, fruits.Length - 1)];

    public bool IsValidLevel(int level) => level >= 1 && level <= fruits.Length;

    public int MaxLevel => fruits.Length;
}
