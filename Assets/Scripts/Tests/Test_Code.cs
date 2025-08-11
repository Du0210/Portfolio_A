using HDU.Managers;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Codice.Client.BaseCommands.BranchExplorer;
using Cysharp.Threading.Tasks;

public class Test_Code : MonoBehaviour
{
    void Start()
    {
        solution(new int[3] { 93, 30, 55 }, new int[3] { 1, 30, 5 }).Forget();

    }

    public async UniTask solution(int[] progresses, int[] speeds)
    {
        float timer = 5f;
        int[] answer = new int[] { };
        int check = 0;
        Queue<(int index, int percent)> pQueue = new Queue<(int, int)>();
        List<int> dList = new List<int>();

        for (int i = 0; i < progresses.Length; i++)
            pQueue.Enqueue((i, progresses[i]));

        while (pQueue.Count > 0)
        {
            int distributeCount = 0;
            int loopCount = pQueue.Count;
            for (int i = 0; i < loopCount; i++)
            {
                if (pQueue.Count <= 0)
                    break;
                (int index, int percent) progress = pQueue.Dequeue();
                progress.percent += speeds[progress.index];

                if (progress.percent >= 100 && check == progress.index)
                {
                    check++;
                    distributeCount++;
                }
                else
                    pQueue.Enqueue(progress);
            }

            if (distributeCount != 0)
            {
                dList.Add(distributeCount);
            }

            if (timer < 0)
                break;
            timer -= Time.deltaTime;
            await UniTask.Yield();
        }


        Debug.Log(dList.ToArray());
    }
}
