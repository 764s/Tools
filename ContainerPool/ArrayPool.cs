using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class ArrayPool<T>
{
    private class Pool
    {
        private Stack<T[]> containers;

        private int capcity;

        public Pool(int capcity)
        {
            this.capcity = capcity;
        }

        public T[] GetFromPool()
        {
            T[] result = default;

            if (containers == null || containers.Count == 0)
            {
                result = new T[capcity];
            }
            else
            {
                result = containers.Pop();
            }

            return result;
        }

        public void ReturnToPool(T[] instance)
        {
            if (containers == null) containers = new Stack<T[]>();
            containers.Push(instance);
        }
    }

    private const int maxExactCapcity = 30;
    private const int defaultCapcity = 64;

    private static Pool[] powerPoolList;
    private static Pool defaultPool;
    private static Pool[] fixedPoolList;

    public static T[] Get(int capcity = -1, bool excatly = false)
    {
        if (capcity == -1) capcity = defaultCapcity;

        capcity = GetCapcity(capcity, excatly, out var mode, out var power);

        switch (mode)
        {
            case PoolMode.Default:
                if (defaultPool == null) defaultPool = new Pool(defaultCapcity);
                return defaultPool.GetFromPool();
            
            case PoolMode.Fixed:
                if (fixedPoolList == null) fixedPoolList = new Pool[maxExactCapcity + 1];
                if (fixedPoolList[capcity] == null) fixedPoolList[capcity] = new Pool(capcity);
                return fixedPoolList[capcity].GetFromPool();
                
            // case PoolMode.Power:
            default:
                if (powerPoolList == null) powerPoolList = new Pool[32];
                if (powerPoolList[power] == null) powerPoolList[power] = new Pool(capcity);
                return powerPoolList[power].GetFromPool();
        }
    }


    public static void Return(T[] value, bool clear)
    {
        if (clear)
        {
            for (var i = 0; i < value.Length; i++)
            {
                value[i] = default;
            }
        }
    
        var capcity = GetCapcity(value.Length, value.Length <= maxExactCapcity, out var mode, out var pow);

#if UNITY_ASSERTIONS
        Assert.IsTrue(capcity == value.Length, $"{nameof(ArrayPool<T>)} 回收异常 容器长度不正确");
#endif

        switch (mode)
        {
            case PoolMode.Default:
                defaultPool.ReturnToPool(value);
                break;
            
            case PoolMode.Fixed:
                fixedPoolList[capcity].ReturnToPool(value);
                break;
                
            // case PoolMode.Power:
            default:
                powerPoolList[pow].ReturnToPool(value);
                break;
        }
    }
    
    private static int GetCapcity(int capcity, bool excatly, out PoolMode mode, out int pow)
    {
        if (capcity == defaultCapcity) goto DefaultMode;

        if (excatly)
        {
            if (capcity <= maxExactCapcity) goto FixedMode;
            Debug.LogWarning($"{nameof(ArrayPool<T>)}.{nameof(GetCapcity)} 获取精确容量的数组时, 最大容量不能超过{maxExactCapcity}, 指定容量: {capcity}");
        }

        if (capcity < defaultCapcity) goto DefaultMode;

        mode = PoolMode.Power;
        pow = Mathf.CeilToInt(Mathf.Log(capcity, 2));
        
        // Debug.Log($"{nameof(ArrayPool<T>)}.{nameof(GetCapcity)} mode: {mode} capcity: {(int)Mathf.Pow(2, pow)} pow: {pow}");
        
        return (int)Mathf.Pow(2, pow);
        
        
        DefaultMode:
        mode = PoolMode.Default;
        pow = 0;
        
        // Debug.Log($"{nameof(ArrayPool<T>)}.{nameof(GetCapcity)} mode: {mode} capcity: {defaultCapcity} pow: {pow}");
            
        return defaultCapcity;
        
        FixedMode:
        mode = PoolMode.Fixed;
        pow = 0;
        
        // Debug.Log($"{nameof(ArrayPool<T>)}.{nameof(GetCapcity)} mode: {mode} capcity: {capcity} pow: {pow}");
        
        return capcity;
        
    }

    private enum PoolMode
    {
        Default,
        Fixed,
        Power,
    }
}
