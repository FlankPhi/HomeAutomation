using System;
using System.Collections;

namespace HomeAutomation
{
    public class List
    {
        private readonly ArrayList _list;
        private readonly Type _myType;

        public List(Type type)
        {
            _myType = type;
            _list = new ArrayList();
        }

        public List(Type type, int size)
        {
            _myType = type;
            _list = new ArrayList { Capacity = size };
        }

        public void Add(Object obj)
        {
            if (obj.GetType() != _myType)
                throw new ArgumentException("Incorrect type. Type expected:" + _myType + " Type received:" +
                                            obj.GetType());
            _list.Add(obj);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public object At(int index)
        {
            return _list[index];
        }

        public bool Contains(object obj)
        {
            return _list.Contains(obj);
        }

        public void Remove(object obj)
        {
            if (!Contains(obj)) throw new ArgumentException("Tried to remove an object that doesn't exist.");
            _list.Remove(obj);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }
    }
}