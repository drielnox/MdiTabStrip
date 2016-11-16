using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;

namespace MdiTabStrip
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class MdiTabCollection : CollectionBase
    {
        public MdiTab this[int index]
        {
            get { return (MdiTab)List[index]; }
            set { List[index] = value; }
        }

        public int VisibleCount
        {
            get
            {
                int c = 0;
                foreach (MdiTab tab in List)
                {
                    if (tab.Visible)
                    {
                        c += 1;
                    }
                }

                return c;
            }
        }

        public int FirstVisibleTabIndex
        {
            get
            {
                int index = 0;

                foreach (MdiTab tab in List)
                {
                    if (tab.Visible)
                    {
                        index = List.IndexOf(tab);
                    }
                }

                return index;
            }
        }

        public int LastVisibleTabIndex
        {
            get
            {
                int c = 0;
                foreach (MdiTab tab in List)
                {
                    if (tab.Visible)
                    {
                        c = List.IndexOf(tab);
                    }
                }

                return c;
            }
        }

        public int Add(MdiTab tab)
        {
            return List.Add(tab);
        }

        public bool Contains(MdiTab tab)
        {
            return List.Contains(tab);
        }

        public void Insert(int index, MdiTab value)
        {
            List.Insert(index, value);
        }

        public int IndexOf(MdiTab value)
        {
            return List.IndexOf(value);
        }

        public void Remove(MdiTab value)
        {
            List.Remove(value);
        }

        protected override void OnValidate(object value)
        {
            if (!typeof(MdiTab).IsAssignableFrom(value.GetType()))
            {
                throw new Exception("Value must be MdiTab");
            }
        }
    }
}
