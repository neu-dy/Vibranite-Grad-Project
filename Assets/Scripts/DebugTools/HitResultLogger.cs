using UnityEngine;
using Vibranite.RhythmGame;

namespace Vibranite.DebugTools
{
    /// <summary>
    /// Prints every judged hit to the Console. Placeholder for real feedback.
    /// </summary>
    public class HitResultLogger : MonoBehaviour
    {
        [SerializeField] private HitEvaluator evaluator;

        private void OnEnable()
        {
            evaluator.HitJudged += LogResult;
        }

        private void OnDisable()
        {
            evaluator.HitJudged -= LogResult;
        }

        private void LogResult(HitResult result)
        {
            string verdict = result.IsSuccess ? "SUCCESS" : "FAIL";
            Debug.Log($"[Hit] {verdict} - {result}");
        }
    }
}
