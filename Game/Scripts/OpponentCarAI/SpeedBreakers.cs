using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBreaker : MonoBehaviour
{
    public float durationOfReduction = 3f;

    private void OnTriggerEnter(Collider other)
    {
        OpponentCar opponentCar = other.GetComponent<OpponentCar>();
        if (opponentCar != null)
        {
            opponentCar.acceleration = Random.Range(0.5f, 1f);
            opponentCar.currentSpeed = Random.Range(25f, 28f);
            StartCoroutine(ResetAcceleration(opponentCar));
        }
    }

    IEnumerator ResetAcceleration(OpponentCar opponentCar)
    {
        yield return new WaitForSeconds(durationOfReduction);
        // reset acceleration and current speed
        opponentCar.ResetAcceleration();
    }
}
