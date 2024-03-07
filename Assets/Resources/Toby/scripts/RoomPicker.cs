using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPicker : Room
{

    public GameObject[] ValidatedRooms;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<Room> roomsThatMeetConstraints = new List<Room>();

        foreach (GameObject roomPrefab in ValidatedRooms)
        {
            ValidatedRoom validateRoom = roomPrefab.GetComponent<ValidatedRoom>();
            if (validateRoom.MeetsConstraint(requiredExits)){
                roomsThatMeetConstraints.Add(validateRoom);
            }
        }

        return Instantiate(
            roomsThatMeetConstraints[
                Random.Range(0, roomsThatMeetConstraints.Count)]);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}