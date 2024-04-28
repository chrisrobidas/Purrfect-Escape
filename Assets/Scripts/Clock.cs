using System;
using System.Collections;
using UnityEngine;

public class Clock : MonoBehaviour
{
    private Transform _hourHand;
    private Transform _minuteHand;
    private Transform _secondHand;
    void Start()
    {
        _minuteHand = transform.GetChild(0);
        _hourHand = transform.GetChild(1);
        _secondHand = transform.GetChild(2);
        
        StartCoroutine(nameof(ClockTick));
    }

    IEnumerator ClockTick()
    {
        while (gameObject.activeInHierarchy)
        {
            DateTime dateTime = DateTime.Now;
            double hourAngle = (dateTime.Hour%12 * 30 + dateTime.Minute/2f);
            double minuteAngle = dateTime.Minute * 6;
            double secondAngle = dateTime.Second * 6;

            _hourHand.localRotation = Quaternion.Euler(Vector3.forward * (float)hourAngle); 
            _minuteHand.localRotation = Quaternion.Euler(Vector3.forward * (float)minuteAngle); 
            _secondHand.localRotation = Quaternion.Euler(Vector3.forward * (float)secondAngle); 
            
            yield return new WaitForSeconds(1);
        }
    }
}
