using System.Collections.Generic;
using System.Text;
using MsgPack;

namespace Tarantool.Net.IProto
{
    public class Tuple : IPackable
    {
        public Tuple()
        {
        }

        public Tuple(IEnumerable<object> fields)
        {
            _fields.AddRange(fields);
        }

        public void AddField(object data)
        {
            _fields.Add(data);
        }

        public void Reset()
        {
            _fields.Clear();
        }

        public List<object> Fields => _fields;

        private readonly List<object> _fields = new List<object>();

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            packer.PackArrayHeader(_fields.Count);
            foreach (var field in _fields)
            {
                if (field is string)
                    packer.PackString(field.ToString());
                else {
                    packer.PackObject(field);
                }
            }
        }

        public override string ToString()
        {
            return string.Concat("[", string.Join(", ", _fields), "]");
        }
    }
}

