using System.Collections.Generic;

public class ListPool<T>
{
    private static Stack<List<T>> colorContainers;
    
    public static List<T> GetFromPool()
    {
        if (colorContainers != null && colorContainers.Count > 0)
        {
            return colorContainers.Pop();
        }

        return new List<T>();
    }
    
    public static void ReturnToPool(List<T> container, bool clear)
    {
        if (clear)
        {
            for (var i = 0; i < container.Count; i++)
            {
                container[i] = default;
            }
        }
        
        if (colorContainers == null) colorContainers = new Stack<List<T>>();
        colorContainers.Push(container);
    }
}
