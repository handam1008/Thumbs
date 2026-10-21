using UnityEngine;

namespace WSH._09.SO
{
    [CreateAssetMenu(fileName = "CorrectPuzzleDataSO", menuName = "SO/PuzzleDataSO")]
    public class PuzzleDataSO : ScriptableObject
    {
        public float[] puzzleData = new float[20];
    }
}
