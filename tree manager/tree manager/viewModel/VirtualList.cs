using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SendManager;
using tree_manager.Properties;

namespace tree_manager.viewModel
{
    public class Page<T> : List<T>
    {
    }
    public abstract class VirtualList<T> : IList
    {
        public int PageSize { get; private set; }
        private string[] _columns;
        protected IBackupData BackupData { get; private set; }
        public string[] Columns
        {
            get { return _columns ?? (_columns = GetCaptions()); }
        }

        private int _rowCount = -1;

        private Page<T>[] Pages { get; set; }

        protected VirtualList(int maxRowCount, IBackupData backupData)
        {
            PageSize = Settings.Default.maxSize;
            Pages = new Page<T>[(maxRowCount + PageSize - 1) / PageSize];
            BackupData = backupData;
            _rowCount = maxRowCount;
        }

        public virtual string[] GetCaptions()
        {
            var properties = typeof(T).GetProperties();
            return properties.Select(a => a.Name).ToArray();
        }


        public abstract Page<T> GetPage(int pageNumber);

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public int Count
        {
            get
            {
                return _rowCount;
            }
            set
            {
                _rowCount = value;
            }
        }


        public bool IsSynchronized
        {
            get { return false; }
        }


        public object SyncRoot
        {
            get { return this; }
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(object value)
        {
            return Pages.Any(page => page == value);
        }

        public int IndexOf(object value)
        {
            return 0;
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        object IList.this[int index]
        {
            get
            {
                int pageNumber = index / PageSize;
                if (Pages[pageNumber] != null && Pages[pageNumber].Count > index % PageSize)
                    return Pages[pageNumber][index % PageSize];

                var newPage = GetPage(pageNumber);
                Pages[pageNumber] = newPage;
                if (newPage != null && newPage.Count > index % PageSize)
                    return newPage[index % PageSize];
                return new object[PageSize];
            }
            set
            {
                throw new NotSupportedException();
            }

        }
        public T this[int index]
        {
            get
            {
                var t = (T)(this as IList)[index];
                return t;
            }
        }

    }
}
