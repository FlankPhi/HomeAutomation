using System;
using System.Collections;
using System.Net.Sockets;

namespace HomeAutomation.Etc.Generic
{
    public class List : IEnumerable
    {
        private readonly ArrayList _list;
        private readonly Type _myType;
        private int _index;

        public List(Type type)
        {
            _myType = type;
            _list = new ArrayList();
            _index = 0;
        }

        public List(Type type, int size)
        {
            _myType = type;
            _list = new ArrayList {Capacity = size};
        }

        public void Add(object obj)
        {
            //var object = (_myType) ;
            //if (component == null)
            //    throw new ArgumentException("Incorrect type. Type expected:" + _myType +
            //                                " Type received:" + obj.GetType());
            
            //bug This needs to validate type... how?
            _list.Add(obj);
            _index++;
        }

        public void Clear()
        {
            _list.Clear();
        }

        public object At(int index)
        {
            return _list[index];
        }

        public int Count()
        {
            return _index;
        }

        public bool Contains(object obj)
        {
            return _list.Contains(obj);
        }

        public void Remove(object obj)
        {
            if (!Contains(obj)) throw new ArgumentException("Tried to remove an object that doesn't exist.");
            _list.Remove(obj);
            _index--;
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
            _index--;
        }

        public int IndexOf(Socket socket)
        {
            return _list.IndexOf(socket);
        }

        public IEnumerator GetEnumerator()
        {
            var enumerator = _list.GetEnumerator();
            while (enumerator.MoveNext())
                yield return enumerator.Current;

        }

    }
}