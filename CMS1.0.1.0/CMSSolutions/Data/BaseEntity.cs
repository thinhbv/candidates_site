using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace CMSSolutions.Data
{
    [DataContract]
    [Serializable]
    public abstract class BaseEntity
    {
        public abstract bool IsTransient();

        public abstract void OnInserting();

        public abstract object GetIdValue();
    }

    [DataContract]
    [Serializable]
    public abstract class BaseEntity<TKey> : BaseEntity
    {
        [Key]
        [DataMember]
        [DisplayName("Id")]
        public virtual TKey Id { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseEntity<TKey>);
        }

        public virtual bool Equals(BaseEntity<TKey> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetType();
                var thisType = GetType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        private static bool IsTransient(BaseEntity<TKey> obj)
        {
            return obj != null && Equals(obj.Id, default(TKey));
        }

        public override bool IsTransient()
        {
            return Equals(Id, default(TKey));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override object GetIdValue()
        {
            return Id;
        }

        public override void OnInserting()
        {
            if (typeof(TKey) == typeof(Guid) && Id == (dynamic)default(TKey))
            {
                Id = (dynamic)Guid.NewGuid();
            }
        }

        public static bool operator ==(BaseEntity<TKey> x, BaseEntity<TKey> y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(BaseEntity<TKey> x, BaseEntity<TKey> y)
        {
            return !(x == y);
        }
    }
}