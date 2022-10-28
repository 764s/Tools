namespace Collections
{
    /// 管子队列
    /// 会自动丢掉老的元素
    /// 内部通过 startPosition 来避免移除元素时整体迁移  
    public class Tube<T>
    {
        public int MaxCount { get; private set; }
        private int count;
        private int startPosition;
        private T[] arr;

        public int Count => count;

        public Tube(int maxCount)
        {
            this.MaxCount = maxCount;
        }

        public void Clear()
        {
            count = 0;
            startPosition = 0;
        }
    
        public void AddValue(T value)
        {
            var newCount = count < MaxCount ? count + 1 : MaxCount;
            
            if (arr == null)
            {
                arr = new T[4];  
            }
            else
            {
                if(arr.Length < newCount) Array.Resize(ref arr, Mathf.Min(arr.Length * 2, MaxCount));
            }

            if (count < MaxCount)
            {
                arr[count] = value;
                count = newCount;
            }
            else
            {
                arr[startPosition] = value;
                startPosition++;

                if (startPosition >= count)
                {
                    startPosition = 0;
                }
            }
        }

        public bool GetValue(int index, ref T value)
        {
            var internalIndex = GetInternalIndex(index);
            if (internalIndex == -1) return false;

            value = arr[internalIndex];
            return true;
        }

        /// 获取在数组的存储索引
        private int GetInternalIndex(int index)
        {
            if (index < 0 || index >= count) return -1;

            var internalIndexAtEnd = count - 1 - startPosition;
            if (index <= internalIndexAtEnd) return index + startPosition;
            return index - internalIndexAtEnd - 1;
        }
    }
}