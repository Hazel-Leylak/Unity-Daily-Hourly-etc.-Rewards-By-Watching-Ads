using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
public class HourlyControl : MonoBehaviour
{
    public string urlDate = "http://worldtimeapi.org/api/ip";
    public string now_date;
    public string now_hour;
    public int benzin;
    //public bool reward;
    public DateTime Now;
    public bool delete;
    public Button RewardButton;
    bool timerComplete = false;
    bool timerStart;
    float rem;
    TimeSpan diff;
    //float remaining;
    public Text remainingTime;
    DateTime endingTime;
    TimeSpan remaining;
    float timerRem;
    bool reward = false;
    //public Slider fuelBar;
    //public Text remainingTime;


        //if reward => playerprefs oldadate e kaydet
    private void Start()
    {
        if (delete) //if delete is true, then delete the oldDate playerPref record
        {
            PlayerPrefs.DeleteKey("oldDate");
            Debug.Log("deleted");
        }
        string oldDateStr = PlayerPrefs.GetString("oldDate"); // get the oldDate record
        Debug.Log("date: " + PlayerPrefs.GetString("oldDate"));
        if (string.IsNullOrEmpty(oldDateStr)) //if oldDate is empty then it's first login
        {
            enableButton();
            Debug.Log("First Login");

        }
        else //if it's not empty, then get the now date and find the difference between last reward date and now time
        {
            disableButton();
            StartCoroutine(CheckInternet());
        }

    }
    public void GetReward()
    {
        Debug.Log("GET REWARD");
        reward = true;
        StartCoroutine(CheckDate());
        
    }
    private IEnumerator CheckInternet() //check the internet connection. It's gonna get the date
    {
        WWW www_ = new WWW(urlDate);
        yield return www_;
        if (string.IsNullOrEmpty(www_.text))
        {
            Debug.Log("no connection");
        }
        else
        {
            Debug.Log("succes connection");
            StartCoroutine(CheckDate());
        }
    }
    private IEnumerator CheckDate() //check date and get time and date
    {
        Debug.Log("Check Date");
        disableButton();
        WWW www = new WWW(urlDate);
        yield return www;
        string[] splitDate = www.text.Split(new string[] { "datetime\":\"" }, StringSplitOptions.None);
        now_date = splitDate[1].Substring(0, 10);
        now_hour = splitDate[1].Substring(11, 8);
        Now = DateTime.Parse(now_date + " " + now_hour); //now date and time
        if (reward)
        {
            PlayerPrefs.SetString("oldDate", Now.ToString());
            Debug.Log("new date : " + PlayerPrefs.GetString("oldDate"));
            reward = false;
        }
        CheckTimeDiff();
       
    }
    public void CheckTimeDiff()
    {
       
        string oldDateStr = PlayerPrefs.GetString("oldDate");
        DateTime nowDate = Now; //now
        DateTime oldDate = Convert.ToDateTime(oldDateStr); //last the the reward claimed
        diff = nowDate - oldDate; //the time between now and last
        Debug.Log("diff: " + diff.TotalMinutes);
        if (diff.TotalSeconds > 120)
        {
            timerComplete = true;
            Debug.Log("diff is bigger than 1 min");
            enableButton();
        }
        else
        {
            Debug.Log("Now:" + Now);
            TimeSpan add = TimeSpan.FromSeconds(120);
            DateTime oldTime = Convert.ToDateTime(PlayerPrefs.GetString("oldDate"));
            endingTime = oldTime.Add(add);
            Debug.Log("ending: " + endingTime);
            remaining = endingTime - Now;
            Debug.Log("REM: " + remaining.TotalSeconds);
            timerRem = Mathf.Round((float)remaining.TotalSeconds);
            Debug.Log("TIMERREM : " + timerRem.ToString());
            //Debug.Log("diff is smaller than 1 min");
            disableButton();
            //remaining = (int)diff.TotalSeconds;
            timerComplete = false;
            timerStart = true;
        }
    }
    private void disableButton()
    {
        RewardButton.interactable = false;
    }
    private void enableButton()
    {
        RewardButton.interactable = true;
    }
    private void Update()
    {
        if (timerStart)
        {
            Debug.Log("time start");
            if (!timerComplete && !string.IsNullOrEmpty(PlayerPrefs.GetString("oldDate")))
            {
                //Debug.Log("AZALT");
                timerRem -= Time.deltaTime;
                remainingTime.text = Mathf.Round(timerRem).ToString();
                Debug.Log("AZALTILDI");
                //Debug.Log("timer text"+remaining);
                if (timerRem <= 0 && !timerComplete)
                {
                    Debug.Log("timer completed");
                    timerComplete = true;
                    remainingTime.text = "Get Reward";
                    enableButton();
                }
            }
        }
    }
    //public int GetReward()
    //{
    //    if (reward)
    //    {
    //        string oldDateStr = PlayerPrefs.GetString("oldDate");
    //        if (string.IsNullOrEmpty(oldDateStr))
    //        {
    //            Debug.Log("First Login");
    //            PlayerPrefs.SetString("oldDate", Now.ToString());
    //            enableButton();
    //            return 0;
    //        }
    //        else
    //        {
    //            StartCoroutine(CheckDate());
    //            DateTime nowDate = Now;
    //            DateTime oldDate = Convert.ToDateTime(oldDateStr);
    //            TimeSpan diff = nowDate - oldDate;
    //            Debug.Log("diff: " + diff.TotalMinutes);
    //            if (diff.TotalMinutes > 2)
    //            {
    //                Debug.Log("diff is bigger than 2");
    //                PlayerPrefs.SetString("oldDate", nowDate.ToString());
    //                benzin = 4;
    //            }
    //            return (int)diff.TotalMinutes;
    //        }
    //    }
    //    else return 6;
    //}

}