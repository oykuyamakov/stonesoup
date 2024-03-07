using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tuniksRoomPicker : Room{

    public GameObject[] ValidatedRooms;

    public override Room createRoom(ExitConstraint requiredExits){
        List<Room> roomsThatMeetConstraints = new List<Room>();

        foreach (GameObject roomPrefab in ValidatedRooms){
            tuniksValidatedRoom validateRoom = roomPrefab.GetComponent<tuniksValidatedRoom>();
            if (validateRoom.MeetsConstraint(requiredExits)){
                roomsThatMeetConstraints.Add(validateRoom);
            }
        }

        Room newRoom = roomsThatMeetConstraints[Random.Range(0, roomsThatMeetConstraints.Count)];
        return Instantiate(newRoom);
    }
}
