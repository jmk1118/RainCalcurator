using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject startPanel; // 시작 화면
    [SerializeField] GameObject mainPanel; // 메인 화면
    [SerializeField] GameObject optionPanel; // 옵션 화면
    [SerializeField] GameObject pointCheckPanel; // 점수 확인 화면
    [SerializeField] GameObject pausePanel; // 일시정지 화면
    [SerializeField] GameObject gameOverPanel; // 게임오버 화면
    bool isPause; // 일시정지 상태
    string logPath; // 최고 점수가 저장되는 로트 파일 위치
    Stream fileStream; // 파일스트림
    byte[] fileByte = new byte[4]; // 파일에 최고점수를 읽고쓰기위한 바이트 배열
    int maxPointPlus; // 덧셈 최고점수
    int maxPointMinus; // 뺄셈 최고점수
    int maxPointX; // 곱셈 최고점수
    int maxPointDiv; // 나눗셈 최고점수 
    int maxPointAll; // 전부 최고점수

    int option;
    public bool Plus { get; set; } // 덧셈 출현 불린값
    public bool Minus { get; set; } // 뺄셈 출현 불린값
    public bool Multipication { get; set; } // 곱셈 출현 불린값
    public bool Division { get; set; } // 나눗셈 출현 불린값

    private void Awake()
    {
        isPause = false; // 일시정지하지 않은 상태로 초기화

        // 모든 사칙연산이 출현하는 상태로 초기화
        Plus = true;
        Minus = true;
        Multipication = true;
        Division = true;
        option = 5;

        // 저장된 최고기록 로그에서 최고점수를 받아온다
        logPath = Application.persistentDataPath + "log.txt";
        fileStream = new FileStream(logPath, FileMode.OpenOrCreate);

        fileStream.Read(fileByte, 0, fileByte.Length);
        maxPointPlus = BitConverter.ToInt32(fileByte, 0);
        pointCheckPanel.transform.GetChild(1).GetComponent<Text>().text = maxPointPlus.ToString() + "점";

        fileStream.Read(fileByte, 0, fileByte.Length);
        maxPointMinus = BitConverter.ToInt32(fileByte, 0);
        pointCheckPanel.transform.GetChild(2).GetComponent<Text>().text = maxPointMinus.ToString() + "점";

        fileStream.Read(fileByte, 0, fileByte.Length);
        maxPointX = BitConverter.ToInt32(fileByte, 0);
        pointCheckPanel.transform.GetChild(3).GetComponent<Text>().text = maxPointX.ToString() + "점";

        fileStream.Read(fileByte, 0, fileByte.Length);
        maxPointDiv = BitConverter.ToInt32(fileByte, 0);
        pointCheckPanel.transform.GetChild(4).GetComponent<Text>().text = maxPointDiv.ToString() + "점";

        fileStream.Read(fileByte, 0, fileByte.Length);
        maxPointAll = BitConverter.ToInt32(fileByte, 0);
        pointCheckPanel.transform.GetChild(5).GetComponent<Text>().text = maxPointAll.ToString() + "점";

        fileStream.Close();
    }

    void Update()
    {
        // 뒤로가기 버튼을 누르면 일시정지한다
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(mainPanel.activeSelf)
            {
                if(!isPause)
                {
                    isPause = true;
                    Time.timeScale = 0;
                    pausePanel.SetActive(true);
                }
            }
        }
    }

    // 게임 시작 버튼 메소드
    public void StartGame()
    {
        mainPanel.SetActive(true);
        startPanel.SetActive(false);
    }

    // 게임 설정 버튼 메소드
    public void OptionGame()
    {
        optionPanel.SetActive(true);
    }

    // 덧셈 출현 옵션 메소드
    public void PlusOption(bool check)
    {
        if(check)
        {
            Plus = true;
            Minus = false;
            Multipication = false;
            Division = false;
            option = 1;
        }
    }

    // 뺄셈 출현 옵션 메소드
    public void MinusOption(bool check)
    {
        if (check)
        {
            Plus = false;
            Minus = true;
            Multipication = false;
            Division = false;
            option = 2;
        }
    }

    // 곱셈 출현 옵션 메소드
    public void MultiOption(bool check)
    {
        if (check)
        {
            Plus = false;
            Minus = false;
            Multipication = true;
            Division = false;
            option = 3;
        }
    }

    // 나눗셈 출현 옵션 메소드
    public void DivisionOption(bool check)
    {
        if (check)
        {
            Plus = false;
            Minus = false;
            Multipication = false;
            Division = true;
            option = 4;
        }
    }

    public void AllOption(bool check)
    {
        if(check)
        {
            Plus = true;
            Minus = true;
            Multipication = true;
            Division = true;
            option = 5;
        }
    }

    // 옵션 창을 닫는 메소드
    public void OptionClose()
    {
        optionPanel.SetActive(false);
    }

    public void PointCheckOpen()
    {
        pointCheckPanel.SetActive(true);
    }

    public void PointCheckClose()
    {
        pointCheckPanel.SetActive(false);
    }

    // 게임 종료 버튼 메소드
    public void EndGame()
    {
        Application.Quit();
    }

    // 일시정지 UI에서 YES를 선택하면 시작화면으로 돌아가는 메소드
    public void PauseYes()
    {
        startPanel.SetActive(true);
        mainPanel.SetActive(false);
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    // 일시정지 UI에서 NO를 선택하면 게임을 다시 진행하는 메소드
    public void PauseNo()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    // 체력이 0이 되면 호출되는 메소드
    // 게임 오버 패널을 활성화하고 게임을 정지시킨다
    public void GameOver(int point)
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        isPause = true;

        // 최고 점수를 갱신하면 로그 파일에 최고점수를 갱신하고, maxPoint도 최고점수로 갱신한다
        if (option == 1 && point > maxPointPlus)
        {
            fileStream = new FileStream(logPath, FileMode.Open);
            fileByte = BitConverter.GetBytes(point);
            fileStream.Seek(0, SeekOrigin.Begin);
            fileStream.Write(fileByte, 0, fileByte.Length);
            fileStream.Close();
            maxPointPlus = point;
            pointCheckPanel.transform.GetChild(1).GetComponent<Text>().text = maxPointPlus.ToString() + "점";
        }
        if(option == 2 && point > maxPointMinus)
        {
            fileStream = new FileStream(logPath, FileMode.Open);
            fileByte = BitConverter.GetBytes(point);
            fileStream.Seek(4, SeekOrigin.Begin);
            fileStream.Write(fileByte, 0, fileByte.Length);
            fileStream.Close();
            maxPointMinus = point;
            pointCheckPanel.transform.GetChild(2).GetComponent<Text>().text = maxPointMinus.ToString() + "점";
        }
        if (option == 3 && point > maxPointX)
        {
            fileStream = new FileStream(logPath, FileMode.Open);
            fileByte = BitConverter.GetBytes(point);
            fileStream.Seek(8, SeekOrigin.Begin);
            fileStream.Write(fileByte, 0, fileByte.Length);
            fileStream.Close();
            maxPointX = point;
            pointCheckPanel.transform.GetChild(3).GetComponent<Text>().text = maxPointX.ToString() + "점";
        }
        if (option == 4 && point > maxPointDiv)
        {
            fileStream = new FileStream(logPath, FileMode.Open);
            fileByte = BitConverter.GetBytes(point);
            fileStream.Seek(12, SeekOrigin.Begin);
            fileStream.Write(fileByte, 0, fileByte.Length);
            fileStream.Close();
            maxPointDiv = point;
            pointCheckPanel.transform.GetChild(4).GetComponent<Text>().text = maxPointDiv.ToString() + "점";
        }
        if (option == 5 && point > maxPointAll)
        {
            fileStream = new FileStream(logPath, FileMode.Open);
            fileByte = BitConverter.GetBytes(point);
            fileStream.Seek(16, SeekOrigin.Begin);
            fileStream.Write(fileByte, 0, fileByte.Length);
            fileStream.Close();
            maxPointAll = point;
            pointCheckPanel.transform.GetChild(5).GetComponent<Text>().text = maxPointAll.ToString() + "점";
        }
    }

    // 게임 오버 패널에서 재시작을 누르면 실행되는 메소드
    public void ReStart()
    {
        mainPanel.SetActive(false);
        mainPanel.SetActive(true);

        // 게임 오버 화면을 위한 설정들을 되돌린다
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }

    // 게임 오버 패널에서 재시작하지 않기를 누르면 실행되는 메소드
    public void BackStartPanel()
    {
        startPanel.SetActive(true);
        mainPanel.SetActive(false);

        // 게임 오버 화면을 위한 설정들을 되돌린다
        gameOverPanel.SetActive(false);
        Time.timeScale = 1;
        isPause = false;
    }
}
