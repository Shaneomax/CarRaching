using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapSystem : MonoBehaviour
{
    public int maxLaps;

    private void OnTriggerEnter(Collider other)
    {
        OpponentCar opponentCar = other.GetComponent<OpponentCar>();
        CarController playerCar = other.GetComponent<CarController>();

        if (opponentCar != null)
        {
            opponentCar.IncreaseLap();
            CheckRaceCompletion(opponentCar); // ✅ Now it checks for race completion
        }

        if (playerCar != null)
        {
            playerCar.IncreaseLap();
            CheckRaceCompletion(playerCar); // ✅ Now it checks for race completion
        }
    }

    private void CheckRaceCompletion(OpponentCar opponentCar)
    {
        if (opponentCar.currentLap >= maxLaps) // ✅ Use >= instead of >
        {
            EndMission(false); // Player loses
        }
    }

    private void CheckRaceCompletion(CarController playerCar)
    {
        if (playerCar.currentLap >= maxLaps) // ✅ Use >= instead of >
        {
            EndMission(true); // Player wins
        }
    }

    void EndMission(bool success)
    {
        if (success)
        {
            Debug.Log("🏁 Player Wins the Race!");
            // You can add more win logic here (UI, stop movement, etc.)
        }
        else
        {
            Debug.Log("❌ Player Loses the Race!");
            // You can add more lose logic here
        }
    }
}
