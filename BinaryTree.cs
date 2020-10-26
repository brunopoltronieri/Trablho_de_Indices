using System;

namespace trabalho_arquivos
{
    public class BinaryTree<TData>
    {
        private class Node
        {
            public string Key;
            public TData Data;
            public Node Left;
            public Node Right;

            public Node(string key, TData data)
            {
                Key = key;
                Data = data;
                Left = null;
                Right = null;
            }
        }

        private Node _root;
        private int _count;

        public int Count => _count;

        public BinaryTree()
        {
            _root = null;
            _count = 0;
        }

        public void Insert(string key, TData data)
        {
            if (_root == null)
            {
                _root = new Node(key, data);
                _count++;
                return;
            }

            var it = _root;

            while (true)
            {
                var diff = string.Compare(key, it.Key);

                // new key is smaller than the current key
                if (diff < 0)
                {
                    if (it.Left == null)
                    {
                        it.Left = new Node(key, data);
                        _count++;
                        return;
                    }

                    it = it.Left;
                }
                // new key is greater than the current key
                else if (diff > 0)
                {
                    if (it.Right == null)
                    {
                        it.Right = new Node(key, data);
                        _count++;
                        return;
                    }

                    it = it.Right;
                }
                else throw new ArgumentException($"Key \"{key}\" already exists");
            }
        }

        public bool TryGet(string key, out TData data)
        {
            if (_root == null)
            {
                data = default(TData);
                return false;
            }

            var it = _root;

            while (true)
            {
                var diff = string.Compare(key, it.Key);

                // new key is smaller than the current key
                if (diff < 0)
                {
                    if (it.Left == null)
                    {
                        data = default(TData);
                        return false;
                    }

                    it = it.Left;
                }
                // new key is greater than the current key
                else if (diff > 0)
                {
                    if (it.Right == null)
                    {
                        data = default(TData);
                        return false;
                    }

                    it = it.Right;
                }
                // they are the same key
                else
                {
                    data = it.Data;
                    return true;
                }
            }
        }
    }
}