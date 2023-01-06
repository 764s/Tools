using System.Collections.Generic;

/// 回收时不主动清理容器的内容, 应该先手动清理再放进来  
/// 用到的地方推荐 goto 写法, 避免不小心提前 return
/// todo 监测回收情况
public static class ContainerPool
{
    public static List<T> GetList<T>()
    {
        return ListPool<T>.GetFromPool();
    }

    /// 注意对材质容器, 应该始终清空, 防止卸载不掉
    public static void ReturnList<T>(List<T> list, bool clear = false)
    {
        ListPool<T>.ReturnToPool(list, clear);
    }
    
    public static T[] GetArray<T>(int capcity = -1, bool excatly = true)
    {
        return ArrayPool<T>.Get(capcity, excatly);
    }
    
    /// 注意对材质容器, 应该始终清空, 防止卸载不掉
    public static void ReturnArray<T>(T[] array, bool clear = false)
    {
        ArrayPool<T>.Return(array, clear);
    }
  
}
