using System.Collections.Generic;
using Trashure.Transition;

namespace Trashure.Save
{
    public class DataSlot
    {
        /// <summary>
        /// 進度條，String是GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region 用UI來顯示進度詳情
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + "/" +
                           timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日/";
                }
                else return string.Empty;
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Start" => "海邊",
                        "01.yongkang" => "農場",
                        "02.Home" => "木屋",
                        "03.Stall" => "市集",
                        _ => string.Empty
                    };
                }
                else return string.Empty;
            }
        }
        #endregion
    }
}