
using System.Collections;
using UnityEngine;

public class QLearningAgent : MonoBehaviour
{
    [Header("Training")]
    public int episodes = 100;
    public int maxStepsPerEpisode = 100;
    public float trainingStepDelay = 0.02f;

    [Header("Learning")]
    public float learningRate = 0.1f;
    public float discountFactor = 0.95f;
    public float epsilon = 1f;
    public float minimumEpsilon = 0.05f;
    public float epsilonDecay = 0.98f;

    [Header("Display")]
    public float moveInterval = 0.4f;

    private GridManager grid;
    private Vector2Int currentPosition;
    private float[,,] qTable;

    private int currentEpisode;
    private int currentEpisodeStep;
    private int totalSteps;
    private int successfulEpisodes;

    private string status = "Waiting...";
    private bool trainingFinished;
    private bool pathFinished;

    private readonly Vector2Int[] directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    public void Initialize(
        GridManager gridManager,
        Vector2Int startPosition)
    {
        grid = gridManager;
        currentPosition = startPosition;

        qTable = new float[grid.width, grid.height, 4];

        transform.position = grid.GetWorldPosition(currentPosition);

        StartCoroutine(TrainVisibly());
    }

    private IEnumerator TrainVisibly()
    {
        status = "Training";
        Debug.Log("ECHO: Training started!");

        float currentEpsilon = epsilon;

        for (int episode = 0; episode < episodes; episode++)
        {
            currentEpisode = episode + 1;
            currentEpisodeStep = 0;

            currentPosition = grid.startPosition;
            transform.position = grid.GetWorldPosition(currentPosition);

            yield return new WaitForSeconds(0.1f);

            bool reachedGoal = false;

            for (int step = 0; step < maxStepsPerEpisode; step++)
            {
                currentEpisodeStep = step + 1;

                if (grid.IsGoal(currentPosition))
                {
                    reachedGoal = true;
                    break;
                }

                int action = ChooseAction(
                    currentPosition,
                    currentEpsilon
                );

                if (action < 0)
                    break;

                Vector2Int nextPosition =
                    currentPosition + directions[action];

                float reward = grid.IsGoal(nextPosition)
                    ? 10f
                    : -0.04f;

                float oldQ =
                    qTable[currentPosition.x, currentPosition.y, action];

                float nextMaxQ = grid.IsGoal(nextPosition)
                    ? 0f
                    : GetMaxQ(nextPosition);

                qTable[currentPosition.x, currentPosition.y, action] =
                    oldQ + learningRate *
                    (reward + discountFactor * nextMaxQ - oldQ);

                currentPosition = nextPosition;
                transform.position = grid.GetWorldPosition(currentPosition);
                totalSteps++;

                if (grid.IsGoal(currentPosition))
                {
                    reachedGoal = true;
                    break;
                }

                yield return new WaitForSeconds(trainingStepDelay);
            }

            if (reachedGoal)
                successfulEpisodes++;

            currentEpsilon = Mathf.Max(
                minimumEpsilon,
                currentEpsilon * epsilonDecay
            );

            if (currentEpisode % 25 == 0)
            {
                Debug.Log(
                    "ECHO: Episode " + currentEpisode +
                    "/" + episodes +
                    " | Success rate: " +
                    GetSuccessRate().ToString("F1") + "%"
                );
            }
        }

        trainingFinished = true;
        status = "Training completed";

        Debug.Log(
            "ECHO: Training finished! Success rate: " +
            GetSuccessRate().ToString("F1") + "%"
        );

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(ShowLearnedPath());
    }

    private IEnumerator ShowLearnedPath()
    {
        status = "Showing learned path";

        currentPosition = grid.startPosition;
        transform.position = grid.GetWorldPosition(currentPosition);

        yield return new WaitForSeconds(0.5f);

        for (int step = 0; step < maxStepsPerEpisode; step++)
        {
            if (grid.IsGoal(currentPosition))
            {
                status = "Goal reached!";
                pathFinished = true;
                Debug.Log("ECHO: Goal reached!");
                yield break;
            }

            int action = GetBestAction(currentPosition);

            if (action < 0)
                break;

            Vector2Int nextPosition =
                currentPosition + directions[action];

            if (!grid.IsWalkable(nextPosition))
                break;

            currentPosition = nextPosition;
            transform.position = grid.GetWorldPosition(currentPosition);

            yield return new WaitForSeconds(moveInterval);
        }

        if (!grid.IsGoal(currentPosition))
            status = "Path ended - try more training";

        pathFinished = true;
    }

    private int ChooseAction(Vector2Int position, float exploration)
    {
        int[] valid = GetValidActions(position);

        if (valid.Length == 0)
            return -1;

        if (Random.value < exploration)
            return valid[Random.Range(0, valid.Length)];

        return GetBestAction(position);
    }

    private int GetBestAction(Vector2Int position)
    {
        int[] valid = GetValidActions(position);

        if (valid.Length == 0)
            return -1;

        float bestValue = float.NegativeInfinity;
        int[] bestActions = new int[4];
        int bestCount = 0;

        foreach (int action in valid)
        {
            float value =
                qTable[position.x, position.y, action];

            if (value > bestValue + 0.0001f)
            {
                bestValue = value;
                bestCount = 0;
                bestActions[bestCount++] = action;
            }
            else if (Mathf.Abs(value - bestValue) <= 0.0001f)
            {
                bestActions[bestCount++] = action;
            }
        }

        return bestActions[Random.Range(0, bestCount)];
    }

    private float GetMaxQ(Vector2Int position)
    {
        int[] valid = GetValidActions(position);

        if (valid.Length == 0)
            return 0f;

        float maxValue = float.NegativeInfinity;

        foreach (int action in valid)
        {
            float value =
                qTable[position.x, position.y, action];

            if (value > maxValue)
                maxValue = value;
        }

        return maxValue;
    }

    private int[] GetValidActions(Vector2Int position)
    {
        int[] buffer = new int[4];
        int count = 0;

        for (int action = 0; action < directions.Length; action++)
        {
            Vector2Int next = position + directions[action];

            if (grid.IsWalkable(next))
                buffer[count++] = action;
        }

        int[] result = new int[count];
        System.Array.Copy(buffer, result, count);

        return result;
    }

    private float GetSuccessRate()
    {
        if (currentEpisode == 0)
            return 0f;

        return (float)successfulEpisodes / currentEpisode * 100f;
    }

    private void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 290, 190), "ECHO-Lite | AI Dashboard");

        GUILayout.BeginArea(new Rect(25, 40, 265, 155));

        GUILayout.Label("Status: " + status);
        GUILayout.Label(
            "Episode: " + currentEpisode + " / " + episodes
        );
        GUILayout.Label(
            "Successful episodes: " + successfulEpisodes
        );
        GUILayout.Label(
            "Success rate: " + GetSuccessRate().ToString("F1") + "%"
        );
        GUILayout.Label("Training steps: " + totalSteps);

        if (trainingFinished)
            GUILayout.Label("Training: 100%");

        GUILayout.EndArea();
    }
}
