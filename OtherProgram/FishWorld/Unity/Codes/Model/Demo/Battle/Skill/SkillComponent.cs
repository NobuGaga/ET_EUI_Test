using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class SkillComponent: Entity, IAwake, IDestroy
    {
        public        long       Timer;
        public        List<int> SkillIds    = new List<int>();
    }
}