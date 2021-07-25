using Kochi_TVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kochi_TVM.Business
{
    public static class Stations
    {
        public static Station currentStation = new Station();
        public static Dictionary<int, Station> stationList = new Dictionary<int, Station>();
        public static bool FillStationList()
        {
            bool result = false;
            stationList.Clear();
            try
            {
                using (var context = new TVM_Entities())
                {
                    var courses = context.sp_SelStations("en").ToList();
                    foreach(var item in courses)
                    {
                        var listStation = new Station
                        {
                            id = Convert.ToInt32(item.recId),
                            order = Convert.ToInt32(item.recId),
                            name = item.explanation.ToString(),
                            description = item.explanation.ToString(),
                            code = item.stationCode.ToString(),
                            mapRow = Convert.ToInt32(item.mapRow),
                            mapColumn = Convert.ToInt32(item.mapColumn),
                            mapHereRow = Convert.ToInt32(item.mapHereRow),
                            mapHereColumn = Convert.ToInt32(item.mapHereColumn)
                        };
                        AddStation(listStation);
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        public static bool FillCurrentStation()
        {
            bool result = false;
            try
            {
                int stationId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId"));
                Parameters.TVMConst.iniReader.Write("DEVICES_NAME", "StationId", stationId.ToString());
                currentStation = Stations.GetStation(stationId);

                //Log.log.Write("name: " + currentStation.name + " map row: " + currentStation.mapRow.ToString() + " mapColumn: " + currentStation.mapHereColumn.ToString()
                //       + " mapHereRow: " + currentStation.mapHereRow.ToString() + " mapHereColumn: " + currentStation.mapHereColumn.ToString());
                result = true;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        private static bool AddStation(Station listStation)
        {
            bool result = false;
            try
            {
                if (!stationList.ContainsKey(listStation.id))
                {
                    stationList.Add(listStation.id, listStation);
                }
                result = true;
            }
            catch (Exception e)
            { }
            return result;
        }
        public static Station GetStation(int id)
        {
            return stationList[id];
        }
        public static Station GetStation(string name)
        {
            int id = 0;
            foreach (var item in stationList)
            {
                if (item.Value.name == name)
                {
                    id = item.Key;
                    break;
                }
            }
            return GetStation(id);
        }
    }

    public class Station
    {
        public Station()
        {
            id = order = 0;
            description = name = "";
            code = "";
            mapRow = 3;
            mapColumn = 4;
            mapHereRow = 4;
            mapHereColumn = 0;
        }
        public Station(int o, int i, string n, string d, string c, int mr, int mc, int hr, int hc)
        {
            order = o;
            id = i;
            name = n;
            description = d;
            code = c;
            mapRow = mr;
            mapColumn = mc;
            mapHereRow = hr;
            mapHereColumn = hc;
        }
        public int order;
        public int id;
        public string name;
        public string description;
        public string code;
        //on railway map coordinate 100 * 100 grid
        public int mapRow;
        public int mapColumn;
        public int mapHereRow;
        public int mapHereColumn;
    }
}
