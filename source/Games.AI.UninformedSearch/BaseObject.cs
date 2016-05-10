using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Games.AI.UninformedSearch
{
    /// <summary>
    /// Facilitates indicating which property(s) describe the unique signature of an 
    /// entity. See BaseObject.GetTypeSpecificSignatureProperties() for when this is leveraged.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DomainSignatureAttribute : Attribute
    {
    }

    /// <summary>
    ///     Provides a standard base class for facilitating comparison of objects.
    /// 
    ///     For a discussion of the implementation of Equals/GetHashCode, see 
    ///     http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    ///     and http://groups.google.com/group/sharp-architecture/browse_thread/thread/f76d1678e68e3ece?hl=en for 
    ///     an in depth and conclusive resolution.
    /// </summary>    
    /// <see></see>"http://github.com/sharparchitecture/Sharp-Architecture/blob/master/Solutions/SharpArch.Domain/DomainModel/BaseObject.cs"/>
    [Serializable]
    public abstract class BaseObject
    {
        /// <summary>
        ///     To help ensure hashcode uniqueness, a carefully selected random number multiplier 
        ///     is used within the calculation.  Goodrich and Tamassia's Data Structures and
        ///     Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        ///     of collissions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        ///     for more information.
        /// </summary>
        private const int HashMultiplier = 31;

        /// <summary>
        ///     This static member caches the domain signature properties to avoid looking them up for 
        ///     each instance of the same type.
        /// 
        ///     A description of the very slick ThreadStatic attribute may be found at 
        ///     http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2005/08/18/132026.aspx
        /// </summary>
        [ThreadStatic]
        private static Dictionary<Type, IEnumerable<PropertyInfo>> signaturePropertiesDictionary;

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var compareTo = obj as BaseObject;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            return compareTo != null && GetType().Equals(compareTo.GetTypeUnproxied()) &&
                   HasSameObjectSignatureAs(compareTo);
        }

        /// <summary>
        ///     This is used to provide the hashcode identifier of an object using the signature 
        ///     properties of the object; although it's necessary for NHibernate's use, this can 
        ///     also be useful for business logic purposes and has been included in this base 
        ///     class, accordingly.  Since it is recommended that GetHashCode change infrequently, 
        ///     if at all, in an object's lifetime, it's important that properties are carefully
        ///     selected which truly represent the signature of an object.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                var signatureProperties = GetSignatureProperties().ToList();

                // It's possible for two objects to return the same hash code based on 
                // identically valued properties, even if they're of two different types, 
                // so we include the object's type in the hash calculation
                var hashCode = GetType().GetHashCode();

                hashCode = signatureProperties.Select(property => property.GetValue(this, null))
                                              .Where(value => value != null)
                                              .Aggregate(hashCode, (current, value) => (current * HashMultiplier) ^ value.GetHashCode());

                if (signatureProperties.Any())
                {
                    return hashCode;
                }

                // If no properties were flagged as being part of the signature of the object,
                // then simply return the hashcode of the base object as the hashcode.
                // ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
                return base.GetHashCode();
            }
        }

        /// <summary>
        /// Gets the properties that determine the identity of this object.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<PropertyInfo> GetSignatureProperties()
        {
            IEnumerable<PropertyInfo> properties;

            // Init the signaturePropertiesDictionary here due to reasons described at 
            // http://blogs.msdn.com/jfoscoding/archive/2006/07/18/670497.aspx
            if (signaturePropertiesDictionary == null)
            {
                signaturePropertiesDictionary = new Dictionary<Type, IEnumerable<PropertyInfo>>();
            }

            if (signaturePropertiesDictionary.TryGetValue(GetType(), out properties))
            {
                return properties;
            }

            return signaturePropertiesDictionary[GetType()] = GetTypeSpecificSignatureProperties();
        }

        /// <summary>
        ///     You may override this method to provide your own comparison routine.
        /// </summary>
        public virtual bool HasSameObjectSignatureAs(BaseObject compareTo)
        {
            var signatureProperties = GetSignatureProperties().ToList();

            if ((from property in signatureProperties
                 let valueOfThisObject = property.GetValue(this, null)
                 let valueToCompareTo = property.GetValue(compareTo, null)
                 where valueOfThisObject != null || valueToCompareTo != null
                 where (valueOfThisObject == null ^ valueToCompareTo == null) || (!valueOfThisObject.Equals(valueToCompareTo))
                 select valueOfThisObject).Any())
            {
                return false;
            }

            // If we've gotten this far and signature properties were found, then we can
            // assume that everything matched; otherwise, if there were no signature 
            // properties, then simply return the default bahavior of Equals
            // ReSharper disable once BaseObjectEqualsIsObjectEquals
            return signatureProperties.Any() || base.Equals(compareTo);
        }

        /// <summary>
        ///     Enforces the template method pattern to have child objects determine which specific 
        ///     properties should and should not be included in the object signature comparison. Note
        ///     that the the BaseObject already takes care of performance caching, so this method 
        ///     shouldn't worry about caching...just return the goods man!
        /// </summary>
        protected virtual IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            return
                GetType().GetProperties().Where(
                    p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));
        }

        /// <summary>
        ///     When NHibernate proxies objects, it masks the type of the actual entity object.
        ///     This wrapper burrows into the proxied object to get its actual type.
        /// 
        ///     Although this assumes NHibernate is being used, it doesn't require any NHibernate
        ///     related dependencies and has no bad side effects if NHibernate isn't being used.
        /// 
        ///     Related discussion is at http://groups.google.com/group/sharp-architecture/browse_thread/thread/ddd05f9baede023a ...thanks Jay Oliver!
        /// </summary>
        protected virtual Type GetTypeUnproxied()
        {
            return GetType();
        }

        /// <summary>
        /// Overrides the equals operator to have the same functionality
        /// as the .Equals() method. Is true if obj1 is equal to obj2.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param>        
        public static bool operator ==(BaseObject obj1, BaseObject obj2)
        {
            if ((object)obj1 == null || (object)obj2 == null)
                return Equals(obj1, obj2);

            return obj1.Equals(obj2);
        }

        /// <summary>
        /// Overrides the not equals operator to have the same functionality
        /// as the negated .Equals() method. Is true if obj1 is not equal to obj2.
        /// </summary>
        /// <param name="obj1">The obj1.</param>
        /// <param name="obj2">The obj2.</param> 
        public static bool operator !=(BaseObject obj1, BaseObject obj2)
        {
            return !(obj1 == obj2);
        }
    }
}
