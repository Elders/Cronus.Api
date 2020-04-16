using System;
using System.Runtime.Serialization;

namespace Elders.Cronus.Api.Playground.Domain.Samples
{
    [DataContract(Name = "12f54e09-f844-4613-b472-6c6cf4a37c71")]
    public class Volume : ValueObject<Volume>
    {
        Volume() { }

        public Volume(int ammount)
        {
            if (ammount <= 0) throw new ArgumentException("Volume ammount must be possitive.");

            Ammount = ammount;
        }

        [DataMember(Order = 1)]
        public int Ammount { get; private set; }
    }
}
