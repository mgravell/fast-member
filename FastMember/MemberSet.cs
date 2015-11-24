using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastMember
{
    /// <summary>
    /// Represents an abstracted view of the members defined for a type
    /// </summary>
    public sealed class MemberSet : IEnumerable<Member>, IList<Member>
    {
        Member[] members;
        internal MemberSet(Type type)
        {
            members = type.GetProperties().Cast<MemberInfo>().Concat(type.GetFields().Cast<MemberInfo>()).OrderBy(x => x.Name, StringComparer.Ordinal)
                .Select(member => new Member(member)).ToArray();
        }
        /// <summary>
        /// Return a sequence of all defined members
        /// </summary>
        public IEnumerator<Member> GetEnumerator()
        {
            foreach (var member in members) yield return member;
        }
        /// <summary>
        /// Get a member by index
        /// </summary>
        public Member this[int index]
        {
            get { return members[index]; }
        }
        /// <summary>
        /// The number of members defined for this type
        /// </summary>
        public int Count { get { return members.Length; } }
        Member IList<Member>.this[int index]
        {
            get { return members[index]; }
            set { throw new NotSupportedException(); }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        bool ICollection<Member>.Remove(Member item) { throw new NotSupportedException(); }
        void ICollection<Member>.Add(Member item) { throw new NotSupportedException(); }
        void ICollection<Member>.Clear() { throw new NotSupportedException(); }
        void IList<Member>.RemoveAt(int index) { throw new NotSupportedException(); }
        void IList<Member>.Insert(int index, Member item) { throw new NotSupportedException(); }

        bool ICollection<Member>.Contains(Member item) { return members.Contains(item); }
        void ICollection<Member>.CopyTo(Member[] array, int arrayIndex) { members.CopyTo(array, arrayIndex); }
        bool ICollection<Member>.IsReadOnly { get { return true; } }
        int IList<Member>.IndexOf(Member member) { return Array.IndexOf<Member>(members, member); }
        
    }
    /// <summary>
    /// Represents an abstracted view of an individual member defined for a type
    /// </summary>
    public sealed class Member
    {
        private readonly MemberInfo member;
        internal Member(MemberInfo member)
        {
            this.member = member;
        }
        /// <summary>
        /// The name of this member
        /// </summary>
        public string Name { get { return member.Name; } }
        /// <summary>
        /// The type of value stored in this member
        /// </summary>
        public Type Type
        {
            get
            {
                if(member is FieldInfo) return ((FieldInfo)member).FieldType;
                if (member is PropertyInfo) return ((PropertyInfo)member).PropertyType;
                throw new NotSupportedException(member.GetType().Name);
            }
        }

        /// <summary>
        /// Is the attribute specified defined on this type
        /// </summary>
        public bool IsDefined(Type attributeType)
        {
            if (attributeType == null) throw new ArgumentNullException("attributeType");
#if COREFX
            return member.IsDefined(attributeType);
#else
            return Attribute.IsDefined(member, attributeType);
#endif
        }


    }
}
