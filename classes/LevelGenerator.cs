using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Generator
{

    public class LevelGenerator : MonoBehaviour
    {
        //serialized settings, changeable only before generation
        [Header("Settings")]
        [SerializeField] private int set_MapSizeX;
        [SerializeField] private int set_MapSizeY;
        [SerializeField] private int set_maxRoomAmount;
        [SerializeField] private int set_roomAmountTolerance;
        [SerializeField] private float set_minRoomDistance;
        [SerializeField] private int set_scale = 5;
        [SerializeField] private int set_roomSize;
        [SerializeField] private int set_roomSizeTolerance;

        //public properties
        public Vector2 MapSize { get; private set; }
        public List<Room> Rooms { get; private set; }

        //private fields
        private int roomAmount;

        private void Start()
        {
            GeneratorVisualiser.Start(); //later used in Room objects - initiation requied
            GenerateMap();
        }

        public void GenerateMap()
        {
            GeneratorVisualiser.Clear();
            InitSettings();
            GenerateMapBorders();
            GenerateRooms();
            CheckForOverlapping();
            FillRooms();
        }//can be called by anything, even by a button, causes whole map re-generation (not only generation but cleanup as well)

        private void InitSettings()
        {
            MapSize = new Vector2(set_MapSizeX, set_MapSizeY);
            roomAmount = Random.Range(set_maxRoomAmount - set_roomAmountTolerance, set_maxRoomAmount + set_roomAmountTolerance);
            Rooms = new List<Room>();
        }

        private void GenerateMapBorders()
        {
            RoomMapBorders map = new RoomMapBorders(MapSize.x, MapSize.y, set_scale);
            Rooms.Add(map);     //RoomMapBorders derives from Room
            map.Generate();
        }

        private void GenerateRooms()
        {
            for (int i = 0; i < roomAmount; i++)
            {
                bool generatedPosition = true;
                Vector3 newPos;
                int triesCounter = 0;
                while (true) //try to generate a new room position in requied distance
                {
                    //set a random position
                    newPos = new Vector3(
                        Mathf.Round(Random.Range(-MapSize.x / 2, MapSize.x / 2)) * set_scale, 
                        0, 
                        Mathf.Round(Random.Range(-MapSize.y / 2, MapSize.y / 2)) * set_scale
                        );

                    float distance = 0;

                    if (Rooms.Count == 1) //if there's only the map border (it's always first)
                    {
                        break;
                    }

                    //check if any of the other rooms are too close
                    foreach (Room room in Rooms)
                    {
                        iRoomGeneratable generatable = room as iRoomGeneratable;
                        if (generatable != null)
                        {
                            distance = Vector3.Distance(room.Pos, newPos);
                            if (distance < set_minRoomDistance)
                            {
                                generatedPosition = false;
                            }
                        }
                    }

                    if (generatedPosition) break;

                    //failsafe
                    triesCounter++;
                    generatedPosition = true;
                    if (triesCounter > 10)
                    {
                        generatedPosition = false;
                        break;
                    }

                }

                //position generated, now add a room
                if (generatedPosition)
                {
                    int randomScaleX = Random.Range(set_roomSize - set_roomSizeTolerance, set_roomSize + set_roomSizeTolerance);
                    int randomScaleY = Random.Range(set_roomSize - set_roomSizeTolerance, set_roomSize + set_roomSizeTolerance);
                    Room newRoom = new RoomGeneric(randomScaleX, randomScaleY, newPos.x, newPos.z, set_scale);

                    Rooms.Add(newRoom);
                    iRoomGeneratable generatable = newRoom as iRoomGeneratable;
                    if (generatable != null)
                    {
                        newRoom.RoomID = i;
                        generatable.Generate();
                    }
                }
            }
        }

        private void CheckForOverlapping() //adjust rooms shapes based on collisions with other rooms
        {
            for (int i = 0; i < Rooms.Count; i++) //first room checked
            {
                iRoomCollidable iCheck = Rooms[i] as iRoomCollidable; //check for interface - only to perform check on valid rooms
                if (iCheck != null)
                {
                    for (int y = 0; y < Rooms.Count; y++) //second (other) room checked
                    {
                        iRoomCollidable iiRoom = Rooms[y] as iRoomCollidable;
                        if (iiRoom != null)
                        {
                            bool checkableID = true;

                            if (Rooms[y].RoomID == Rooms[i].RoomID) //don't check for the same room
                            {
                                checkableID = false; 
                            }

                            foreach (int checkedId in Rooms[y].OverlapsWith) 
                            {
                                if (checkedId == Rooms[i].RoomID) //ignore already checked rooms
                                {
                                    checkableID = false;
                                    break;
                                }
                            }

                            if (checkableID) 
                            {
                                iCheck.CheckOverlapping(Rooms[y]); //adjust borders if collides with another room
                            }
                        }
                    }

                    (Rooms[i] as RoomGeneric).DisplayBorder();
                }
            }
        }

        private void FillRooms() //fill each room with "floor" markers
        {
            foreach (Room r in Rooms)
            {
                iRoomGeneratable generatable = r as iRoomGeneratable;
                if(generatable != null)
                {
                    generatable.Fill(GeneratorRoomFiller.FillRoom(r)); //generate a filling and assign add it to a room
                }

            }
        }
    }

    //EDITOR GUI
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelGenerator))]
    public class LevelScriptEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Generate"))
            {
                (target as LevelGenerator).GenerateMap();
            }
        }
    }
#endif
}


