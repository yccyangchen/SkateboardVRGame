using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SelectionTaskMeasure : MonoBehaviour
{
    public GameObject targerT;
    public GameObject targerTPrefab;
    Vector3 targetTStartingPos;
    public GameObject objectT;
    public GameObject objectTPrefab;
    Vector3 objectTStartingPos;

    public GameObject taskStartPanel;
    public GameObject donePanel;
    public TMP_Text startPanelText;
    public TMP_Text scoreText;
    public int completeCount;
    public bool isTaskStart;
    public bool isTaskEnd;
    public bool isCountdown;
    public Vector3 manipulationError;
    public float taskTime;
    public GameObject taskUI;
    public GameObject stadiumPos;
    public ParkourCounter parkourCounter;
    public DataRecording dataRecording;
    private int part;
    public float partSumTime;
    public float partSumErr;
    public bool startAllowed = true;
    public bool doneAllowed = false;
    public LocomotionTechnique locomotionTechnique;


    // Start is called before the first frame update
    void Start()
    {
        parkourCounter = this.GetComponent<ParkourCounter>();
        dataRecording = this.GetComponent<DataRecording>();
        part = 1;
        donePanel.SetActive(false);
        scoreText.text = "Part" + part.ToString();
        taskStartPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isTaskStart)
        {
            // recording time
            taskTime += Time.deltaTime;
        }

        if (isCountdown)
        {
            taskTime += Time.deltaTime;
            startPanelText.text = (3.0 - taskTime).ToString("F1");
        }
    }

    public void StartOneTask()
    {
        startAllowed = false;
        doneAllowed = true;
        taskTime = 0f;
        taskStartPanel.SetActive(false);
        donePanel.SetActive(true);
        objectTStartingPos = stadiumPos.transform.position + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.1f, 0.1f), Random.Range(-0.25f, 0.25f));
        targetTStartingPos = stadiumPos.transform.position;
        objectT = Instantiate(objectTPrefab, objectTStartingPos, new Quaternion(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
        targerT = Instantiate(targerTPrefab, targetTStartingPos, new Quaternion(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)));
    }

    public void ChangeTShapeRotation(Quaternion rot)
    {
        if (objectT != null)
        {
            // The rotation of the user's hmd
            objectT.transform.rotation = objectT.transform.rotation * rot;
        }
    }

    public void ChangeTShapePosition(Vector3 pos)
    {
        if (objectT != null)
        {
            objectT.transform.position += pos;
        }
    }

    public void EndOneTask()
    {
        donePanel.SetActive(false);
        doneAllowed = false;
        
        // release
        isTaskEnd = true;
        isTaskStart = false;
        
        // distance error
        manipulationError = Vector3.zero;
        for (int i = 0; i < targerT.transform.childCount; i++)
        {
            manipulationError += targerT.transform.GetChild(i).transform.position - objectT.transform.GetChild(i).transform.position;
        }
        scoreText.text = scoreText.text + "Time: " + taskTime.ToString("F1") + ", offset: " + manipulationError.magnitude.ToString("F2") + "\n";
        partSumErr += manipulationError.magnitude;
        partSumTime += taskTime;
        dataRecording.AddOneData(parkourCounter.locomotionTech.stage.ToString(), completeCount, taskTime, manipulationError);

        // Debug.Log("Time: " + taskTime.ToString("F1") + "\nPrecision: " + manipulationError.magnitude.ToString("F1"));
        Destroy(objectT);
        Destroy(targerT);
        StartCoroutine(Countdown(0f));
    }

    IEnumerator Countdown(float t)
    {
        taskTime = 0f;
        taskStartPanel.SetActive(true);
        isCountdown = true;
        completeCount += 1;

        if (completeCount > 4)
        {
            taskStartPanel.SetActive(false);
            scoreText.text = "Done Part" + part.ToString();
            part += 1;
            completeCount = 0;
            locomotionTechnique.ShowStadionCamera(false);
        }
        else
        {
            yield return new WaitForSeconds(t);
            isCountdown = false;
            startPanelText.text = "start";
            startAllowed = true;
        }
        isCountdown = false;
        yield return 0;
    }
}
