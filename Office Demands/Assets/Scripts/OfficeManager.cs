using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeManager : MonoBehaviour
{
    public PaperList paperListTemplate;
    public Transform paperListParent;
    public List<PaperList> paperLists;
    public GameObject[] roomTemplates;
    public List<GameObject> rooms;
    public int roomsCount = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < roomsCount; i++)
        {
            CreateRoom();
        }
        ReadyPlayer(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoom()
    {
        PaperList paperList = Instantiate(paperListTemplate.gameObject, paperListParent).GetComponent<PaperList>();
        paperLists.Add(paperList);
        
        int randomRoomIndex = Random.Range(0, roomTemplates.Length);
        Vector3 roomPosStart;
        if (rooms.Count > 0)
        {
            roomPosStart = rooms[rooms.Count - 1].transform.Find("RoomEndPosition").position;
        }
        else roomPosStart = new Vector3(0, 0, 0);

        GameObject newRoom = Instantiate(roomTemplates[randomRoomIndex], roomPosStart, Quaternion.identity);
        rooms.Add(newRoom);
        newRoom.gameObject.SetActive(true);
        newRoom.transform.position += roomPosStart - newRoom.transform.Find("RoomStartPosition").position;
        newRoom.GetComponentInChildren<ObjectHolderManager>().paperList = paperList;
        newRoom.GetComponentInChildren<OfficeRoomCollider>().paperList = paperList;
    }

    public void PaperListCompleted(PaperList list)
    {
        int index = paperLists.IndexOf(list);
        paperLists.Remove(list);
        Destroy(list.gameObject);
        rooms.RemoveAt(index);

        if (index < paperLists.Count)
        {
            ReadyPlayer(index);
        }
    }

    public void ReadyPlayer(int roomIndex)
    {
        paperLists[roomIndex].SetupAndStart(this);

        Transform cam = Camera.main.transform;
        cam.position = rooms[roomIndex].transform.Find("RoomCameraPosition").position;
        /*
        foreach (ExteriorItem item in cam.GetComponentsInChildren<ExteriorItem>())
        {
            item.returnPos = item.transform.position;
        }
        foreach (InteriorItem item in cam.GetComponentsInChildren<InteriorItem>())
        {
            item.returnPos = item.transform.position;
        }
        */
    }
}
