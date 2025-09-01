namespace RenovationRumble.Logic.Serialization
{
    using System;

    /// <summary>
    /// A custom attribute to mark a class as a discriminated union.
    /// This is backed by a Roselyn source generator to avoid reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class JsonDiscriminatedUnionAttribute : Attribute
    {
        public Type EnumType { get; }
        public string EnumProperty { get; }
        public string Discriminator { get; }
        
        /// <param name="enumType">The enum used as discriminator.</param>
        /// <param name="enumProperty">Name of the base property that returns the enum.</param>
        /// <param name="discriminator">JSON field name with the default being 'type'.</param>
        public JsonDiscriminatedUnionAttribute(Type enumType, string enumProperty, string discriminator = "type")
        {
            EnumType = enumType; 
            EnumProperty = enumProperty;
            Discriminator = discriminator;
        }
    }
}