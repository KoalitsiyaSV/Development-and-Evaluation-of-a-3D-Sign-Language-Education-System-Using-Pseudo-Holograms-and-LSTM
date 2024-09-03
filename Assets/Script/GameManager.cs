using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private string[] stringArray = new string[8];
    private float[] scoreArray = new float[8];

    //������
    public AngleDataSender angleDataSender;

    //���ۿ�
    public DataSender dataSender;

    /*
    *  Game Scene
    */
    // Scene UI
    [Header("Game Scene UI")]
    public GameObject GameMenuUI;
    public GameObject GamePlayUI;
    public GameObject ScoreUI;
    public GameObject HelpUI;

    // Scene Texts
    [Header("Game Scene Text")]
    public TextMeshPro SuggestedText;
    public TextMeshPro CountDownText;
    
    // Scene Factors
    private float countDownTime;
    private float currentTime;

    private string[] SuggestedWords = new string[8];

    /*
     *  Study UI
     */
    [Header("Study Scene UI")]
    public GameObject StudyMenuUI;
    public GameObject StudyUI;
    public GameObject StudyOptionUI;
    public GameObject StudyScoreUI;
    public GameObject StudyHelpUI;

    [Header("Study Scene Text")]
    public TextMeshPro StudyText;
    public TextMeshProUGUI CountDownText2;

    [Header("For Study Animation")]
    public GameObject ExampleHand;
    public Animator RightHandAnimator;
    public Animator LeftHandAnimator;
    public GameObject LeftHand;

    private float CurrentTimeStudy;

    bool bCanPlay;

    private void Start()
    {

        bCanPlay = true;

        stringArray[0] = "�ȳ��ϼ���";
        stringArray[1] = "����ؿ�";
        stringArray[2] = "������";
        stringArray[3] = "���Ŀ�";
        stringArray[4] = "�����ؿ�";
        stringArray[5] = "�߿���";
        stringArray[6] = "�ƴϿ�";
        stringArray[7] = "������";

        SuggestedWords = stringArray;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.S))
        {
            StudyAllBtnPressed();
        }

       if(Input.GetKeyDown(KeyCode.T))
        {
            PlayBtnPressed();
        }
    }

    public void OnExitGame()
    {
        Application.Quit();
    }

    /*
    *  Game Scene
    */

    // GameStart
    public void PlayBtnPressed()
    {
        if (!GameMenuUI || !GamePlayUI || !ScoreUI) return;

        // �迭 ������
        //ShuffleArray(SuggestedWords);

        // ���� ���� �� UI ����
        GameMenuUI.SetActive(false);

        // ���� ���� �� UI Ȱ��
        GamePlayUI.SetActive(true);

        // ����
        // �ܾ �����ְ� 321 ī��Ʈ �Ŀ� 5�� ���� �����ϱ�
        DeleteFiles();

        StartCoroutine(PlayGame());
    }

    // Game Exit
    public void PlayStopBtnPressed()
    {
        StopAllCoroutines();

        GamePlayUI.SetActive(false);
        GameMenuUI.SetActive(true);
    }

    // Exit Btn in Score Panel
    public void ScorePanelExitBtnPressed()
    {
        ScoreUI.SetActive(false);
        GameMenuUI.SetActive(true);
    }

    // Help Btn in Game Scene Menu
    public void HelpBtnPressed()
    {
        if (HelpUI == null) return;

        GameMenuUI.SetActive(false);
        HelpUI.SetActive(true);
    }

    // Exit Btn in Help Panel
    public void HelpPanelExitBtnPressed()
    {
        if (GameMenuUI == null) return;

        HelpUI.SetActive(false);
        GameMenuUI.SetActive(true);
    }

    // SuffleArray
    void ShuffleArray(string[] array)
    {
        for(int i = 6; i > 3; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string tmp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = tmp;
        }
    }

    // Game Flow
    IEnumerator PlayGame()
    {
        for (int i = 0; i < 4; i++)
        {
            SuggestedText.text = SuggestedWords[i];

            currentTime = 3f;

            angleDataSender.setFileName = SuggestedWords[i];
            angleDataSender.setFileNum = "0";

            yield return StartCoroutine(CountDown());

            
        }
        ShowScorePanel();
    }

    // Count Down in Game
    IEnumerator CountDown()
    {
        while(currentTime > 0)
        {
            CountDownText.text = Mathf.Ceil(currentTime).ToString();

            yield return new WaitForSeconds(1.0f);

            currentTime -= 1.0f;
        }

        CountDownText.text = "Start!";

        yield return new WaitForSeconds(1.0f);
        angleDataSender.setTime = "5";
        angleDataSender.ReverseTrigger();

        currentTime = 5f;

        while (currentTime > 0)
        {
            CountDownText.text = Mathf.Ceil(currentTime).ToString();

            yield return new WaitForSeconds(1.0f);

            currentTime -= 1.0f;
        }
    }

    // Show Score Panel after Game
    void ShowScorePanel()
    {
        GamePlayUI.SetActive(false);
        ScoreUI.SetActive(true);
    }

    void DeleteFiles()
    {
        string path = Application.streamingAssetsPath + "/data"; // data ���� ���

        // �־��� ����� ��� ������ ������
        string[] files = Directory.GetFiles(path);

        // ������ �ϳ��� �ݺ��Ͽ� ����
        foreach (string file in files)
        {
            File.Delete(file);
        }
    }

    /*
    *  Study Scene
    */
    public void StudyAllBtnPressed()
    {
        StudyMenuUI.SetActive(false);
        StudyUI.SetActive(true);

        StartCoroutine(PlayStudy());
    }

    public void StudyOptionBtnPressed()
    {
        
    }

    public void ExitBtnInStudyPressed()
    {
        StopAllCoroutines();

        StudyUI.SetActive(false);
        StudyMenuUI.SetActive(true);
    }

    // Help UI
    public void StudyHelpBtnPressed()
    {
        StudyMenuUI.SetActive(false);
        StudyHelpUI.SetActive(true);
    }
    
    // Exit Help UI
    public void ExitBtnInStudyHelpPressed()
    {
        StudyMenuUI.SetActive(true);
        StudyHelpUI.SetActive(false);
    }

    public void EndStudy()
    {
        StudyUI.SetActive(false);
        StudyMenuUI.SetActive(true);
    }

    public void ExitBtnInStudyScorePressed()
    {
        StudyScoreUI.SetActive(false);
        StudyMenuUI.SetActive(true);
    }

    IEnumerator PlayStudy()
    {

        for(int AnimNum = 0; AnimNum < 4; AnimNum++)
        {
            StudyText.text = stringArray[AnimNum];
           
            /*
            *  ���⼭ ������ �����ִ��� �� �𵨷� ���ø� �����ִ��� ��
            */

            for (int j = 0; j < 3; j++)
            {
                Debug.Log(AnimNum);

                yield return new WaitForSeconds(1.0f);

                ShowExample();

                if (AnimNum == 3)
                {
                    LeftHand.SetActive(false);
                }

                RightHandAnimator.SetInteger("AnimNum", AnimNum);
                if (AnimNum != 3)
                {
                    LeftHandAnimator.SetInteger("AnimNum", AnimNum);
                }

                //yield return StartCoroutine(WaitForAnimationState(RightHandAnimator, AnimNum));
                //if (AnimNum != 3)
                //{
                //    yield return StartCoroutine(WaitForAnimationState(LeftHandAnimator, AnimNum));
                //}

                yield return new WaitForSeconds(0.3f);

                AnimatorStateInfo stateInfo = RightHandAnimator.GetCurrentAnimatorStateInfo(0);
                float AnimDuration = stateInfo.length;

                Debug.Log(AnimDuration);

                yield return new WaitForSeconds(AnimDuration);



                HideExample();

                //CurrentTimeStudy = AnimDuration + 0.5f;
                //int RoundedDuration = Mathf.CeilToInt(CurrentTimeStudy);

                

                //// �н����� ���� ������ �ʿ��ұ�?
                ////angleDataSender.setTime = RoundedDuration.ToString();
                ////angleDataSender.setFileName = stringArray[i];
                ////angleDataSender.setFileNum = j + 1.ToString();
                ////angleDataSender.ReverseTrigger();

                //while (RoundedDuration > -1)
                //{
                //    CountDownText2.text = Mathf.Ceil(RoundedDuration).ToString();

                //    yield return new WaitForSeconds(1.0f);

                //    RoundedDuration -= 1;
                //}

                CountDownText2.text = "";
            }
        }

        LeftHand.SetActive(true);
        EndStudy();
    }

    public void ShowExample()
    {
        ExampleHand.SetActive(true);
    }

    public void HideExample()
    {
        ExampleHand.SetActive(false);
    }

    IEnumerator WaitForAnimationState(Animator animator, int animNum)
    {
        // ������ �ִϸ��̼� ���·� ��ȯ�� ������ ���
        while (animator.GetInteger("AnimNum") != animNum || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }
}
