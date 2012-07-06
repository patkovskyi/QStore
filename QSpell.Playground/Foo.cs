using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace QSpell.Playground
{
    [ProtoContract]
    public class Foo<T> : IEnumerable<IEnumerable<T>>
    {
        [ProtoMember(1)]
        public string SomeMessage { get; protected set; }
        [ProtoMember(2)]
        private int someField;
        
        [ProtoMember(3)]
        private IEnumerable<IEnumerable<T>> values;

        public Foo(string message, int field, IEnumerable<IEnumerable<T>> values)
        {
            SomeMessage = message;
            someField = field;
            this.values = values;
        }

        public Foo() { }

        public IEnumerator<IEnumerable<T>> GetEnumerator()
        {
            return values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
