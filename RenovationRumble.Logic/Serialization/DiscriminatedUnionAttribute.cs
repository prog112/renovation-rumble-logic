namespace RenovationRumble.Logic.Serialization
{
    using System;

    /// <summary>
    /// A custom attribute to mark a class as a discriminated union.
    /// This is backed by a Roselyn source generator to avoid reflection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class DiscriminatedUnionAttribute : Attribute
    {        
        /// <param name="enumType">The enum used as discriminator.</param>
        /// <param name="enumProperty">Name of the base property that returns the enum.</param>
        /// <param name="discriminator">Generated field name with the default being 'type'.</param>
        // ReSharper disable UnusedParameter.Local
        public DiscriminatedUnionAttribute(Type enumType, string enumProperty, string discriminator = "type")
        {
            // No need to do anything with the args as they get intercepted by the Roselyn analyzer.
            // See the generated DiscriminatedUnionRegistry class.
        }
    }
}