namespace ET
{
    public partial class Entity
    {
        /// <summary> 替换 ID </summary>
        /// <param name="oldId">旧的 Unit ID</param>
        /// <param name="newId">新的 Unit ID</param>
        public void Replace(long oldId, long newId)
        {
            this.children.TryGetValue(oldId, out Entity child);

            if (child == null)
                return;
            
            this.children.Add(newId, child);
            this.children.Remove(oldId);
        }
    }
}