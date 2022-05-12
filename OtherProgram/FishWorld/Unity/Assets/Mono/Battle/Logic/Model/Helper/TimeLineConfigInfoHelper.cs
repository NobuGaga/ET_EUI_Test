using System;
using System.Collections.Generic;

namespace ET
{
    /// <summary> 时间轴数据 Mono 组件类, 对应热更层 FishUnitComponent 持有跟调用 </summary>
    public static class TimeLineConfigInfoHelper
    {
        /// <summary> 时间轴配置表配置项数量最值 </summary>
        internal const ushort DefaultLineCount = 128;

        /// <summary> 时间轴配置表节点个数最值 </summary>
        private const ushort DefaultNodeCount = 32;

        /// <summary> 配置表字符串分隔符 </summary>
        private const char SplitSymbol = ',';

        private static Dictionary<int, List<TimeLineConfigInfo>> timeLineInfoMap;

        private static TimeLineConfigInfo startStateInfo;

        private static TimeLineConfigInfo endStateInfo;

        private static Action<long, TimeLineConfigInfo> executeTimeLineDelegate;

        static TimeLineConfigInfoHelper()
        {
            timeLineInfoMap = new Dictionary<int, List<TimeLineConfigInfo>>(DefaultLineCount);
            startStateInfo = new TimeLineConfigInfo(0, ConstHelper.TimeLineType_ActiveState);
            endStateInfo = new TimeLineConfigInfo(1, ConstHelper.TimeLineType_DeadState);
        }

        public static void Add(int id, string[] configNodeArray)
        {
            List<TimeLineConfigInfo> list;

            if (timeLineInfoMap.ContainsKey(id))
            {
                list = timeLineInfoMap[id];
                list.Clear();
            }
            else
            {
                list = new List<TimeLineConfigInfo>(DefaultNodeCount);
                timeLineInfoMap.Add(id, list);
            }

            bool hasStartState = false;
            bool hasEndState = false;
            for (int index = 0; index < configNodeArray.Length; index++)
            {
                string configNode = configNodeArray[index];
                string[] nodes = configNode.Split(SplitSymbol);

                TimeLineConfigInfo timeLineInfo = new TimeLineConfigInfo(float.Parse(nodes[0]), int.Parse(nodes[1]));
                if (timeLineInfo.Type == ConstHelper.TimeLineType_ActiveState) hasStartState = true;
                if (timeLineInfo.Type == ConstHelper.TimeLineType_DeadState) hasEndState = true;
                int argumentLength = nodes.Length - 2;
                timeLineInfo.Arguments = new string[argumentLength];

                for (int argumentIndex = 0; argumentIndex < argumentLength; argumentIndex++)
                    timeLineInfo.Arguments[argumentIndex] = nodes[argumentIndex + 2];

                list.Add(timeLineInfo);
            }

            list.Sort(SortTimeLineInfoList);

            if (!hasStartState) list.Insert(0, startStateInfo);
            if (!hasEndState) list.Add(endStateInfo);
        }

        private static int SortTimeLineInfoList(TimeLineConfigInfo leftInfo, TimeLineConfigInfo rightInfo)
        {
            if (leftInfo.LifeTime < rightInfo.LifeTime)
                return 1;
            else if (leftInfo.LifeTime > rightInfo.LifeTime)
                return -1;
            else 
                return 0;
        }

        public static void SetExecuteTimeLine(Action<long, TimeLineConfigInfo> action) =>
                           executeTimeLineDelegate = action;

        public static void FixedUpdate(long unitId, TimeLineMonoInfo info)
        {
            var configId = info.ConfigId;
            if (!timeLineInfoMap.ContainsKey(configId))
                return;

            var list = timeLineInfoMap[configId];
            info.LifeTime += (float)TimeHelper.ClinetDeltaFrameTime() / 1000;
            ref float currentListTime = ref info.LifeTime;
            for (int index = info.NodeIndex; index < list.Count; index++)
            {
                TimeLineConfigInfo nodeInfo = list[index];
                if (currentListTime < nodeInfo.LifeTime)
                    break;

                executeTimeLineDelegate(unitId, nodeInfo);
                info.NodeIndex = index;
            }
        }
    }
}