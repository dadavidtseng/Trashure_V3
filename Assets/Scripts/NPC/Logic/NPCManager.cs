using System.Collections.Generic;

public class NPCManager : Singleton<NPCManager>
{
#region 宣告
    public SceneRouteDataList_SO sceneRouteDate;
    public List<NPCPosition> npcPositionList;

    private Dictionary<string, SceneRoute> sceneRouteDict = new Dictionary<string, SceneRoute>();
#endregion

#region 事件函數
    protected override void Awake()
    {
        base.Awake();
        
        InitSceneRouteDict();
    }

    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }
#endregion

#region 註冊事件
    private void OnStartNewGameEvent(int obj)
    {
        foreach (var character in npcPositionList)
        {
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().StartScene = character.startScene;
        }
    }
#endregion

    /// <summary>
    /// 初始化路徑字典
    /// </summary>
    private void InitSceneRouteDict()
    {
        if (sceneRouteDate.sceneRouteList.Count > 0)
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList)
            {
                var key = route.fromSceneName + route.gotoSceneName;

                if (sceneRouteDict.ContainsKey(key))
                {
                    continue;
                }
                sceneRouteDict.Add(key, route);
                
            }
        }
    }

    /// <summary>
    /// 獲得兩個場景間的路徑
    /// </summary>
    /// <param name="fromSceneName">起始場景</param>
    /// <param name="gotoSceneName">目標場景</param>
    /// <returns></returns>
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName)
    {
        return sceneRouteDict[fromSceneName + gotoSceneName];
    }
}
