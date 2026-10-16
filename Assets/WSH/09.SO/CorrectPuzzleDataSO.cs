using UnityEngine;

namespace WSH._09.SO
{
    [CreateAssetMenu(fileName = "CorrectPuzzleDataSO", menuName = "SO/CorrectPuzzleDataSO")]
    public class CorrectPuzzleDataSO : ScriptableObject
    {
        public bool[] correctPuzzleData = new bool[15];
    }
}
