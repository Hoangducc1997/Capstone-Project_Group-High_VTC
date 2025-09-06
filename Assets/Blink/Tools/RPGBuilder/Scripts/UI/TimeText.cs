using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeText : MonoBehaviour
{
    public TextMeshProUGUI text;
    //public Image sunImg;
    //public Image moonImg;

    private void OnEnable()
    {
        WorldEvents.TimeChange += TimeChanged;
    }

    private void OnDisable()
    {
        WorldEvents.TimeChange -= TimeChanged;
    }

    private void TimeChanged(CharacterEntries.TimeData timeData)
    {
        // Hiển thị giờ phút
        string minuteText = timeData.CurrentMinute >= 10
            ? timeData.CurrentMinute.ToString()
            : "0" + timeData.CurrentMinute;
        text.text = timeData.CurrentHour + ":" + minuteText;

        //// Ban ngày (4h - 20h) => hiện mặt trời
        //if (timeData.CurrentHour >= 4 && timeData.CurrentHour < 20)
        //{
        //    sunImg.gameObject.SetActive(true);
        //    moonImg.gameObject.SetActive(false);
        //}
        //else // Ban đêm (20h - 4h) => hiện mặt trăng
        //{
        //    sunImg.gameObject.SetActive(false);
        //    moonImg.gameObject.SetActive(true);
        //}
    }
}
