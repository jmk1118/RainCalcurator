using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionSummoner : MonoBehaviour
{
    [SerializeField] GameObject GameManagerMain; // 메인 게임 매니저 오브젝트
    [SerializeField] float summonTime; // 최대 소환시간. 최소 소환시간은 최대의 1/2
    [SerializeField] GameObject questionPrefab; // 문제 프리팹
    [SerializeField] GameObject deadLine; // 문제가 닿으면 사라지는 최종 라인 오브젝트
    List<GameObject> questions; // 문제 리스트
    Queue<GameObject> stayQuestions; // 대기 문제 큐

    void Awake()
    {
        questions = new List<GameObject>();
        stayQuestions = new Queue<GameObject>();
    }

    private void Start()
    {
        GameObject quest;
        for(int i = 0; i < 20; i++) // 20개의 문제 오브젝트를 미리 생성하여 큐에 쌓아둔다
        {
            quest = Instantiate(questionPrefab, this.transform);
            quest.GetComponent<Question>().Summoner = this.gameObject;
            quest.GetComponent<Question>().QuestionIndex = i;
            quest.GetComponent<Question>().DeadLine = deadLine;
            stayQuestions.Enqueue(quest);
        }
        StartCoroutine("SummonQuestion"); // 문제들을 소환하는 코루틴
    }

    // 문제들을 소환하는 코루틴
    IEnumerator SummonQuestion()
    {
        GameObject quest;
        questions.Add(stayQuestions.Dequeue());
        // summonTime과 그 반 사이의 시간대로 랜덤하게 문제를 소환
        while (true)
        {
            quest = stayQuestions.Dequeue();
            quest.transform.position = this.transform.position;
            quest.SetActive(true);
            questions.Add(quest);
            yield return new WaitForSeconds(Random.Range(summonTime / 2, summonTime));
        }
    }

    // 유저에게 입력받은 수가 정답인지 확인하는 메소드
    public void CheckAllQuestions(int userAnswer)
    {
        for(int i = 0; i < questions.Count; i++)
        {
            if(questions[i].GetComponent<Question>().Check(userAnswer))
            {
                questions[i].SetActive(false);
                stayQuestions.Enqueue(questions[i]);
                questions.RemoveAt(i);
                GameManagerMain.GetComponent<GameManagerMain>().GetPoint();
                break;
            }
        }
    }

    // 완전히 떨어진 문제 오브젝트를 다시 대기 상태로 만드는 메소드
    public void PoolingQuestion(int index)
    {
        for (int i = 0; i < questions.Count; i++)
        {
            if (questions[i].GetComponent<Question>().QuestionIndex == index)
            {
                questions[i].SetActive(false);
                stayQuestions.Enqueue(questions[i]);
                questions.RemoveAt(i);
                GameManagerMain.GetComponent<GameManagerMain>().LosePoint();
                break;
            }
        }
    }
}
